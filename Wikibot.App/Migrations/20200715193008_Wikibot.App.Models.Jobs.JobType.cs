using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Wikibot.App.Migrations
{
    public partial class WikibotAppModelsJobsJobType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Jobs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProposedChanges",
                table: "Jobs",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedDate",
                table: "Jobs",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "ProposedChanges",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "SubmittedDate",
                table: "Jobs");
        }
    }
}
