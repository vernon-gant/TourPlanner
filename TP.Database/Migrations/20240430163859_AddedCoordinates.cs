using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TP.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedCoordinates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "EndCoordinates_Latitude",
                table: "Tours",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "EndCoordinates_Longitude",
                table: "Tours",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "StartCoordinates_Latitude",
                table: "Tours",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "StartCoordinates_Longitude",
                table: "Tours",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndCoordinates_Latitude",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "EndCoordinates_Longitude",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "StartCoordinates_Latitude",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "StartCoordinates_Longitude",
                table: "Tours");
        }
    }
}
