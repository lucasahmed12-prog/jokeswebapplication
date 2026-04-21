using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace jokesWebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddJokeComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "Joke");

            migrationBuilder.CreateTable(
                name: "JokeComment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CommentText = table.Column<string>(type: "TEXT", nullable: false),
                    JokeId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JokeComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JokeComment_Joke_JokeId",
                        column: x => x.JokeId,
                        principalTable: "Joke",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JokeComment_JokeId",
                table: "JokeComment",
                column: "JokeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JokeComment");

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "Joke",
                type: "TEXT",
                nullable: true);
        }
    }
}
