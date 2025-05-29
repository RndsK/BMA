using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bma.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedTransferRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "RecurrenceType",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SupportingDocument",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransferFrom",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TransferRequest_Amount",
                table: "Requests",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransferRequest_Currency",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransferRequest_Purpose",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransferTo",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TransferRequestSignOffParticipants",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferRequestSignOffParticipants", x => new { x.UserId, x.RequestId });
                    table.ForeignKey(
                        name: "FK_TransferRequestSignOffParticipants_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransferRequestSignOffParticipants_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransferRequestSignOffParticipants_RequestId",
                table: "TransferRequestSignOffParticipants",
                column: "RequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable(
                name: "TransferRequestSignOffParticipants");

            migrationBuilder.DropColumn(
                name: "RecurrenceType",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "SupportingDocument",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "TransferFrom",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "TransferRequest_Amount",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "TransferRequest_Currency",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "TransferRequest_Purpose",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "TransferTo",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Requests");

        }
    }
}
