using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Data.Database.Migrations
{
    /// <inheritdoc />
    public partial class LinkNotesToTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "Notes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TaskId",
                table: "Notes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notes_TaskId",
                table: "Notes",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_Tasks_TaskId",
                table: "Notes",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notes_Tasks_TaskId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Notes_TaskId",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "Notes");
        }
    }
}
