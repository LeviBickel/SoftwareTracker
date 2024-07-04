using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoftwareTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class notifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "NotifyOnLicExp",
                table: "Licenses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotifyOnSupExp",
                table: "Licenses",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotifyOnLicExp",
                table: "Licenses");

            migrationBuilder.DropColumn(
                name: "NotifyOnSupExp",
                table: "Licenses");
        }
    }
}
