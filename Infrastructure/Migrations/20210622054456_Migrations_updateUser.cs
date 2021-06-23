using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class Migrations_updateUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HashedPassword",
                table: "Users",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Salt",
                table: "Users",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsersId",
                table: "Income",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsersId",
                table: "Expenditure",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Income_UsersId",
                table: "Income",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenditure_UsersId",
                table: "Expenditure",
                column: "UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenditure_Users_UsersId",
                table: "Expenditure",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Income_Users_UsersId",
                table: "Income",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenditure_Users_UsersId",
                table: "Expenditure");

            migrationBuilder.DropForeignKey(
                name: "FK_Income_Users_UsersId",
                table: "Income");

            migrationBuilder.DropIndex(
                name: "IX_Income_UsersId",
                table: "Income");

            migrationBuilder.DropIndex(
                name: "IX_Expenditure_UsersId",
                table: "Expenditure");

            migrationBuilder.DropColumn(
                name: "HashedPassword",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Salt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UsersId",
                table: "Income");

            migrationBuilder.DropColumn(
                name: "UsersId",
                table: "Expenditure");
        }
    }
}
