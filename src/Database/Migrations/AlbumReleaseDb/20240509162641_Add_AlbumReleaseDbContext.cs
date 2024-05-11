using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MuzakBot.Database.Migrations.AlbumReleaseDb
{
    /// <inheritdoc />
    public partial class Add_AlbumReleaseDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "release_reminders",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    artistId = table.Column<string>(type: "TEXT", nullable: false),
                    albumId = table.Column<string>(type: "TEXT", nullable: false),
                    guildId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    userIdsToRemind = table.Column<string>(type: "TEXT", nullable: false),
                    releaseDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    partitionKey = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_release_reminders", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "release_reminders");
        }
    }
}
