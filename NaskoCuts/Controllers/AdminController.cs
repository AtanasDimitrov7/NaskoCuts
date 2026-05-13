using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaskoCuts.Data;
using NaskoCuts.Models.Entities;
using System.Text;

namespace NaskoCuts.Controllers
{
    public class AdminController : Controller
    {
        private const string AdminPassword = "nasko1234";
        private readonly ApplicationDbContext _db;

        public AdminController(ApplicationDbContext db)
        {
            _db = db;
        }

        private bool IsAdmin => HttpContext.Session.GetString("IsAdmin") == "true";

        // ── Вход / Изход ──

        [HttpGet]
        public IActionResult Login()
        {
            if (IsAdmin) return RedirectToAction(nameof(Index));
            return View();
        }

        [HttpPost]
        public IActionResult Login(string password)
        {
            if (password == AdminPassword)
            {
                HttpContext.Session.SetString("IsAdmin", "true");
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Error = "Грешна парола.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("IsAdmin");
            return RedirectToAction(nameof(Login));
        }

        // ── Резервации ──

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin) return RedirectToAction(nameof(Login));

            var appointments = await _db.Appointments
                .Include(a => a.Service)
                .Include(a => a.Barber)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return View(appointments);
        }

        [HttpPost]
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

            ViewBag.TopServices = appointments
                .GroupBy(a => a.Service?.Name ?? "Неизвестна")
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();

            ViewBag.TopBarbers = appointments
                .GroupBy(a => a.Barber?.FullName ?? "Неизвестен")
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();

            ViewBag.Revenue = appointments
                .Where(a => a.Status == AppointmentStatus.Completed)
                .Sum(a => a.Service?.Price ?? 0);

            return View();
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
        public async Task<IActionResult> CreateService(string name, string description, decimal price, int durationMinutes, string imageUrl)
        {
            if (!IsAdmin) return RedirectToAction(nameof(Login));

            _db.Services.Add(new Service
            {
                Name = name,
                Description = description ?? string.Empty,
                Price = price,
                DurationMinutes = durationMinutes,
                ImageUrl = imageUrl ?? string.Empty,
                IsActive = true
            });

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(ManageServices));
        }

        [HttpPost]
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
        public async Task<IActionResult> CreateBarber(string fullName, string role, string bio, string imageUrl, string instagramUrl)
        {
            if (!IsAdmin) return RedirectToAction(nameof(Login));

            _db.Barbers.Add(new Barber
            {
                FullName = fullName,
                Role = role ?? string.Empty,
                Bio = bio ?? string.Empty,
                ImageUrl = imageUrl ?? string.Empty,
                InstagramUrl = instagramUrl ?? string.Empty,
                IsActive = true
            });

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(ManageBarbers));
        }

        [HttpPost]
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

        [HttpPost]
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

        // ── CSV Експорт ──

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
            sb.AppendLine("ID,Клиент,Имейл,Телефон,Услуга,Бръснар,Дата,Статус,Код");

            foreach (var a in appointments)
            {
                sb.AppendLine($"{a.Id}," +
                    $"\"{a.ClientName}\"," +
                    $"\"{a.ClientEmail}\"," +
                    $"\"{a.ClientPhone}\"," +
                    $"\"{a.Service?.Name}\"," +
                    $"\"{a.Barber?.FullName}\"," +
                    $"\"{a.AppointmentDate:dd.MM.yyyy HH:mm}\"," +
                    $"\"{a.Status}\"," +
                    $"\"{a.ConfirmationCode}\"");
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", $"appointments_{DateTime.Now:yyyyMMdd}.csv");
        }
    }
}