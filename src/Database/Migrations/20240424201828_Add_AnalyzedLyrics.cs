using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MuzakBot.Database.Migrations
{
    /// <inheritdoc />
    public partial class Add_AnalyzedLyrics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "analyzed_lyrics",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    createdAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    artistName = table.Column<string>(type: "TEXT", nullable: false),
                    songName = table.Column<string>(type: "TEXT", nullable: false),
                    promptStyleUsed = table.Column<string>(type: "TEXT", nullable: false),
                    songLyricsId = table.Column<string>(type: "TEXT", nullable: false),
                    analysis = table.Column<string>(type: "TEXT", nullable: false),
                    partitionKey = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_analyzed_lyrics", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "analyzed_lyrics");
        }
    }
}
