using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wevo_Pay_Project.Migrations
{
    /// <inheritdoc />
    public partial class AddUserReferral : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReferredByUserId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ReferredByUserId",
                table: "Users",
                column: "ReferredByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_ReferredByUserId",
                table: "Users",
                column: "ReferredByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_ReferredByUserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_ReferredByUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ReferredByUserId",
                table: "Users");
        }
    }
}
