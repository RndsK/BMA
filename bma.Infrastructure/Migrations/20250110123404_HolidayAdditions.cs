using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bma.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HolidayAdditions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Requests",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false);

            migrationBuilder.AddColumn<DateOnly>(
                name: "EndDate",
                table: "Requests",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "HolidayRequest_StartDate",
                table: "Requests",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "HolidayRequest_StartDate",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Requests");
        }
    }
}
