using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bma.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedExpensesRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "ProjectName",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Requests",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Attachment",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExpenseType",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "Attachment",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "ExpenseType",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "ProjectName",
                table: "Requests");
        }
    }
}
