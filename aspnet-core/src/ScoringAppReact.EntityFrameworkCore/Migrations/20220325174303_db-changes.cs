using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ScoringAppReact.Migrations
{
    public partial class dbchanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Teams_TeamId",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerScores_Teams_TeamId1",
                table: "PlayerScores");

            migrationBuilder.DropIndex(
                name: "IX_PlayerScores_TeamId1",
                table: "PlayerScores");

            migrationBuilder.DropIndex(
                name: "IX_Players_TeamId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "TeamId1",
                table: "PlayerScores");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "HomeTeamOvers",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "OppTeamOvers",
                table: "Matches");

            migrationBuilder.AddColumn<int>(
                name: "Overs",
                table: "TeamScores",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Teams",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<long>(
                name: "TeamId",
                table: "PlayerScores",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "TeamPlayers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    CreatingTime = table.Column<long>(nullable: true),
                    UpdatingTime = table.Column<long>(nullable: true),
                    DeletingTime = table.Column<long>(nullable: true),
                    TeamId = table.Column<long>(nullable: false),
                    PlayerId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamPlayers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamPlayers_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamPlayers_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerScores_TeamId",
                table: "PlayerScores",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamPlayers_PlayerId",
                table: "TeamPlayers",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamPlayers_TeamId",
                table: "TeamPlayers",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerScores_Teams_TeamId",
                table: "PlayerScores",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerScores_Teams_TeamId",
                table: "PlayerScores");

            migrationBuilder.DropTable(
                name: "TeamPlayers");

            migrationBuilder.DropIndex(
                name: "IX_PlayerScores_TeamId",
                table: "PlayerScores");

            migrationBuilder.DropColumn(
                name: "Overs",
                table: "TeamScores");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Teams");

            migrationBuilder.AlterColumn<int>(
                name: "TeamId",
                table: "PlayerScores",
                type: "int",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<long>(
                name: "TeamId1",
                table: "PlayerScores",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TeamId",
                table: "Players",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<float>(
                name: "HomeTeamOvers",
                table: "Matches",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "OppTeamOvers",
                table: "Matches",
                type: "real",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerScores_TeamId1",
                table: "PlayerScores",
                column: "TeamId1");

            migrationBuilder.CreateIndex(
                name: "IX_Players_TeamId",
                table: "Players",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Teams_TeamId",
                table: "Players",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerScores_Teams_TeamId1",
                table: "PlayerScores",
                column: "TeamId1",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
