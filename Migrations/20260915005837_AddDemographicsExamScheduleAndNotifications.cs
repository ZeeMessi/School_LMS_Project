using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolLMS.Migrations
{
    /// <inheritdoc />
    public partial class AddDemographicsExamScheduleAndNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Teachers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BloodGroup",
                table: "Teachers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CnicNumber",
                table: "Teachers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactNumber",
                table: "Teachers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BloodGroup",
                table: "Students",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CnicOrBFormNumber",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactNumber",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "GuardianContactNumber",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuardianName",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Exams",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "RecordedAt",
                table: "ExamResults",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "RelatedExamTypeId",
                table: "Announcements",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StudentNavViews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    Section = table.Column<int>(type: "int", nullable: false),
                    LastViewedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentNavViews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentNavViews_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_RelatedExamTypeId",
                table: "Announcements",
                column: "RelatedExamTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentNavViews_StudentId_Section",
                table: "StudentNavViews",
                columns: new[] { "StudentId", "Section" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_ExamTypes_RelatedExamTypeId",
                table: "Announcements",
                column: "RelatedExamTypeId",
                principalTable: "ExamTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_ExamTypes_RelatedExamTypeId",
                table: "Announcements");

            migrationBuilder.DropTable(
                name: "StudentNavViews");

            migrationBuilder.DropIndex(
                name: "IX_Announcements_RelatedExamTypeId",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "BloodGroup",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "CnicNumber",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "ContactNumber",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "BloodGroup",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "CnicOrBFormNumber",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "ContactNumber",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "GuardianContactNumber",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "GuardianName",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "RecordedAt",
                table: "ExamResults");

            migrationBuilder.DropColumn(
                name: "RelatedExamTypeId",
                table: "Announcements");
        }
    }
}
