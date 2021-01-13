using Microsoft.EntityFrameworkCore.Migrations;

namespace EduLocate.Server.Migrations
{
    public partial class SocialLinks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "FacebookLink",
                "Schools",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                "TwitterLink",
                "Schools",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "FacebookLink",
                "Schools");

            migrationBuilder.DropColumn(
                "TwitterLink",
                "Schools");
        }
    }
}