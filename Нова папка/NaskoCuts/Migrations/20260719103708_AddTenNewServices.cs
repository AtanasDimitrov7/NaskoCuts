using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaskoCuts.Migrations
{
    /// <inheritdoc />
    public partial class AddTenNewServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "Id", "Name", "Description", "DurationMinutes", "Price", "ImageUrl", "IsActive" },
                values: new object[,]
                {
                    { 7, "Buzz Cut", "Бърз, чист машинков подстриг.", 25, 29.34m, "https://www.barberstake.com/wp-content/uploads/2025/01/Buzz-Cuts-For-Men-1.jpg", true },
                    { 8, "Crew Cut", "Кратка, спретната класика с ниска поддръжка.", 20, 18m, "https://premiumbarbershop.com/wp-content/uploads/2024/05/crew-cut.jpg", true },
                    { 9, "Textured Crop", "Модерен текстуриран връх с изчистени страни.", 35, 28m, "https://i.pinimg.com/originals/98/6c/54/986c549669767bc1c37b52a2fd09d240.jpg?nii=t", true },
                    { 10, "Slick Back", "Гладко пригладена назад прическа.", 25, 22m, "https://content.latest-hairstyles.com/wp-content/uploads/mid-length-slick-back-for-men.jpg", true },
                    { 11, "Mohawk Fade", "Смела ивица по средата с fade отстрани.", 40, 30m, "https://i.pinimg.com/originals/32/1f/b6/321fb65f241caaef9059afb78a1ea582.png", true },
                    { 12, "Line Up (Edge Up)", "Прецизна линия на косата и брадата.", 10, 10m, "https://www.barberstake.com/wp-content/uploads/2025/12/Sponged-Afro-with-Temp-Fade-and-Edge-Up-@amanfour_barbers_palace.jpg", true },
                    { 13, "Comb Over Fade", "Прическа настрани с плътен fade.", 30, 25m, "https://menhairstylesworld.com/wp-content/uploads/2022/04/Slick-Comb-Over-with-Skin-Fade.jpg", true },
                    { 14, "Curly Shape Up", "Оформяне на естествено къдрава коса.", 30, 24m, "https://i.pinimg.com/originals/92/80/6b/92806bcc75d215fc636c3d99983eaf8c.jpg", true },
                    { 15, "Long Hair Trim", "Подрязване и оформяне на по-дълга коса.", 25, 20m, "https://vagazine.com/vaga_v3/wp-content/uploads/2025/02/mens-long-haircuts-mid-length-brow-flow.png", true },
                    { 16, "Hair Design", "Дизайн с линии, издълбани във фейда.", 15, 20m, "https://i.pinimg.com/originals/76/32/c1/7632c10481d0f1e0a13b341c4fe1b7dc.jpg", true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "Services", keyColumn: "Id", keyValues: new object[] { 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 });
        }
    }
}