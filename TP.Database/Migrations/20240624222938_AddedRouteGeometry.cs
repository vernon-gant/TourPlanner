using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TP.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedRouteGeometry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RouteGeometry",
                table: "Tours",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RouteGeometry",
                table: "Tours");
        }
    }
}
