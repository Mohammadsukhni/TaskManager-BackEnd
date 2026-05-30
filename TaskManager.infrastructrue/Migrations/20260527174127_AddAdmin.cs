using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Email", "FirstName", "IsActive", "IsDeleted", "LastName", "LastUpdatedBy", "LastUpdatedDate", "OtpCode", "OtpExpiration", "PasswordHash", "Phone", "UserRole" },
                values: new object[] { -1, "Seed", new DateTime(2026, 5, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@taskmanager.local", "Mohammad", true, false, "Alsukhni", null, null, null, null, "$2a$11$Fl8jk/Zy1yA/M84RLJkfqeO1tMea4yc4vLm9yejrjNMszzvNMlUA.", "0790000000", 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -1);
        }
    }
}

