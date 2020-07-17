using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Wikibot.App.Migrations
{
    public partial class WikibotAppModelsJobsJobContext3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TimeFinished",
                table: "Jobs",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "TimePreFinished",
                table: "Jobs",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "TimePreStarted",
                table: "Jobs",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeStarted",
                table: "Jobs",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeFinished",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "TimePreFinished",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "TimePreStarted",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "TimeStarted",
                table: "Jobs");
        }
    }
}
