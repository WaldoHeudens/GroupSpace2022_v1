using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroupSpace2022.Migrations
{
    public partial class messagesUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_Group_GroupId",
                table: "Message");

            migrationBuilder.DropIndex(
                name: "IX_Message_GroupId",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Message");

            migrationBuilder.RenameColumn(
                name: "Sent",
                table: "Message",
                newName: "Created");

            migrationBuilder.AddColumn<int>(
                name: "MessageId",
                table: "Media",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MessageDestinations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageId = table.Column<int>(type: "int", nullable: false),
                    ReceiverId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Received = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Read = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageDestinations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageDestinations_AspNetUsers_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageDestinations_Message_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Message",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Media_MessageId",
                table: "Media",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageDestinations_MessageId",
                table: "MessageDestinations",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageDestinations_ReceiverId",
                table: "MessageDestinations",
                column: "ReceiverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Media_Message_MessageId",
                table: "Media",
                column: "MessageId",
                principalTable: "Message",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Media_Message_MessageId",
                table: "Media");

            migrationBuilder.DropTable(
                name: "MessageDestinations");

            migrationBuilder.DropIndex(
                name: "IX_Media_MessageId",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "MessageId",
                table: "Media");

            migrationBuilder.RenameColumn(
                name: "Created",
                table: "Message",
                newName: "Sent");

            migrationBuilder.AddColumn<DateTime>(
                name: "Deleted",
                table: "Message",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "Message",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Message_GroupId",
                table: "Message",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Group_GroupId",
                table: "Message",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
