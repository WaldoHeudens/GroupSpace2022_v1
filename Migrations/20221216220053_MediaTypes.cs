using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroupSpace2022.Migrations
{
    public partial class MediaTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryMedia_Media_MediasId",
                table: "CategoryMedia");

            migrationBuilder.RenameColumn(
                name: "MediasId",
                table: "CategoryMedia",
                newName: "MediaId");

            migrationBuilder.RenameIndex(
                name: "IX_CategoryMedia_MediasId",
                table: "CategoryMedia",
                newName: "IX_CategoryMedia_MediaId");

            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                table: "Media",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MediaType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Denominator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Media_TypeId",
                table: "Media",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryMedia_Media_MediaId",
                table: "CategoryMedia",
                column: "MediaId",
                principalTable: "Media",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Media_MediaType_TypeId",
                table: "Media",
                column: "TypeId",
                principalTable: "MediaType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryMedia_Media_MediaId",
                table: "CategoryMedia");

            migrationBuilder.DropForeignKey(
                name: "FK_Media_MediaType_TypeId",
                table: "Media");

            migrationBuilder.DropTable(
                name: "MediaType");

            migrationBuilder.DropIndex(
                name: "IX_Media_TypeId",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Media");

            migrationBuilder.RenameColumn(
                name: "MediaId",
                table: "CategoryMedia",
                newName: "MediasId");

            migrationBuilder.RenameIndex(
                name: "IX_CategoryMedia_MediaId",
                table: "CategoryMedia",
                newName: "IX_CategoryMedia_MediasId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryMedia_Media_MediasId",
                table: "CategoryMedia",
                column: "MediasId",
                principalTable: "Media",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
