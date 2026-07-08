using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wevo_Pay_Project.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemSettingsAndFees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransferRequests_Users_UserId",
                table: "TransferRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferRequests_companyWallets_CompanyWalletId",
                table: "TransferRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_companyWallets",
                table: "companyWallets");

            migrationBuilder.RenameTable(
                name: "companyWallets",
                newName: "CompanyWallets");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "TransferRequests",
                newName: "TransferAmount");

            migrationBuilder.AddColumn<int>(
                name: "CompletedByAdminId",
                table: "TransferRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Fee",
                table: "TransferRequests",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FeePercentage",
                table: "TransferRequests",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "RejectedByAdminId",
                table: "TransferRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "TransferRequests",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "VerifiedByAdminId",
                table: "TransferRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompanyWallets",
                table: "CompanyWallets",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeePercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    MinTransferAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxTransferAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransferRequests_CompletedByAdminId",
                table: "TransferRequests",
                column: "CompletedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRequests_RejectedByAdminId",
                table: "TransferRequests",
                column: "RejectedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRequests_VerifiedByAdminId",
                table: "TransferRequests",
                column: "VerifiedByAdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_TransferRequests_CompanyWallets_CompanyWalletId",
                table: "TransferRequests",
                column: "CompanyWalletId",
                principalTable: "CompanyWallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TransferRequests_Users_CompletedByAdminId",
                table: "TransferRequests",
                column: "CompletedByAdminId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransferRequests_Users_RejectedByAdminId",
                table: "TransferRequests",
                column: "RejectedByAdminId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransferRequests_Users_UserId",
                table: "TransferRequests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransferRequests_Users_VerifiedByAdminId",
                table: "TransferRequests",
                column: "VerifiedByAdminId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransferRequests_CompanyWallets_CompanyWalletId",
                table: "TransferRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferRequests_Users_CompletedByAdminId",
                table: "TransferRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferRequests_Users_RejectedByAdminId",
                table: "TransferRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferRequests_Users_UserId",
                table: "TransferRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferRequests_Users_VerifiedByAdminId",
                table: "TransferRequests");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropIndex(
                name: "IX_TransferRequests_CompletedByAdminId",
                table: "TransferRequests");

            migrationBuilder.DropIndex(
                name: "IX_TransferRequests_RejectedByAdminId",
                table: "TransferRequests");

            migrationBuilder.DropIndex(
                name: "IX_TransferRequests_VerifiedByAdminId",
                table: "TransferRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompanyWallets",
                table: "CompanyWallets");

            migrationBuilder.DropColumn(
                name: "CompletedByAdminId",
                table: "TransferRequests");

            migrationBuilder.DropColumn(
                name: "Fee",
                table: "TransferRequests");

            migrationBuilder.DropColumn(
                name: "FeePercentage",
                table: "TransferRequests");

            migrationBuilder.DropColumn(
                name: "RejectedByAdminId",
                table: "TransferRequests");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "TransferRequests");

            migrationBuilder.DropColumn(
                name: "VerifiedByAdminId",
                table: "TransferRequests");

            migrationBuilder.RenameTable(
                name: "CompanyWallets",
                newName: "companyWallets");

            migrationBuilder.RenameColumn(
                name: "TransferAmount",
                table: "TransferRequests",
                newName: "Amount");

            migrationBuilder.AddPrimaryKey(
                name: "PK_companyWallets",
                table: "companyWallets",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransferRequests_Users_UserId",
                table: "TransferRequests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TransferRequests_companyWallets_CompanyWalletId",
                table: "TransferRequests",
                column: "CompanyWalletId",
                principalTable: "companyWallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
