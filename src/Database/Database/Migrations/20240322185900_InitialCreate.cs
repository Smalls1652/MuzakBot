using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MuzakBot.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    RateLimitEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    RateLimitMaxRequests = table.Column<int>(type: "INTEGER", nullable: false),
                    RateLimitIgnoredUserIds = table.Column<string>(type: "TEXT", nullable: true),
                    CommandIsEnabledToSpecificGuilds = table.Column<bool>(type: "INTEGER", nullable: false),
                    CommandEnabledGuildIds = table.Column<string>(type: "TEXT", nullable: true),
                    CommandDisabledGuildIds = table.Column<string>(type: "TEXT", nullable: true),
                    PartitionKey = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PromptStyles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ShortName = table.Column<string>(type: "TEXT", nullable: false),
                    AnalysisType = table.Column<string>(type: "TEXT", nullable: false),
                    Prompt = table.Column<string>(type: "TEXT", nullable: false),
                    UserPrompt = table.Column<string>(type: "TEXT", nullable: false),
                    NoticeText = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    LastUpdated = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    PartitionKey = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromptStyles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SongLyrics",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ArtistName = table.Column<string>(type: "TEXT", nullable: false),
                    SongName = table.Column<string>(type: "TEXT", nullable: false),
                    Lyrics = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    PartitionKey = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SongLyrics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRateLimits",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    CurrentRequestCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LastRequestTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    PartitionKey = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRateLimits", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configs");

            migrationBuilder.DropTable(
                name: "PromptStyles");

            migrationBuilder.DropTable(
                name: "SongLyrics");

            migrationBuilder.DropTable(
                name: "UserRateLimits");
        }
    }
}
