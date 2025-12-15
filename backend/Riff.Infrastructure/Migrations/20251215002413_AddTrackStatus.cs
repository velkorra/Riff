using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Riff.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTrackStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PausedDurationInSeconds",
                table: "Tracks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "StartedAt",
                table: "Tracks",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Tracks",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PausedDurationInSeconds",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "StartedAt",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Tracks");
        }
    }
}
