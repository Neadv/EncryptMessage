using Microsoft.EntityFrameworkCore.Migrations;

namespace EncryptMessage.Migrations
{
    public partial class FixMessageModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "lockoutOnFailure",
                table: "Messages",
                newName: "LockoutOnFailure");

            migrationBuilder.AddColumn<bool>(
                name: "IsLockout",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLockout",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "LockoutOnFailure",
                table: "Messages",
                newName: "lockoutOnFailure");
        }
    }
}
