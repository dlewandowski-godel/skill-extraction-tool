using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillExtractor.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSkillCreatedAtAndCompositeIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_skills_name",
                table: "skills");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "skills",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "ix_skills_name_category",
                table: "skills",
                columns: new[] { "name", "category" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_skills_name_category",
                table: "skills");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "skills");

            migrationBuilder.CreateIndex(
                name: "ix_skills_name",
                table: "skills",
                column: "name",
                unique: true);
        }
    }
}
