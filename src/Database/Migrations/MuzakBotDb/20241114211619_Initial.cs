using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MuzakBot.Database.Migrations.MuzakBotDb
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "analyzed_lyrics",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    createdAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    artistName = table.Column<string>(type: "text", nullable: false),
                    songName = table.Column<string>(type: "text", nullable: false),
                    promptStyleUsed = table.Column<string>(type: "text", nullable: false),
                    songLyricsId = table.Column<string>(type: "text", nullable: false),
                    analysis = table.Column<string>(type: "text", nullable: false),
                    partitionKey = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_analyzed_lyrics", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "command_configs",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    rateLimitEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    rateLimitMaxRequests = table.Column<int>(type: "integer", nullable: false),
                    rateLimitIgnoredUserIds = table.Column<List<string>>(type: "text[]", nullable: true),
                    commandIsEnabledToSpecificGuilds = table.Column<bool>(type: "boolean", nullable: false),
                    commandEnabledGuildIds = table.Column<decimal[]>(type: "numeric(20,0)[]", nullable: true),
                    commandDisabledGuildIds = table.Column<decimal[]>(type: "numeric(20,0)[]", nullable: true),
                    partitionKey = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_command_configs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "database_updates",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    migratedToEfCore = table.Column<bool>(type: "boolean", nullable: false),
                    partitionKey = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_database_updates", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "LyricsAnalyzerItems",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    artistName = table.Column<string>(type: "text", nullable: false),
                    songName = table.Column<string>(type: "text", nullable: false),
                    promptStyle = table.Column<string>(type: "text", nullable: false),
                    createdAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    partitionKey = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LyricsAnalyzerItems", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "prompt_styles",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    shortName = table.Column<string>(type: "text", nullable: false),
                    gptModel = table.Column<string>(type: "text", nullable: false),
                    analysisType = table.Column<string>(type: "text", nullable: false),
                    prompt = table.Column<string>(type: "text", nullable: false),
                    userPrompt = table.Column<string>(type: "text", nullable: false),
                    noticeText = table.Column<string>(type: "text", nullable: false),
                    createdOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    lastUpdated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    partitionKey = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prompt_styles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "release_reminders",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    albumId = table.Column<string>(type: "text", nullable: false),
                    guildId = table.Column<string>(type: "text", nullable: false),
                    channelId = table.Column<string>(type: "text", nullable: false),
                    userIdsToRemind = table.Column<List<string>>(type: "text[]", nullable: false),
                    releaseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    reminderSent = table.Column<bool>(type: "boolean", nullable: false),
                    partitionKey = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_release_reminders", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "SongLyricsItems",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    artistName = table.Column<string>(type: "text", nullable: false),
                    songName = table.Column<string>(type: "text", nullable: false),
                    lyrics = table.Column<string>(type: "text", nullable: false),
                    createdAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    partitionKey = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongLyricsItems", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "SongLyricsRequestJobs",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    geniusUrl = table.Column<string>(type: "text", nullable: false),
                    createdAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    standaloneServiceAcknowledged = table.Column<bool>(type: "boolean", nullable: false),
                    fallbackMethodNeeded = table.Column<bool>(type: "boolean", nullable: false),
                    isCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    songLyricsItemId = table.Column<string>(type: "text", nullable: true),
                    partitionKey = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongLyricsRequestJobs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_rate_limit",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    userId = table.Column<string>(type: "text", nullable: false),
                    currentRequestCount = table.Column<int>(type: "integer", nullable: false),
                    lastRequestTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    partitionKey = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_rate_limit", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "analyzed_lyrics");

            migrationBuilder.DropTable(
                name: "command_configs");

            migrationBuilder.DropTable(
                name: "database_updates");

            migrationBuilder.DropTable(
                name: "LyricsAnalyzerItems");

            migrationBuilder.DropTable(
                name: "prompt_styles");

            migrationBuilder.DropTable(
                name: "release_reminders");

            migrationBuilder.DropTable(
                name: "SongLyricsItems");

            migrationBuilder.DropTable(
                name: "SongLyricsRequestJobs");

            migrationBuilder.DropTable(
                name: "user_rate_limit");
        }
    }
}
