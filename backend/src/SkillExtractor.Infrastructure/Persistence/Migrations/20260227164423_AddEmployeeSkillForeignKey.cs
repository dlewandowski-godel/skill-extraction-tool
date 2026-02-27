using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillExtractor.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeSkillForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_employee_skills_skill_id",
                table: "employee_skills",
                column: "skill_id");

            migrationBuilder.AddForeignKey(
                name: "fk_employee_skills_skills_skill_id",
                table: "employee_skills",
                column: "skill_id",
                principalTable: "skills",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_employee_skills_skills_skill_id",
                table: "employee_skills");

            migrationBuilder.DropIndex(
                name: "ix_employee_skills_skill_id",
                table: "employee_skills");
        }
    }
}
