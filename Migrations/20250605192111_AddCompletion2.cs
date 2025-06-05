using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasinoDeYann.Migrations
{
    /// <inheritdoc />
    public partial class AddCompletion2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCanceled",
                table: "Stats",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCanceled",
                table: "Stats");
        }
    }
}
