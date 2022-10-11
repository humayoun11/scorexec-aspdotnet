using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ScoringAppReact.Migrations
{
    public partial class umpiremodule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UmpireId",
                table: "Galleries",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Umpires",
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
                    Name = table.Column<string>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    Contact = table.Column<string>(nullable: true),
                    Age = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Umpires", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Galleries_UmpireId",
                table: "Galleries",
                column: "UmpireId");

            migrationBuilder.AddForeignKey(
                name: "FK_Galleries_Umpires_UmpireId",
                table: "Galleries",
                column: "UmpireId",
                principalTable: "Umpires",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Galleries_Umpires_UmpireId",
                table: "Galleries");

            migrationBuilder.DropTable(
                name: "Umpires");

            migrationBuilder.DropIndex(
                name: "IX_Galleries_UmpireId",
                table: "Galleries");

            migrationBuilder.DropColumn(
                name: "UmpireId",
                table: "Galleries");
        }
    }
}
