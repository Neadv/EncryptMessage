using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EncryptMessage.Migrations
{
    public partial class AddedAuthorizationMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDisposable",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrivate",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "lockoutOnFailure",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "AllowedUserMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllowedUserMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AllowedUserMessages_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AllowedUserMessages_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "MessageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AllowedUserMessages_MessageId",
                table: "AllowedUserMessages",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_AllowedUserMessages_UserId",
                table: "AllowedUserMessages",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AllowedUserMessages");

            migrationBuilder.DropColumn(
                name: "IsDisposable",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "IsPrivate",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "lockoutOnFailure",
                table: "Messages");
        }
    }
}
