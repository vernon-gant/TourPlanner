using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TP.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenamedToMeters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Distance",
                table: "Tours",
                newName: "DistanceMeters");

            migrationBuilder.RenameColumn(
                name: "TotalDistance",
                table: "TourLogs",
                newName: "TotalDistanceMeters");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DistanceMeters",
                table: "Tours",
                newName: "Distance");

            migrationBuilder.RenameColumn(
                name: "TotalDistanceMeters",
                table: "TourLogs",
                newName: "TotalDistance");
        }
    }
}
