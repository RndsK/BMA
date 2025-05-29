using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bma.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ApprovalsAndAdditionalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TransferRequestSignOffParticipants",
                table: "TransferRequestSignOffParticipants");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "Requests");

            migrationBuilder.RenameColumn(
                name: "TransferRequest_Currency",
                table: "Requests",
                newName: "ExpensesRequest_Currency");

            migrationBuilder.RenameColumn(
                name: "TransferRequest_Amount",
                table: "Requests",
                newName: "ExpensesRequest_Amount");

            migrationBuilder.RenameColumn(
                name: "TransferRequest_Purpose",
                table: "Requests",
                newName: "SignOffRequest_SupportingDocument");

            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "Requests",
                newName: "SignOffRequest_SignOffParticipants");

            migrationBuilder.AddColumn<string>(
                name: "SignOffParticipants",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "TransferRequestSignOffParticipants",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "JoinRequests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Industry",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Logo",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerName",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransferRequestSignOffParticipants",
                table: "TransferRequestSignOffParticipants",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Approvals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestId = table.Column<int>(type: "int", nullable: true),
                    JoinRequestId = table.Column<int>(type: "int", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Approvals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Approvals_Requests_JoinRequestId",
                        column: x => x.JoinRequestId,
                        principalTable: "Requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Approvals_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RoleInCompany",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleInCompany", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleInCompany_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleInCompany_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransferRequestSignOffParticipants_UserId",
                table: "TransferRequestSignOffParticipants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_JoinRequestId",
                table: "Approvals",
                column: "JoinRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_RequestId",
                table: "Approvals",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleInCompany_CompanyId",
                table: "RoleInCompany",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleInCompany_UserId",
                table: "RoleInCompany",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SignOffParticipants",
                table: "Requests");

            migrationBuilder.DropTable(
                name: "Approvals");

            migrationBuilder.DropTable(
                name: "RoleInCompany");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransferRequestSignOffParticipants",
                table: "TransferRequestSignOffParticipants");

            migrationBuilder.DropIndex(
                name: "IX_TransferRequestSignOffParticipants_UserId",
                table: "TransferRequestSignOffParticipants");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "TransferRequestSignOffParticipants");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Industry",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Logo",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "OwnerName",
                table: "Companies");

            migrationBuilder.RenameColumn(
                name: "ExpensesRequest_Currency",
                table: "Requests",
                newName: "TransferRequest_Currency");

            migrationBuilder.RenameColumn(
                name: "ExpensesRequest_Amount",
                table: "Requests",
                newName: "TransferRequest_Amount");

            migrationBuilder.RenameColumn(
                name: "SignOffRequest_SupportingDocument",
                table: "Requests",
                newName: "TransferRequest_Purpose");

            migrationBuilder.RenameColumn(
                name: "SignOffRequest_SignOffParticipants",
                table: "Requests",
                newName: "Reason");

            migrationBuilder.RenameColumn(
                name: "SignOffParticipants",
                table: "Requests",
                newName: "Purpose");

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "JoinRequests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransferRequestSignOffParticipants",
                table: "TransferRequestSignOffParticipants",
                columns: new[] { "UserId", "RequestId" });
        }
    }
}
