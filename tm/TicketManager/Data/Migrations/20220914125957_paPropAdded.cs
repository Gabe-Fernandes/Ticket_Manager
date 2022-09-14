using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketManager.Migrations
{
    public partial class paPropAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Approved",
                table: "Project_AppUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approved",
                table: "Project_AppUsers");
        }
    }
}
