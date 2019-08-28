using Microsoft.EntityFrameworkCore.Migrations;

namespace CardGame.API.Migrations
{
    public partial class GameTurnCompleted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TurnCompleted",
                table: "Games",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TurnCompleted",
                table: "Games");
        }
    }
}
