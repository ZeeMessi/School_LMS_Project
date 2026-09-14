using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolLMS.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseMaterialsRemarksAndGender : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AttachmentUrl",
                table: "Announcements",
                newName: "AttachmentFileName");

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "Teachers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentContentType",
                table: "Announcements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "AttachmentData",
                table: "Announcements",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TeacherId",
                table: "Announcements",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CourseMaterials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassSubjectId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: true),
                    PostedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileData = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    FileContentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseMaterials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseMaterials_ClassSubjects_ClassSubjectId",
                        column: x => x.ClassSubjectId,
                        principalTable: "ClassSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeacherRemarks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    ClassSubjectId = table.Column<int>(type: "int", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherRemarks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherRemarks_ClassSubjects_ClassSubjectId",
                        column: x => x.ClassSubjectId,
                        principalTable: "ClassSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherRemarks_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_TeacherId",
                table: "Announcements",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMaterials_ClassSubjectId",
                table: "CourseMaterials",
                column: "ClassSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherRemarks_ClassSubjectId",
                table: "TeacherRemarks",
                column: "ClassSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherRemarks_StudentId",
                table: "TeacherRemarks",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_Teachers_TeacherId",
                table: "Announcements",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_Teachers_TeacherId",
                table: "Announcements");

            migrationBuilder.DropTable(
                name: "CourseMaterials");

            migrationBuilder.DropTable(
                name: "TeacherRemarks");

            migrationBuilder.DropIndex(
                name: "IX_Announcements_TeacherId",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "AttachmentContentType",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "AttachmentData",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "Announcements");

            migrationBuilder.RenameColumn(
                name: "AttachmentFileName",
                table: "Announcements",
                newName: "AttachmentUrl");
        }
    }
}
