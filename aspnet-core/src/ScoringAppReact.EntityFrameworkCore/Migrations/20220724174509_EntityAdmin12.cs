using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ScoringAppReact.Migrations
{
    public partial class EntityAdmin12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EntityAdmin",
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
                    UserId = table.Column<long>(nullable: false),
                    MatchId = table.Column<long>(nullable: true),
                    TeamId = table.Column<long>(nullable: true),
                    EventId = table.Column<long>(nullable: true),
                    GroundId = table.Column<long>(nullable: true),
                    PlayerId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityAdmin", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityAdmin_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntityAdmin_Grounds_GroundId",
                        column: x => x.GroundId,
                        principalTable: "Grounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntityAdmin_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntityAdmin_Players_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntityAdmin_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntityAdmin_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntityAdmin_EventId",
                table: "EntityAdmin",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityAdmin_GroundId",
                table: "EntityAdmin",
                column: "GroundId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityAdmin_MatchId",
                table: "EntityAdmin",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityAdmin_TeamId",
                table: "EntityAdmin",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityAdmin_UserId",
                table: "EntityAdmin",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntityAdmin");
        }
    }
}
