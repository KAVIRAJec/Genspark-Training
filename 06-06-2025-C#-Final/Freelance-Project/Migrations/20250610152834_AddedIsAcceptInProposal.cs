using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Freelance_Project.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsAcceptInProposal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAccepted",
                table: "Proposals",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAccepted",
                table: "Proposals");
        }
    }
}
