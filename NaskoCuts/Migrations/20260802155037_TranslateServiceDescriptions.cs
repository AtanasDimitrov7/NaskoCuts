using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaskoCuts.Migrations
{
    /// <inheritdoc />
    public partial class TranslateServiceDescriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(table: "Services", keyColumn: "Id", keyValue: 7, column: "Description", value: "Fast, clean machine buzz cut.");
            migrationBuilder.UpdateData(table: "Services", keyColumn: "Id", keyValue: 8, column: "Description", value: "Short, neat classic with low maintenance.");
            migrationBuilder.UpdateData(table: "Services", keyColumn: "Id", keyValue: 9, column: "Description", value: "Modern textured top with clean sides.");
            migrationBuilder.UpdateData(table: "Services", keyColumn: "Id", keyValue: 10, column: "Description", value: "Smooth, slicked-back hairstyle.");
            migrationBuilder.UpdateData(table: "Services", keyColumn: "Id", keyValue: 11, column: "Description", value: "Bold center strip with a fade on the sides.");
            migrationBuilder.UpdateData(table: "Services", keyColumn: "Id", keyValue: 12, column: "Description", value: "Precise hairline and beard edge-up.");
            migrationBuilder.UpdateData(table: "Services", keyColumn: "Id", keyValue: 13, column: "Description", value: "Side-swept style with a sharp fade.");
            migrationBuilder.UpdateData(table: "Services", keyColumn: "Id", keyValue: 14, column: "Description", value: "Shaping for naturally curly hair.");
            migrationBuilder.UpdateData(table: "Services", keyColumn: "Id", keyValue: 15, column: "Description", value: "Trim and styling for longer hair.");
            migrationBuilder.UpdateData(table: "Services", keyColumn: "Id", keyValue: 16, column: "Description", value: "Line design carved into the fade.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(table: "Services", keyColumn: "Id", keyValue: 7, column: "Description", value: "Бърз, чист машинков подстриг.");
            migrationBuilder.UpdateData(table: "Services", keyColumn: "Id", keyValue: 8, column: "Description", value: "Кратка, спретната класика с ниска поддръжка.");
            migrationBuilder.UpdateData(table: "Services", keyColumn: "Id", keyValue: 9, column: "Description", value: "Модерен текстуриран връх с изчистени страни.");
            migrationBuilder.UpdateData(table: "Services", keyColumn: "Id", keyValue: 10, column: "Description", value: "Гладко пригладена назад прическа.");
            migrationBuilder.UpdateData(table: "Services", keyColumn: "Id", keyValue: 11, column: "Description", value: "Смела ивица по средата с fade отстрани.");
            migrationBuilder.UpdateData(table: "Services", keyColumn: "Id", keyValue: 12, column: "Description", value: "Прецизна линия на косата и брадата.");
            migrationBuilder.UpdateData(table: "Services", keyColumn: "Id", keyValue: 13, column: "Description", value: "Прическа настрани с плътен fade.");
            migrationBuilder.UpdateData(table: "Services", keyColumn: "Id", keyValue: 14, column: "Description", value: "Оформяне на естествено къдрава коса.");
            migrationBuilder.UpdateData(table: "Services", keyColumn: "Id", keyValue: 15, column: "Description", value: "Подрязване и оформяне на по-дълга коса.");
            migrationBuilder.UpdateData(table: "Services", keyColumn: "Id", keyValue: 16, column: "Description", value: "Дизайн с линии, издълбани във фейда.");
        }
    }
}