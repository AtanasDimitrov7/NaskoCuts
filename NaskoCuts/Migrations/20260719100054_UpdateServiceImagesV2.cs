using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaskoCuts.Migrations
{
    /// <inheritdoc />
    public partial class UpdateServiceImagesV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "https://www.menshairstylestoday.com/wp-content/uploads/2025/03/Best-Hair-Clippers-728x728.jpg");

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "https://content.latest-hairstyles.com/wp-content/uploads/two-block-haircut-with-an-undercut-fade-for-guys.jpg");

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: "https://glaminati.com/wp-content/uploads/2023/07/taper-fade-haircut-side-part-high-683x1024.jpg");

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImageUrl",
                value: "https://theortizbarbershop.com/wp-content/uploads/2023/06/young-man-getting-his-beard-styled-by-a-barber-asking-if-do-barbers-trim-beards.jpg");

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImageUrl",
                value: "https://conwayvillagebarbershop.com/wp-content/uploads/2026/01/shave68.jpg");

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 6,
                column: "ImageUrl",
                value: "https://i.pinimg.com/originals/fa/1d/14/fa1d14d24d8be252233553be0925cfc3.png");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Не е нужно да връщаме старите снимки назад — оставяме празно.
        }
    }
}