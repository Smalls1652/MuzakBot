using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MuzakBot.Database.Migrations
{
    /// <inheritdoc />
    public partial class Add_Currently_Remaining_DatabaseItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "command_configs",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    rateLimitEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    rateLimitMaxRequests = table.Column<int>(type: "INTEGER", nullable: false),
                    rateLimitIgnoredUserIds = table.Column<string>(type: "TEXT", nullable: true),
                    commandIsEnabledToSpecificGuilds = table.Column<bool>(type: "INTEGER", nullable: false),
                    commandEnabledGuildIds = table.Column<string>(type: "TEXT", nullable: true),
                    commandDisabledGuildIds = table.Column<string>(type: "TEXT", nullable: true),
                    partitionKey = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_command_configs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "prompt_styles",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    shortName = table.Column<string>(type: "TEXT", nullable: false),
                    analysisType = table.Column<string>(type: "TEXT", nullable: false),
                    prompt = table.Column<string>(type: "TEXT", nullable: false),
                    UserPrompt = table.Column<string>(type: "TEXT", nullable: false),
                    noticeText = table.Column<string>(type: "TEXT", nullable: false),
                    createdOn = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    lastUpdated = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    partitionKey = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prompt_styles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_rate_limit",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    userId = table.Column<string>(type: "TEXT", nullable: false),
                    currentRequestCount = table.Column<int>(type: "INTEGER", nullable: false),
                    lastRequestTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    partitionKey = table.Column<string>(type: "TEXT", nullable: false)
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
                name: "command_configs");

            migrationBuilder.DropTable(
                name: "prompt_styles");

            migrationBuilder.DropTable(
                name: "user_rate_limit");
        }
    }
}
