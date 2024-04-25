using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoftwareTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class licenseExp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LicenseExp",
                table: "Licenses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LicenseExp",
                table: "Licenses");
        }
    }
}
