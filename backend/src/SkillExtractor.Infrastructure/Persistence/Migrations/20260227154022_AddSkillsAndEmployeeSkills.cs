using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillExtractor.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSkillsAndEmployeeSkills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "employee_skills",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    skill_id = table.Column<Guid>(type: "uuid", nullable: false),
                    proficiency_level = table.Column<int>(type: "integer", nullable: false),
                    is_manual_override = table.Column<bool>(type: "boolean", nullable: false),
                    source_document_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_document_type = table.Column<int>(type: "integer", nullable: false),
                    extracted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_employee_skills", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "skills",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    aliases = table.Column<List<string>>(type: "text[]", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_skills", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_employee_skills_user_id",
                table: "employee_skills",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_employee_skills_user_id_skill_id",
                table: "employee_skills",
                columns: new[] { "user_id", "skill_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_employee_skills_user_id_source_document_type_is_manual_over",
                table: "employee_skills",
                columns: new[] { "user_id", "source_document_type", "is_manual_override" });

            migrationBuilder.CreateIndex(
                name: "ix_skills_is_active",
                table: "skills",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_skills_name",
                table: "skills",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "employee_skills");

            migrationBuilder.DropTable(
                name: "skills");
        }
    }
}
