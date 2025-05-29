using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bma.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OvertimeRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "StartDate",
                table: "Requests",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Length",
                table: "Requests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "Length",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "Requests");
        }
    }
}
