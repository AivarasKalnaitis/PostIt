using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostIt.API.Migrations
{
    /// <inheritdoc />
    public partial class Logswithexception : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Exception",
                table: "Logs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Exception",
                table: "Logs");
        }
    }
}
