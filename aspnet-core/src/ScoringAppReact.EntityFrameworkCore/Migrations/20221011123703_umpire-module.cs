using Microsoft.EntityFrameworkCore.Migrations;

namespace ScoringAppReact.Migrations
{
    public partial class umpiremodule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileUrl",
                table: "Umpires");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Umpires");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Umpires",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Contact",
                table: "Umpires",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Umpires",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Age",
                table: "Umpires",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "Umpires",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Umpires",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "Umpires");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "Umpires");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Umpires");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Umpires",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Contact",
                table: "Umpires",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Umpires",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileUrl",
                table: "Umpires",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Umpires",
                type: "int",
                nullable: true);
        }
    }
}
