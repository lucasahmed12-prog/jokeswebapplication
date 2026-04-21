using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace jokesWebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddJokeLikesFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Joke",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Joke",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "JokeLike",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    JokeId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LikedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JokeLike", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JokeLike_Joke_JokeId",
                        column: x => x.JokeId,
                        principalTable: "Joke",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JokeLike_JokeId",
                table: "JokeLike",
                column: "JokeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JokeLike");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Joke");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Joke");
        }
    }
}
