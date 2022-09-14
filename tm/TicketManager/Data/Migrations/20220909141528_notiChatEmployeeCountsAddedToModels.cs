using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketManager.Migrations
{
    public partial class notiChatEmployeeCountsAddedToModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeamMemberCount",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MessageCount",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NotificationCount",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeamMemberCount",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "MessageCount",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NotificationCount",
                table: "AspNetUsers");
        }
    }
}
