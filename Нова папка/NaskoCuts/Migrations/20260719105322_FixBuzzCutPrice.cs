using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaskoCuts.Migrations
{
    /// <inheritdoc />
    public partial class FixBuzzCutPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(table: "Services", keyColumn: "Id", keyValue: 7, column: "Price", value: 30m); // Buzz Cut
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(table: "Services", keyColumn: "Id", keyValue: 7, column: "Price", value: 29.34m);
        }
    }
}