using Microsoft.EntityFrameworkCore.Migrations;

namespace Wikibot.App.Migrations
{
    public partial class WikibotAppModelsJobsJobContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Comment = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    RequestType = table.Column<int>(nullable: false),
                    RawRequest = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Jobs");
        }
    }
}
