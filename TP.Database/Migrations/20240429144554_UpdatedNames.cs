using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TP.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartDescription",
                table: "Tours",
                newName: "Start");

            migrationBuilder.RenameColumn(
                name: "EndDescription",
                table: "Tours",
                newName: "End");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Start",
                table: "Tours",
                newName: "StartDescription");

            migrationBuilder.RenameColumn(
                name: "End",
                table: "Tours",
                newName: "EndDescription");
        }
    }
}
