using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddTemplateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkoutTemplateId",
                table: "Workouts",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SetTemplateId",
                table: "Sets",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExerciseTemplateId",
                table: "Exercises",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WorkoutTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Workout_Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Exercise_Name = table.Column<string>(type: "text", nullable: false),
                    Max_Set = table.Column<int>(type: "integer", nullable: false),
                    Min_Set = table.Column<int>(type: "integer", nullable: false),
                    Weight_Step = table.Column<decimal>(type: "numeric", nullable: false),
                    Volume_Step = table.Column<int>(type: "integer", nullable: false),
                    WorkoutTemplateId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseTemplates_WorkoutTemplates_WorkoutTemplateId",
                        column: x => x.WorkoutTemplateId,
                        principalTable: "WorkoutTemplates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SetTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Failure_Set = table.Column<bool>(type: "boolean", nullable: false),
                    Max_Reps = table.Column<int>(type: "integer", nullable: false),
                    Min_Reps = table.Column<int>(type: "integer", nullable: false),
                    Exercise_TemplateId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SetTemplates_ExerciseTemplates_Exercise_TemplateId",
                        column: x => x.Exercise_TemplateId,
                        principalTable: "ExerciseTemplates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_WorkoutTemplateId",
                table: "Workouts",
                column: "WorkoutTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Sets_SetTemplateId",
                table: "Sets",
                column: "SetTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_ExerciseTemplateId",
                table: "Exercises",
                column: "ExerciseTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseTemplates_WorkoutTemplateId",
                table: "ExerciseTemplates",
                column: "WorkoutTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_SetTemplates_Exercise_TemplateId",
                table: "SetTemplates",
                column: "Exercise_TemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercises_ExerciseTemplates_ExerciseTemplateId",
                table: "Exercises",
                column: "ExerciseTemplateId",
                principalTable: "ExerciseTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Sets_SetTemplates_SetTemplateId",
                table: "Sets",
                column: "SetTemplateId",
                principalTable: "SetTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Workouts_WorkoutTemplates_WorkoutTemplateId",
                table: "Workouts",
                column: "WorkoutTemplateId",
                principalTable: "WorkoutTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercises_ExerciseTemplates_ExerciseTemplateId",
                table: "Exercises");

            migrationBuilder.DropForeignKey(
                name: "FK_Sets_SetTemplates_SetTemplateId",
                table: "Sets");

            migrationBuilder.DropForeignKey(
                name: "FK_Workouts_WorkoutTemplates_WorkoutTemplateId",
                table: "Workouts");

            migrationBuilder.DropTable(
                name: "SetTemplates");

            migrationBuilder.DropTable(
                name: "ExerciseTemplates");

            migrationBuilder.DropTable(
                name: "WorkoutTemplates");

            migrationBuilder.DropIndex(
                name: "IX_Workouts_WorkoutTemplateId",
                table: "Workouts");

            migrationBuilder.DropIndex(
                name: "IX_Sets_SetTemplateId",
                table: "Sets");

            migrationBuilder.DropIndex(
                name: "IX_Exercises_ExerciseTemplateId",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "WorkoutTemplateId",
                table: "Workouts");

            migrationBuilder.DropColumn(
                name: "SetTemplateId",
                table: "Sets");

            migrationBuilder.DropColumn(
                name: "ExerciseTemplateId",
                table: "Exercises");
        }
    }
}
