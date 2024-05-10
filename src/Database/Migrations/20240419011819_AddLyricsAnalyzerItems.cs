using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MuzakBot.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddLyricsAnalyzerItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "lyrics_analyzer_items",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    artistName = table.Column<string>(type: "TEXT", nullable: false),
                    songName = table.Column<string>(type: "TEXT", nullable: false),
                    promptStyle = table.Column<string>(type: "TEXT", nullable: false),
                    createdAt = table.Column<DateTimeOffset>(type: "TEXT", rowVersion: true, nullable: false),
                    partitionKey = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lyrics_analyzer_items", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "lyrics_analyzer_items");
        }
    }
}