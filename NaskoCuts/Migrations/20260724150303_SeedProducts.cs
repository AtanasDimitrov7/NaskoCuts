using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaskoCuts.Migrations
{
    /// <inheritdoc />
    public partial class SeedProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Name", "Description", "Price", "ImageUrl", "Category", "Stock", "IsActive" },
                values: new object[,]
                {
                    { 1, "Матиращ клей за коса", "Силна фиксация с матов финиш, за текстуриран стил без блясък.", 25m, "https://onemg.gumlet.io/l_watermark_346,w_690,h_700/a_ignore,w_690,h_700,c_pad,q_auto,f_auto/kw7bg3sbqkjqjqc19f2c.jpg", 0, 30, true },
                    { 2, "Помада за коса", "Класическа помада за гладък, блестящ и лесен за оформяне стил.", 22m, "https://cdn.shopify.com/s/files/1/0045/5104/9304/products/STYLING002_CREAMPOMADE85G-V2_2048x2048.jpg?v=1582754377", 0, 25, true },
                    { 3, "Шампоан за коса", "Дълбоко почистващ шампоан за ежедневна употреба.", 18m, "https://www.thegoldenspartan.com/wp-content/uploads/2022/10/143-S-Jan-23-R-min.jpg", 0, 40, true },
                    { 4, "Масло за брада", "Хидратиращо масло, омекотява и подхранва брадата.", 20m, "https://images-platform.99static.com/mEpH-T-GB7I2QbOpTdNnfSJ5Tz4=/198x259:1782x1843/600x600/99designs-contests-attachments/151/151702/attachment_151702762", 1, 35, true },
                    { 5, "Балсам за брада", "Оформя и подхранва брадата, лека фиксация през целия ден.", 24m, "https://www.blackbloxie.com/cdn/shop/files/il_2048x2048.5523845056_dlkq.jpg?v=1773690005", 1, 20, true },
                    { 6, "Гребен за брада", "Дървен гребен за ежедневно оформяне на брадата.", 12m, "https://bearded-only.com/cdn/shop/products/DSCF2445_1024x1024@2x.jpg?v=1667230651", 1, 50, true },
                    { 7, "Афтършейв балсам", "Успокоява кожата след бръснене, без парене.", 19m, "https://shavenation.com/cdn/shop/products/new-proraso-aftershave-balm-sensitive-skin-white-green-tea-oatmeal-shave-nation.jpg?v=1608234777", 2, 28, true },
                    { 8, "Крем за лице", "Ежедневна хидратация за мъжка кожа.", 21m, "https://i.makeup.jp/g/gd/gdknwlvagnqh.jpg", 2, 22, true },
                    { 9, "Професионална ножица за подстригване", "Прецизна ножица от неръждаема стомана за професионална употреба.", 45m, "https://libertysupply.store/cdn/shop/products/il_fullxfull.1714944488_dg4m_1024x1024@2x.jpg?v=1576019550", 3, 10, true },
                    { 10, "Професионална машинка за подстригване", "Мощна машинка с прецизни ножове за чист рез.", 85m, "https://www.vgrpakistan.pk/cdn/shop/files/1_1c5db535-b3a2-4a47-83f1-af726b19f9e8.jpg?v=1704129301", 3, 8, true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "Products", keyColumn: "Id", keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
        }
    }
}