using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketManager.Migrations
{
    public partial class _1toManyUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Tickets_TicketId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_TicketId",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Tickets",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "AssignedTo",
                table: "Tickets",
                newName: "SenderName");

            migrationBuilder.RenameColumn(
                name: "AssignedFrom",
                table: "Tickets",
                newName: "SenderId");

            migrationBuilder.RenameColumn(
                name: "To",
                table: "Messages",
                newName: "SenderId");

            migrationBuilder.RenameColumn(
                name: "ReceiverName",
                table: "Messages",
                newName: "RecipientName");

            migrationBuilder.RenameColumn(
                name: "From",
                table: "Messages",
                newName: "RecipientId");

            migrationBuilder.AddColumn<string>(
                name: "RecipientId",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientName",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "Messages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecipientId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "RecipientName",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Tickets",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "SenderName",
                table: "Tickets",
                newName: "AssignedTo");

            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "Tickets",
                newName: "AssignedFrom");

            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "Messages",
                newName: "To");

            migrationBuilder.RenameColumn(
                name: "RecipientName",
                table: "Messages",
                newName: "ReceiverName");

            migrationBuilder.RenameColumn(
                name: "RecipientId",
                table: "Messages",
                newName: "From");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_TicketId",
                table: "Comments",
                column: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Tickets_TicketId",
                table: "Comments",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
