using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Freelance_Project.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserInRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Users_UserEmail",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_UserEmail",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "RefreshTokens");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "RefreshTokens",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserEmail",
                table: "RefreshTokens",
                column: "UserEmail");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Users_UserEmail",
                table: "RefreshTokens",
                column: "UserEmail",
                principalTable: "Users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
