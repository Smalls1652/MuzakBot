using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MuzakBot.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSongLyricsItemIdProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "songLyricsItemId",
                table: "song_lyrics_request_jobs",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "songLyricsItemId",
                table: "song_lyrics_request_jobs");
        }
    }
}
