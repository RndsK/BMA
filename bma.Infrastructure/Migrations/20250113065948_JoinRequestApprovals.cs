using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bma.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class JoinRequestApprovals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_Requests_JoinRequestId",
                table: "Approvals");

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_JoinRequests_JoinRequestId",
                table: "Approvals",
                column: "JoinRequestId",
                principalTable: "JoinRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_JoinRequests_JoinRequestId",
                table: "Approvals");

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_Requests_JoinRequestId",
                table: "Approvals",
                column: "JoinRequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
