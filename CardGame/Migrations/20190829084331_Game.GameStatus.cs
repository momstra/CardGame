using Microsoft.EntityFrameworkCore.Migrations;

namespace CardGame.API.Migrations
{
    public partial class GameGameStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameStarted",
                table: "Games");

            migrationBuilder.AddColumn<byte>(
                name: "GameStatus",
                table: "Games",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameStatus",
                table: "Games");

            migrationBuilder.AddColumn<bool>(
                name: "GameStarted",
                table: "Games",
                nullable: false,
                defaultValue: false);
        }
    }
}
