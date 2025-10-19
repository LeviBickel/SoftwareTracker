using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoftwareTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add index on AddedBy for faster user-scoped queries
            migrationBuilder.CreateIndex(
                name: "IX_Licenses_AddedBy",
                table: "Licenses",
                column: "AddedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Archival_AddedBy",
                table: "Archival",
                column: "AddedBy");

            // Composite index for common query patterns
            migrationBuilder.CreateIndex(
                name: "IX_Licenses_AddedBy_LicenseExp",
                table: "Licenses",
                columns: new[] { "AddedBy", "LicenseExp" });

            migrationBuilder.CreateIndex(
                name: "IX_Licenses_AddedBy_SupportExp",
                table: "Licenses",
                columns: new[] { "AddedBy", "SupportExp" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Licenses_AddedBy",
                table: "Licenses");

            migrationBuilder.DropIndex(
                name: "IX_Archival_AddedBy",
                table: "Archival");

            migrationBuilder.DropIndex(
                name: "IX_Licenses_AddedBy_LicenseExp",
                table: "Licenses");

            migrationBuilder.DropIndex(
                name: "IX_Licenses_AddedBy_SupportExp",
                table: "Licenses");
        }
    }
}
