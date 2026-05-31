using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TaskManager.Core.Exceptions;
using TaskManager.Core.IService;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Repositories;
using TaskManager.Infrastructure.Service;
using TaskManager_p.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddSingleton<ILoginAttemptService, LoginAttemptService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IUserService, UserServices>();
builder.Services.AddScoped<IProjectServices, ProjectServices>();
builder.Services.AddScoped<ISprintServices, SprintServices>();
builder.Services.AddScoped<IWorkItemServices, WorkItemServices>();
builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped<IEmailService, EmailServices>();
builder.Services.AddScoped<ISprintBackgroundJobService, SprintBackgroundJobService>();

builder.Services.AddHangfire(x =>
    x.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHangfireServer();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtKey = builder.Configuration["Jwt:Key"] ??
                 throw new ConfigurationException("Jwt:Key is missing.");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("FrontendPolicy");
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseMiddleware<SaveChangesMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.UseHangfireDashboard();

var jordanTimeZone = GetJordanTimeZone();

RecurringJob.AddOrUpdate<ISprintBackgroundJobService>(
    "close-ended-sprints",
    job => job.CloseEndedSprintsAsync(),
    Cron.Daily(1),
    new RecurringJobOptions
    {
        TimeZone = jordanTimeZone
    });

app.MapControllers();

app.Run();

static TimeZoneInfo GetJordanTimeZone()
{
    foreach (var timeZoneId in new[] { "Jordan Standard Time", "Asia/Amman" })
    {
        if (TimeZoneInfo.TryFindSystemTimeZoneById(timeZoneId, out var timeZone))
            return timeZone;
    }

    return TimeZoneInfo.Local;
}
