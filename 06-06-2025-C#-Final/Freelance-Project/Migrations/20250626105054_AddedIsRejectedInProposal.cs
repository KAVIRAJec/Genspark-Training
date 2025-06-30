using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Freelance_Project.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsRejectedInProposal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRejected",
                table: "Proposals",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRejected",
                table: "Proposals");
        }
    }
}
