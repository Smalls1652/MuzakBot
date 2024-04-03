using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MuzakBot.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "song_lyrics",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    artistName = table.Column<string>(type: "TEXT", nullable: false),
                    songName = table.Column<string>(type: "TEXT", nullable: false),
                    lyrics = table.Column<string>(type: "TEXT", nullable: false),
                    createdAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    partitionKey = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_song_lyrics", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "song_lyrics_request_jobs",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    geniusUrl = table.Column<string>(type: "TEXT", nullable: false),
                    createdAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    standaloneServiceAcknowledged = table.Column<bool>(type: "INTEGER", nullable: false),
                    fallbackMethodNeeded = table.Column<bool>(type: "INTEGER", nullable: false),
                    isCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    partitionKey = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_song_lyrics_request_jobs", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "song_lyrics");

            migrationBuilder.DropTable(
                name: "song_lyrics_request_jobs");
        }
    }
}
