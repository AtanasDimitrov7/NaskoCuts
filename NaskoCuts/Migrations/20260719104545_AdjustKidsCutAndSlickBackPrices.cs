using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaskoCuts.Migrations
{
    /// <inheritdoc />
    public partial class AdjustKidsCutAndSlickBackPrices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(table: "Services", keyColumn: "Id", keyValue: 6, column: "Price", value: 15m);  // Kids Cut
            migrationBuilder.UpdateData(table: "Services", keyColumn: "Id", keyValue: 10, column: "Price", value: 25m); // Slick Back
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(table: "Services", keyColumn: "Id", keyValue: 6, column: "Price", value: 10m);
            migrationBuilder.UpdateData(table: "Services", keyColumn: "Id", keyValue: 10, column: "Price", value: 20m);
        }
    }
}