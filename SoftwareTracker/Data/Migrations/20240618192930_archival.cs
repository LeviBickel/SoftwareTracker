using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoftwareTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class archival : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Archival",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Manufacturer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoftwareTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssignedServer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PurchaseOrder = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LicenseType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicenseExp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Support = table.Column<bool>(type: "bit", nullable: false),
                    SupportExp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AmountofKeys = table.Column<int>(type: "int", nullable: false),
                    UsedKeys = table.Column<int>(type: "int", nullable: false),
                    RemainingKeys = table.Column<int>(type: "int", nullable: false),
                    LicenseKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notified = table.Column<bool>(type: "bit", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Archival", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Archival");
        }
    }
}
