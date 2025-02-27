using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class ForeignerKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Student_AccountId",
                table: "Student",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Servant_AccountId",
                table: "Servant",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Room_ManagerId",
                table: "Room",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Manager_AccountId",
                table: "Manager",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Manager_Account_AccountId",
                table: "Manager",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Room_Manager_ManagerId",
                table: "Room",
                column: "ManagerId",
                principalTable: "Manager",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Servant_Account_AccountId",
                table: "Servant",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Student_Account_AccountId",
                table: "Student",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Manager_Account_AccountId",
                table: "Manager");

            migrationBuilder.DropForeignKey(
                name: "FK_Room_Manager_ManagerId",
                table: "Room");

            migrationBuilder.DropForeignKey(
                name: "FK_Servant_Account_AccountId",
                table: "Servant");

            migrationBuilder.DropForeignKey(
                name: "FK_Student_Account_AccountId",
                table: "Student");

            migrationBuilder.DropIndex(
                name: "IX_Student_AccountId",
                table: "Student");

            migrationBuilder.DropIndex(
                name: "IX_Servant_AccountId",
                table: "Servant");

            migrationBuilder.DropIndex(
                name: "IX_Room_ManagerId",
                table: "Room");

            migrationBuilder.DropIndex(
                name: "IX_Manager_AccountId",
                table: "Manager");
        }
    }
}
