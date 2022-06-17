using Microsoft.EntityFrameworkCore.Migrations;

namespace ScoringAppReact.Migrations
{
    public partial class galleryandprofileUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "EntityGalleries");

            migrationBuilder.AddColumn<string>(
                name: "ProfileUrl",
                table: "Teams",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileUrl",
                table: "Players",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileUrl",
                table: "Matches",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileUrl",
                table: "Grounds",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileUrl",
                table: "Events",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "EventId",
                table: "EntityGalleries",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "GroundId",
                table: "EntityGalleries",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MatchId",
                table: "EntityGalleries",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PlayerId",
                table: "EntityGalleries",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TeamId",
                table: "EntityGalleries",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntityGalleries_EventId",
                table: "EntityGalleries",
                column: "EventId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_EntityGalleries_Events_EventId",
                table: "EntityGalleries",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityGalleries_Grounds_GroundId",
                table: "EntityGalleries",
                column: "GroundId",
                principalTable: "Grounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityGalleries_Matches_MatchId",
                table: "EntityGalleries",
                column: "MatchId",
                principalTable: "Matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityGalleries_Players_PlayerId",
                table: "EntityGalleries",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityGalleries_Teams_TeamId",
                table: "EntityGalleries",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntityGalleries_Events_EventId",
                table: "EntityGalleries");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityGalleries_Grounds_GroundId",
                table: "EntityGalleries");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityGalleries_Matches_MatchId",
                table: "EntityGalleries");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityGalleries_Players_PlayerId",
                table: "EntityGalleries");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityGalleries_Teams_TeamId",
                table: "EntityGalleries");

            migrationBuilder.DropIndex(
                name: "IX_EntityGalleries_EventId",
                table: "EntityGalleries");

            migrationBuilder.DropIndex(
                name: "IX_EntityGalleries_GroundId",
                table: "EntityGalleries");

            migrationBuilder.DropIndex(
                name: "IX_EntityGalleries_MatchId",
                table: "EntityGalleries");

            migrationBuilder.DropIndex(
                name: "IX_EntityGalleries_PlayerId",
                table: "EntityGalleries");

            migrationBuilder.DropIndex(
                name: "IX_EntityGalleries_TeamId",
                table: "EntityGalleries");

            migrationBuilder.DropColumn(
                name: "ProfileUrl",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "ProfileUrl",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "ProfileUrl",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "ProfileUrl",
                table: "Grounds");

            migrationBuilder.DropColumn(
                name: "ProfileUrl",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "EntityGalleries");

            migrationBuilder.DropColumn(
                name: "GroundId",
                table: "EntityGalleries");

            migrationBuilder.DropColumn(
                name: "MatchId",
                table: "EntityGalleries");

            migrationBuilder.DropColumn(
                name: "PlayerId",
                table: "EntityGalleries");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "EntityGalleries");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Players",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Matches",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "EntityId",
                table: "EntityGalleries",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
