using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketManager.Migrations
{
    public partial class messagePropertiesChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Messages",
                newName: "SenderName");

            migrationBuilder.RenameColumn(
                name: "FromProfilePicture",
                table: "Messages",
                newName: "ReceiverName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SenderName",
                table: "Messages",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ReceiverName",
                table: "Messages",
                newName: "FromProfilePicture");
        }
    }
}
