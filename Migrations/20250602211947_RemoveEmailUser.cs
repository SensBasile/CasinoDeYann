using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasinoDeYann.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEmailUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "xp",
                table: "Users",
                newName: "Xp");

            migrationBuilder.RenameColumn(
                name: "money",
                table: "Users",
                newName: "Money");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Xp",
                table: "Users",
                newName: "xp");

            migrationBuilder.RenameColumn(
                name: "Money",
                table: "Users",
                newName: "money");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
