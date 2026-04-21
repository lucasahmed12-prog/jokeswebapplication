using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace jokesWebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddJokeAnswerColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "JokesQuestion",
                table: "Joke",
                newName: "JokeQuestion");

            migrationBuilder.RenameColumn(
                name: "JokesAnswer",
                table: "Joke",
                newName: "JokeAnswer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "JokeQuestion",
                table: "Joke",
                newName: "JokesQuestion");

            migrationBuilder.RenameColumn(
                name: "JokeAnswer",
                table: "Joke",
                newName: "JokesAnswer");
        }
    }
}
