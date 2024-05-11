using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MuzakBot.Database.Migrations.AlbumReleaseDb
{
    /// <inheritdoc />
    public partial class Remove_ArtistId_Property : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "artistId",
                table: "release_reminders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "artistId",
                table: "release_reminders",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
