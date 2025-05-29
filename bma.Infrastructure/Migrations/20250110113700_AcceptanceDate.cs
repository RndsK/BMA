
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bma.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AcceptanceDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_Requests_JoinRequestId",
                table: "Approvals");

            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_Requests_RequestId",
                table: "Approvals");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinRequests_AspNetUsers_UserId",
                table: "JoinRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinRequests_Companies_CompanyId",
                table: "JoinRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_AspNetUsers_UserId",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Companies_CompanyId",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleInCompany_Companies_CompanyId",
                table: "RoleInCompany");

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_Requests_JoinRequestId",
                table: "Approvals",
                column: "JoinRequestId",
                principalTable: "JoinRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_Requests_RequestId",
                table: "Approvals",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinRequests_AspNetUsers_UserId",
                table: "JoinRequests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinRequests_Companies_CompanyId",
                table: "JoinRequests",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_AspNetUsers_UserId",
                table: "Requests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Companies_CompanyId",
                table: "Requests",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleInCompany_Companies_CompanyId",
                table: "RoleInCompany",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");

            migrationBuilder.AddColumn<DateTime>(
                name: "AcceptanceDate",
                table: "JoinRequests",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
               name: "AcceptanceDate",
               table: "JoinRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_Requests_JoinRequestId",
                table: "Approvals");

            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_Requests_RequestId",
                table: "Approvals");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinRequests_AspNetUsers_UserId",
                table: "JoinRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_JoinRequests_Companies_CompanyId",
                table: "JoinRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_AspNetUsers_UserId",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Companies_CompanyId",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleInCompany_Companies_CompanyId",
                table: "RoleInCompany");

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_Requests_JoinRequestId",
                table: "Approvals",
                column: "JoinRequestId",
                principalTable: "Requests",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_Requests_RequestId",
                table: "Approvals",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinRequests_AspNetUsers_UserId",
                table: "JoinRequests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JoinRequests_Companies_CompanyId",
                table: "JoinRequests",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_AspNetUsers_UserId",
                table: "Requests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Companies_CompanyId",
                table: "Requests",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleInCompany_Companies_CompanyId",
                table: "RoleInCompany",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
