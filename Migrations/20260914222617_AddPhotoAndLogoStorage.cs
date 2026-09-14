using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolLMS.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoAndLogoStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhotoUrl",
                table: "Teachers",
                newName: "PhotoContentType");

            migrationBuilder.RenameColumn(
                name: "PhotoUrl",
                table: "Students",
                newName: "PhotoContentType");

            migrationBuilder.RenameColumn(
                name: "LogoUrl",
                table: "Schools",
                newName: "LogoContentType");

            migrationBuilder.AddColumn<byte[]>(
                name: "PhotoData",
                table: "Teachers",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "PhotoData",
                table: "Students",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "LogoData",
                table: "Schools",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoData",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "PhotoData",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "LogoData",
                table: "Schools");

            migrationBuilder.RenameColumn(
                name: "PhotoContentType",
                table: "Teachers",
                newName: "PhotoUrl");

            migrationBuilder.RenameColumn(
                name: "PhotoContentType",
                table: "Students",
                newName: "PhotoUrl");

            migrationBuilder.RenameColumn(
                name: "LogoContentType",
                table: "Schools",
                newName: "LogoUrl");
        }
    }
}
