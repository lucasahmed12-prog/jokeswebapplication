using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace jokesWebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddThumbnailToJoke : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "Joke",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "Joke");
        }
    }
}
