using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Data.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddLearningMilestones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LearningMilestones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    LearningActivityId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningMilestones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LearningMilestones_LearningActivities_LearningActivityId",
                        column: x => x.LearningActivityId,
                        principalTable: "LearningActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LearningMilestones_LearningActivityId",
                table: "LearningMilestones",
                column: "LearningActivityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LearningMilestones");
        }
    }
}
