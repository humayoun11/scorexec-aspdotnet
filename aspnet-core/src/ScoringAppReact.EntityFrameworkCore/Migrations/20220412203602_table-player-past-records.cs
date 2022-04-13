using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ScoringAppReact.Migrations
{
    public partial class tableplayerpastrecords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerPastRecords",
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
                    TotalMatch = table.Column<int>(nullable: true),
                    TotalInnings = table.Column<int>(nullable: true),
                    TotalNotOut = table.Column<int>(nullable: true),
                    GetBowled = table.Column<int>(nullable: true),
                    GetHitWicket = table.Column<int>(nullable: true),
                    GetLBW = table.Column<int>(nullable: true),
                    GetCatch = table.Column<int>(nullable: true),
                    GetStump = table.Column<int>(nullable: true),
                    GetRunOut = table.Column<int>(nullable: true),
                    TotalBatRuns = table.Column<int>(nullable: true),
                    TotalBatBalls = table.Column<int>(nullable: true),
                    TotalFours = table.Column<int>(nullable: true),
                    TotalSixes = table.Column<int>(nullable: true),
                    NumberOf50s = table.Column<int>(nullable: true),
                    NumberOf100s = table.Column<int>(nullable: true),
                    BestScore = table.Column<int>(nullable: true),
                    TotalOvers = table.Column<float>(nullable: true),
                    TotalBallRuns = table.Column<int>(nullable: true),
                    TotalWickets = table.Column<int>(nullable: true),
                    TotalMaidens = table.Column<int>(nullable: true),
                    FiveWickets = table.Column<int>(nullable: true),
                    DoBowled = table.Column<int>(nullable: true),
                    DoHitWicket = table.Column<int>(nullable: true),
                    DoLBW = table.Column<int>(nullable: true),
                    DoCatch = table.Column<int>(nullable: true),
                    DoStump = table.Column<int>(nullable: true),
                    OnFieldCatch = table.Column<int>(nullable: true),
                    OnFieldStump = table.Column<int>(nullable: true),
                    OnFieldRunOut = table.Column<int>(nullable: true),
                    PlayerId1 = table.Column<long>(nullable: true),
                    PlayerId = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerPastRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerPastRecords_Players_PlayerId1",
                        column: x => x.PlayerId1,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPastRecords_PlayerId1",
                table: "PlayerPastRecords",
                column: "PlayerId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerPastRecords");
        }
    }
}
