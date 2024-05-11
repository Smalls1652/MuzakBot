using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MuzakBot.Database.Migrations
{
    /// <inheritdoc />
    public partial class LyricsAnalyzerPromptStyle_Missing_Property : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserPrompt",
                table: "prompt_styles",
                newName: "userPrompt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "userPrompt",
                table: "prompt_styles",
                newName: "UserPrompt");
        }
    }
}
