using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeleteTypenoAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Otps_Users_ReceiverId",
                table: "Otps");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProjects_Projects_ProjectId",
                table: "UserProjects");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProjects_Users_UserId",
                table: "UserProjects");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkItemRelations_WorkItems_ChildWorkItemId",
                table: "WorkItemRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkItemRelations_WorkItems_ParentWorkItemId",
                table: "WorkItemRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkItems_Sprints_SprintId",
                table: "WorkItems");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkItems_Users_AssignedToUserId",
                table: "WorkItems");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -1,
                column: "Email",
                value: "muhalsukhni@gmail.com");

            migrationBuilder.AddForeignKey(
                name: "FK_Otps_Users_ReceiverId",
                table: "Otps",
                column: "ReceiverId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProjects_Projects_ProjectId",
                table: "UserProjects",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProjects_Users_UserId",
                table: "UserProjects",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkItemRelations_WorkItems_ChildWorkItemId",
                table: "WorkItemRelations",
                column: "ChildWorkItemId",
                principalTable: "WorkItems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkItemRelations_WorkItems_ParentWorkItemId",
                table: "WorkItemRelations",
                column: "ParentWorkItemId",
                principalTable: "WorkItems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkItems_Sprints_SprintId",
                table: "WorkItems",
                column: "SprintId",
                principalTable: "Sprints",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkItems_Users_AssignedToUserId",
                table: "WorkItems",
                column: "AssignedToUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Otps_Users_ReceiverId",
                table: "Otps");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProjects_Projects_ProjectId",
                table: "UserProjects");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProjects_Users_UserId",
                table: "UserProjects");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkItemRelations_WorkItems_ChildWorkItemId",
                table: "WorkItemRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkItemRelations_WorkItems_ParentWorkItemId",
                table: "WorkItemRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkItems_Sprints_SprintId",
                table: "WorkItems");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkItems_Users_AssignedToUserId",
                table: "WorkItems");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -1,
                column: "Email",
                value: "admin@taskmanager.local");

            migrationBuilder.AddForeignKey(
                name: "FK_Otps_Users_ReceiverId",
                table: "Otps",
                column: "ReceiverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProjects_Projects_ProjectId",
                table: "UserProjects",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProjects_Users_UserId",
                table: "UserProjects",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkItemRelations_WorkItems_ChildWorkItemId",
                table: "WorkItemRelations",
                column: "ChildWorkItemId",
                principalTable: "WorkItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkItemRelations_WorkItems_ParentWorkItemId",
                table: "WorkItemRelations",
                column: "ParentWorkItemId",
                principalTable: "WorkItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkItems_Sprints_SprintId",
                table: "WorkItems",
                column: "SprintId",
                principalTable: "Sprints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkItems_Users_AssignedToUserId",
                table: "WorkItems",
                column: "AssignedToUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
