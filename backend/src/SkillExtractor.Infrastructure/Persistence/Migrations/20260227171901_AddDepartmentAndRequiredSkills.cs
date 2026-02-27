using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillExtractor.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentAndRequiredSkills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "department",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "department_required_skills",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    department_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    skill_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_department_required_skills", x => x.id);
                    table.ForeignKey(
                        name: "fk_department_required_skills_skills_skill_id",
                        column: x => x.skill_id,
                        principalTable: "skills",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_department_required_skills_department_name_skill_id",
                table: "department_required_skills",
                columns: new[] { "department_name", "skill_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_department_required_skills_skill_id",
                table: "department_required_skills",
                column: "skill_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "department_required_skills");

            migrationBuilder.DropColumn(
                name: "department",
                table: "AspNetUsers");
        }
    }
}
