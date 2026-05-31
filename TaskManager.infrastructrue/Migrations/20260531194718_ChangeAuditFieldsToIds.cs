using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAuditFieldsToIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "WorkItems");

            migrationBuilder.DropColumn(
                name: "LastUpdatedBy",
                table: "WorkItems");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "WorkItemRelations");

            migrationBuilder.DropColumn(
                name: "LastUpdatedBy",
                table: "WorkItemRelations");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastUpdatedBy",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "UserProjects");

            migrationBuilder.DropColumn(
                name: "LastUpdatedBy",
                table: "UserProjects");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Sprints");

            migrationBuilder.DropColumn(
                name: "LastUpdatedBy",
                table: "Sprints");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "LastUpdatedBy",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Otps");

            migrationBuilder.DropColumn(
                name: "LastUpdatedBy",
                table: "Otps");

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "WorkItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastUpdatedById",
                table: "WorkItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "WorkItemRelations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastUpdatedById",
                table: "WorkItemRelations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastUpdatedById",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "UserProjects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastUpdatedById",
                table: "UserProjects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Sprints",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastUpdatedById",
                table: "Sprints",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastUpdatedById",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Otps",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastUpdatedById",
                table: "Otps",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "CreatedById", "LastUpdatedById" },
                values: new object[] { -1, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "WorkItems");

            migrationBuilder.DropColumn(
                name: "LastUpdatedById",
                table: "WorkItems");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "WorkItemRelations");

            migrationBuilder.DropColumn(
                name: "LastUpdatedById",
                table: "WorkItemRelations");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastUpdatedById",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "UserProjects");

            migrationBuilder.DropColumn(
                name: "LastUpdatedById",
                table: "UserProjects");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Sprints");

            migrationBuilder.DropColumn(
                name: "LastUpdatedById",
                table: "Sprints");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "LastUpdatedById",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Otps");

            migrationBuilder.DropColumn(
                name: "LastUpdatedById",
                table: "Otps");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "WorkItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastUpdatedBy",
                table: "WorkItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "WorkItemRelations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastUpdatedBy",
                table: "WorkItemRelations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastUpdatedBy",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "UserProjects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastUpdatedBy",
                table: "UserProjects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Sprints",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastUpdatedBy",
                table: "Sprints",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastUpdatedBy",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Otps",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastUpdatedBy",
                table: "Otps",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "CreatedBy", "LastUpdatedBy" },
                values: new object[] { "Seed", null });
        }
    }
}
