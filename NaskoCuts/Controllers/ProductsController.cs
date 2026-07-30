using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaskoCuts.Data;
using NaskoCuts.Models;
using NaskoCuts.Models.Entities;

namespace NaskoCuts.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private const string CartSessionKey = "Cart";

        public ProductsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ProductCategory? category)
        {
            var query = _db.Products.Where(p => p.IsActive);

            if (category.HasValue)
                query = query.Where(p => p.Category == category.Value);

            var products = await query.OrderBy(p => p.Category).ThenBy(p => p.Name).ToListAsync();

            ViewBag.SelectedCategory = category;
            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == productId && p.IsActive);
            if (product == null)
                return NotFound();

            if (quantity < 1) quantity = 1;

            var cart = GetCart();
            var existing = cart.FirstOrDefault(c => c.ProductId == productId);

            if (existing != null)
                existing.Quantity += quantity;
            else
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    ImageUrl = product.ImageUrl,
                    Quantity = quantity
                });

            var capped = cart.First(c => c.ProductId == productId);
            if (capped.Quantity > product.Stock)
                capped.Quantity = product.Stock;

            SaveCart(cart);
            TempData["CartMessage"] = $"{product.Name} е добавен в количката.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Cart()
        {
            return View(GetCart());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCart(int productId, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                if (quantity <= 0)
                    cart.Remove(item);
                else
                    item.Quantity = quantity;
            }
            SaveCart(cart);
            return RedirectToAction(nameof(Cart));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = GetCart();
            cart.RemoveAll(c => c.ProductId == productId);
            SaveCart(cart);
            return RedirectToAction(nameof(Cart));
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = GetCart();
            if (!cart.Any())
                return RedirectToAction(nameof(Cart));

            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(string clientName, string clientEmail, string clientPhone, string? notes)
        {
            var cart = GetCart();
            if (!cart.Any())
                return RedirectToAction(nameof(Cart));

            if (string.IsNullOrWhiteSpace(clientName))
                ModelState.AddModelError("clientName", "Името е задължително.");

            if (string.IsNullOrWhiteSpace(clientEmail) || !clientEmail.Contains("@"))
                ModelState.AddModelError("clientEmail", "Въведи валиден имейл.");

            if (string.IsNullOrWhiteSpace(clientPhone))
                ModelState.AddModelError("clientPhone", "Телефонът е задължителен.");

            foreach (var item in cart)
            {
                var product = await _db.Products.FindAsync(item.ProductId);
                if (product == null || !product.IsActive || product.Stock < item.Quantity)
                    ModelState.AddModelError(string.Empty, $"'{item.ProductName}' вече не е наличен в това количество.");
            }

            if (!ModelState.IsValid)
                return View(cart);

            var order = new Order
            {
                ClientName = clientName,
                ClientEmail = clientEmail,
                ClientPhone = clientPhone,
                Notes = notes ?? string.Empty,
                TotalAmount = cart.Sum(c => c.Total),
                Status = OrderStatus.Pending,
                ConfirmationCode = $"NCP-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}",
                CreatedAt = DateTime.UtcNow
            };

            foreach (var item in cart)
            {
                order.Items.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });

                var product = await _db.Products.FindAsync(item.ProductId);
                if (product != null)
                    product.Stock -= item.Quantity;
            }

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            SaveCart(new List<CartItem>());

            TempData["OrderTotal"] = order.TotalAmount.ToString("0.00");
            TempData["OrderConfirmation"] = order.ConfirmationCode;
            TempData["OrderClientName"] = order.ClientName;

            return RedirectToAction(nameof(OrderConfirmed));
        }

        [HttpGet]
        public IActionResult OrderConfirmed()
        {
            if (TempData["OrderConfirmation"] == null)
                return RedirectToAction(nameof(Index));

            return View();
        }

        private List<CartItem> GetCart()
        {
            var json = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(json))
                return new List<CartItem>();

            return JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
        }
    }
}