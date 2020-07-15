using Microsoft.EntityFrameworkCore.Migrations;

namespace Wikibot.App.Migrations
{
    public partial class WikibotAppModelsJobsJobContext2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FromText",
                table: "Jobs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToText",
                table: "Jobs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Jobs",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Page",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    TextReplacementJobID = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Page", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Page_Jobs_TextReplacementJobID",
                        column: x => x.TextReplacementJobID,
                        principalTable: "Jobs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Page_TextReplacementJobID",
                table: "Page",
                column: "TextReplacementJobID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Page");

            migrationBuilder.DropColumn(
                name: "FromText",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "ToText",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Jobs");
        }
    }
}
