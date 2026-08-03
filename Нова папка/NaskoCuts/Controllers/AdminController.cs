using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaskoCuts.Data;
using NaskoCuts.Models;
using NaskoCuts.Models.Entities;
using System.Text;

namespace NaskoCuts.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly string _adminPassword;

        public AdminController(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            _adminPassword = config["AdminSettings:AdminPanelPassword"] ?? throw new InvalidOperationException("Admin panel password not configured.");
        }

        private bool IsAdmin => HttpContext.Session.GetString("IsAdmin") == "true";

        [HttpGet]
        public IActionResult Login()
        {
            if (IsAdmin) return RedirectToAction(nameof(Index));
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string password)
        {
            if (password == _adminPassword)
            {
                HttpContext.Session.SetString("IsAdmin", "true");
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Error = "Грешна парола.";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin) return RedirectToAction("Login", "Account");

            var appointments = await _db.Appointments
                .Include(a => a.Service)
                .Include(a => a.Barber)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return View(appointments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin) return RedirectToAction(nameof(Login));

            var appointment = await _db.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _db.Appointments.Remove(appointment);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // ── Услуги ──

        [HttpGet]
        public async Task<IActionResult> ManageServices()
        {
            if (!IsAdmin) return RedirectToAction(nameof(Login));

            var services = await _db.Services.OrderBy(s => s.Id).ToListAsync();
            return View(services);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateService(string name, string description, decimal price, int durationMinutes, string? imageUrl)
        {
            if (!IsAdmin) return RedirectToAction(nameof(Login));

            if (string.IsNullOrWhiteSpace(name))
            {
                ViewBag.Error = "Името е задължително.";
                return RedirectToAction(nameof(ManageServices));
            }

            var service = new Service
            {
                Name = name,
                Description = description ?? string.Empty,
                Price = price,
                DurationMinutes = durationMinutes,
                ImageUrl = imageUrl ?? string.Empty,
                IsActive = true
            };

            _db.Services.Add(service);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(ManageServices));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteService(int id)
        {
            if (!IsAdmin) return RedirectToAction(nameof(Login));

            var service = await _db.Services.FindAsync(id);
            if (service != null)
            {
                _db.Services.Remove(service);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ManageServices));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleService(int id)
        {
            if (!IsAdmin) return RedirectToAction(nameof(Login));

            var service = await _db.Services.FindAsync(id);
            if (service != null)
            {
                service.IsActive = !service.IsActive;
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ManageServices));
        }

        // ── Бръснари ──

        [HttpGet]
        public async Task<IActionResult> ManageBarbers()
        {
            if (!IsAdmin) return RedirectToAction(nameof(Login));

            var barbers = await _db.Barbers.OrderBy(b => b.Id).ToListAsync();
            return View(barbers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBarber(string fullName, string? role, string? bio, string? imageUrl, string? instagramUrl)
        {
            if (!IsAdmin) return RedirectToAction(nameof(Login));

            if (string.IsNullOrWhiteSpace(fullName))
                return RedirectToAction(nameof(ManageBarbers));

            var barber = new Barber
            {
                FullName = fullName,
                Role = role ?? string.Empty,
                Bio = bio ?? string.Empty,
                ImageUrl = imageUrl ?? string.Empty,
                InstagramUrl = instagramUrl ?? string.Empty,
                IsActive = true
            };

            _db.Barbers.Add(barber);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(ManageBarbers));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleBarber(int id)
        {
            if (!IsAdmin) return RedirectToAction(nameof(Login));

            var barber = await _db.Barbers.FindAsync(id);
            if (barber != null)
            {
                barber.IsActive = !barber.IsActive;
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ManageBarbers));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBarber(int id)
        {
            if (!IsAdmin) return RedirectToAction(nameof(Login));

            var barber = await _db.Barbers.FindAsync(id);
            if (barber != null)
            {
                _db.Barbers.Remove(barber);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ManageBarbers));
        }

        // ── Продукти ──

        [HttpGet]
        public async Task<IActionResult> ManageProducts()
        {
            if (!IsAdmin) return RedirectToAction(nameof(Login));

            var products = await _db.Products.OrderBy(p => p.Category).ThenBy(p => p.Id).ToListAsync();
            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(string name, string description, decimal price, ProductCategory category, int stock, string? imageUrl)
        {
            if (!IsAdmin) return RedirectToAction(nameof(Login));

            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["ProductError"] = "Името е задължително.";
                return RedirectToAction(nameof(ManageProducts));
            }

            if (price < 0)
            {
                TempData["ProductError"] = "Цената не може да е отрицателна.";
                return RedirectToAction(nameof(ManageProducts));
            }

            if (stock < 0) stock = 0;

            _db.Products.Add(new Product
            {
                Name = name,
                Description = description ?? string.Empty,
                Price = price,
                Category = category,
                Stock = stock,
                ImageUrl = imageUrl ?? string.Empty,
                IsActive = true
            });
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(ManageProducts));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleProduct(int id)
        {
            if (!IsAdmin) return RedirectToAction(nameof(Login));

            var product = await _db.Products.FindAsync(id);
            if (product != null)
            {
                product.IsActive = !product.IsActive;
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ManageProducts));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (!IsAdmin) return RedirectToAction(nameof(Login));

            var product = await _db.Products.FindAsync(id);
            if (product != null)
            {
                _db.Products.Remove(product);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ManageProducts));
        }

        // ── Поръчки ──

        [HttpGet]
        public async Task<IActionResult> ManageOrders()
        {
            if (!IsAdmin) return RedirectToAction(nameof(Login));

            var orders = await _db.Orders
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int id, OrderStatus status)
        {
            if (!IsAdmin) return RedirectToAction(nameof(Login));

            var order = await _db.Orders.FindAsync(id);
            if (order != null)
            {
                order.Status = status;
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ManageOrders));
        }

        // ── Временна диагностика (изтрий след като приключим) ──

        [HttpGet]
        public async Task<IActionResult> DebugOrders()
        {
            var count = await _db.Orders.CountAsync();
            var latest = await _db.Orders.OrderByDescending(o => o.CreatedAt).Take(5).ToListAsync();
            var text = $"Общо поръчки в базата: {count}\n\n" +
                string.Join("\n", latest.Select(o => $"#{o.Id} | {o.ConfirmationCode} | {o.ClientName} | {o.CreatedAt}"));
            return Content(text);
        }

        [HttpGet]
        public async Task<IActionResult> DebugServices()
        {
            var services = await _db.Services.OrderBy(s => s.Id).ToListAsync();
            var text = string.Join("\n", services.Select(s => $"#{s.Id} | {s.Name} | {s.Description}"));
            return Content(text);
        }

        // ── Статистики ──

        [HttpGet]
        public async Task<IActionResult> Statistics()
        {
            if (!IsAdmin) return RedirectToAction(nameof(Login));

            var appointments = await _db.Appointments
                .Include(a => a.Service)
                .Include(a => a.Barber)
                .ToListAsync();

            ViewBag.Total = appointments.Count;
            ViewBag.Pending = appointments.Count(a => a.Status == AppointmentStatus.Pending);
            ViewBag.Confirmed = appointments.Count(a => a.Status == AppointmentStatus.Confirmed);
            ViewBag.Completed = appointments.Count(a => a.Status == AppointmentStatus.Completed);
            ViewBag.Cancelled = appointments.Count(a => a.Status == AppointmentStatus.Cancelled);
            ViewBag.Revenue = appointments
                .Where(a => a.Status == AppointmentStatus.Completed && a.Service != null)
                .Sum(a => a.Service!.Price);

            ViewBag.TopServices = appointments
                .Where(a => a.Service != null)
                .GroupBy(a => a.Service!.Name)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();

            ViewBag.TopBarbers = appointments
                .Where(a => a.Barber != null)
                .GroupBy(a => a.Barber!.FullName)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();

            return View();
        }

        // ── CSV Export ──

        [HttpGet]
        public async Task<IActionResult> ExportCsv()
        {
            if (!IsAdmin) return RedirectToAction(nameof(Login));

            var appointments = await _db.Appointments
                .Include(a => a.Service)
                .Include(a => a.Barber)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("Id,Клиент,Email,Телефон,Услуга,Бръснар,Дата,Статус,Код");

            foreach (var a in appointments)
            {
                sb.AppendLine($"{a.Id},{a.ClientName},{a.ClientEmail},{a.ClientPhone}," +
                              $"{a.Service?.Name},{a.Barber?.FullName}," +
                              $"{a.AppointmentDate:dd.MM.yyyy HH:mm},{a.Status},{a.ConfirmationCode}");
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", $"reservations_{DateTime.Today:yyyyMMdd}.csv");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("IsAdmin");
            return RedirectToAction(nameof(Login));
        }
    }
}