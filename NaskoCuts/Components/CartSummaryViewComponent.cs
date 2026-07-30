using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using NaskoCuts.Models;

namespace NaskoCuts.Components
{
    public class CartSummaryViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var json = HttpContext.Session.GetString("Cart");
            var count = 0;

            if (!string.IsNullOrEmpty(json))
            {
                var cart = JsonSerializer.Deserialize<List<CartItem>>(json);
                count = cart?.Sum(c => c.Quantity) ?? 0;
            }

            return View(count);
        }
    }
}