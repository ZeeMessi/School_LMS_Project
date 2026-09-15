using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolLMS.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentSectionViewTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentSectionViews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    ClassSubjectId = table.Column<int>(type: "int", nullable: false),
                    Section = table.Column<int>(type: "int", nullable: false),
                    LastViewedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentSectionViews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentSectionViews_ClassSubjects_ClassSubjectId",
                        column: x => x.ClassSubjectId,
                        principalTable: "ClassSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentSectionViews_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentSectionViews_ClassSubjectId",
                table: "StudentSectionViews",
                column: "ClassSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSectionViews_StudentId_ClassSubjectId_Section",
                table: "StudentSectionViews",
                columns: new[] { "StudentId", "ClassSubjectId", "Section" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentSectionViews");
        }
    }
}
