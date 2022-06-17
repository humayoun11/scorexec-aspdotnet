using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ScoringAppReact.Migrations
{
    public partial class galleries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntityGalleries");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Galleries");

            migrationBuilder.AddColumn<long>(
                name: "EventId",
                table: "Galleries",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "GroundId",
                table: "Galleries",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MatchId",
                table: "Galleries",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PlayerId",
                table: "Galleries",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TeamId",
                table: "Galleries",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Galleries_EventId",
                table: "Galleries",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Galleries_GroundId",
                table: "Galleries",
                column: "GroundId");

            migrationBuilder.CreateIndex(
                name: "IX_Galleries_MatchId",
                table: "Galleries",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Galleries_PlayerId",
                table: "Galleries",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Galleries_TeamId",
                table: "Galleries",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Galleries_Events_EventId",
                table: "Galleries",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Galleries_Grounds_GroundId",
                table: "Galleries",
                column: "GroundId",
                principalTable: "Grounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Galleries_Matches_MatchId",
                table: "Galleries",
                column: "MatchId",
                principalTable: "Matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Galleries_Players_PlayerId",
                table: "Galleries",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Galleries_Teams_TeamId",
                table: "Galleries",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Galleries_Events_EventId",
                table: "Galleries");

            migrationBuilder.DropForeignKey(
                name: "FK_Galleries_Grounds_GroundId",
                table: "Galleries");

            migrationBuilder.DropForeignKey(
                name: "FK_Galleries_Matches_MatchId",
                table: "Galleries");

            migrationBuilder.DropForeignKey(
                name: "FK_Galleries_Players_PlayerId",
                table: "Galleries");

            migrationBuilder.DropForeignKey(
                name: "FK_Galleries_Teams_TeamId",
                table: "Galleries");

            migrationBuilder.DropIndex(
                name: "IX_Galleries_EventId",
                table: "Galleries");

            migrationBuilder.DropIndex(
                name: "IX_Galleries_GroundId",
                table: "Galleries");

            migrationBuilder.DropIndex(
                name: "IX_Galleries_MatchId",
                table: "Galleries");

            migrationBuilder.DropIndex(
                name: "IX_Galleries_PlayerId",
                table: "Galleries");

            migrationBuilder.DropIndex(
                name: "IX_Galleries_TeamId",
                table: "Galleries");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Galleries");

            migrationBuilder.DropColumn(
                name: "GroundId",
                table: "Galleries");

            migrationBuilder.DropColumn(
                name: "MatchId",
                table: "Galleries");

            migrationBuilder.DropColumn(
                name: "PlayerId",
                table: "Galleries");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Galleries");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Galleries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EntityGalleries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EventId = table.Column<long>(type: "bigint", nullable: true),
                    GalleryId = table.Column<long>(type: "bigint", nullable: false),
                    GroundId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    MatchId = table.Column<long>(type: "bigint", nullable: true),
                    PlayerId = table.Column<long>(type: "bigint", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityGalleries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityGalleries_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntityGalleries_Galleries_GalleryId",
                        column: x => x.GalleryId,
                        principalTable: "Galleries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityGalleries_Grounds_GroundId",
                        column: x => x.GroundId,
                        principalTable: "Grounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntityGalleries_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntityGalleries_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntityGalleries_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntityGalleries_EventId",
                table: "EntityGalleries",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityGalleries_GalleryId",
                table: "EntityGalleries",
                column: "GalleryId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityGalleries_GroundId",
                table: "EntityGalleries",
                column: "GroundId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityGalleries_MatchId",
                table: "EntityGalleries",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityGalleries_PlayerId",
                table: "EntityGalleries",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityGalleries_TeamId",
                table: "EntityGalleries",
                column: "TeamId");
        }
    }
}
