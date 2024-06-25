using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace TP.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tours",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TransportType = table.Column<int>(type: "integer", nullable: false),
                    Start = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    End = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DistanceMeters = table.Column<decimal>(type: "numeric(10,3)", precision: 10, scale: 3, nullable: false),
                    EstimatedTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Popularity = table.Column<int>(type: "integer", nullable: true),
                    ChildFriendliness = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', CURRENT_TIMESTAMP)"),
                    UnprocessedLogsCounter = table.Column<int>(type: "integer", nullable: false),
                    FTX = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "Name", "Description", "Start", "End" }),
                    EndCoordinates_Latitude = table.Column<decimal>(type: "numeric", nullable: false),
                    EndCoordinates_Longitude = table.Column<decimal>(type: "numeric", nullable: false),
                    StartCoordinates_Latitude = table.Column<decimal>(type: "numeric", nullable: false),
                    StartCoordinates_Longitude = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tours", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TourLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    Difficulty = table.Column<int>(type: "integer", nullable: false),
                    TotalDistanceMeters = table.Column<decimal>(type: "numeric(12,3)", precision: 12, scale: 3, nullable: false),
                    TotalTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Rating = table.Column<short>(type: "smallint", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', CURRENT_TIMESTAMP)"),
                    FTX = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "Comment" }),
                    TourId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourLogs_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TourLogs_FTX",
                table: "TourLogs",
                column: "FTX")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_TourLogs_TourId",
                table: "TourLogs",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_Tours_FTX",
                table: "Tours",
                column: "FTX")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TourLogs");

            migrationBuilder.DropTable(
                name: "Tours");
        }
    }
}
