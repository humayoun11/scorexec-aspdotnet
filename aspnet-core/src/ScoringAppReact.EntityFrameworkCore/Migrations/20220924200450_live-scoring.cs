using Microsoft.EntityFrameworkCore.Migrations;

namespace ScoringAppReact.Migrations
{
    public partial class livescoring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Wickets",
                table: "TeamScores",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TotalScore",
                table: "TeamScores",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "Ball_Dots",
                table: "PlayerScores",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Bat_Dots",
                table: "PlayerScores",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBowling",
                table: "PlayerScores",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsStriker",
                table: "PlayerScores",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ball_Dots",
                table: "PlayerScores");

            migrationBuilder.DropColumn(
                name: "Bat_Dots",
                table: "PlayerScores");

            migrationBuilder.DropColumn(
                name: "IsBowling",
                table: "PlayerScores");

            migrationBuilder.DropColumn(
                name: "IsStriker",
                table: "PlayerScores");

            migrationBuilder.AlterColumn<int>(
                name: "Wickets",
                table: "TeamScores",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TotalScore",
                table: "TeamScores",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
