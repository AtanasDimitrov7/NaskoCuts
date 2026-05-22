using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaskoCuts.Data;
using NaskoCuts.Models;
using NaskoCuts.Models.Entities;

namespace NaskoCuts.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> BookAppointment()
        {
            ViewBag.Services = await _db.Services.Where(s => s.IsActive).ToListAsync();
            ViewBag.Barbers = await _db.Barbers.Where(b => b.IsActive).ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> BookAppointment(
            string clientName,
            string clientEmail,
            string clientPhone,
            int serviceId,
            int barberId,
            string appointmentDate,
            string? notes)
        {
            if (string.IsNullOrWhiteSpace(clientName))
                ModelState.AddModelError("clientName", "Името е задължително.");

            if (string.IsNullOrWhiteSpace(clientEmail) || !clientEmail.Contains("@"))
                ModelState.AddModelError("clientEmail", "Въведи валиден имейл.");

            if (string.IsNullOrWhiteSpace(clientPhone))
                ModelState.AddModelError("clientPhone", "Телефонът е задължителен.");

            if (!DateTime.TryParse(appointmentDate, out var parsedDate) || parsedDate.Date < DateTime.Today)
                ModelState.AddModelError("appointmentDate", "Изберете валидна бъдеща дата.");

            if (!ModelState.IsValid)
            {
                ViewBag.Services = await _db.Services.Where(s => s.IsActive).ToListAsync();
                ViewBag.Barbers = await _db.Barbers.Where(b => b.IsActive).ToListAsync();
                return View();
            }

            var confirmation = $"NC-{DateTime.Now:yyyyMMdd}-{Random.Shared.Next(100, 999)}";

            var appointment = new Appointment
            {
                ClientName = clientName,
                ClientEmail = clientEmail,
                ClientPhone = clientPhone,
                ServiceId = serviceId,
                BarberId = barberId,
                AppointmentDate = parsedDate,
                Notes = notes ?? string.Empty,
                ConfirmationCode = confirmation,
                Status = AppointmentStatus.Pending,
                CreatedAt = DateTime.Now
            };

            _db.Appointments.Add(appointment);
            await _db.SaveChangesAsync();

            var service = await _db.Services.FindAsync(serviceId);
            var barber = await _db.Barbers.FindAsync(barberId);

            TempData["ClientName"] = clientName;
            TempData["Service"] = service?.Name ?? "—";
            TempData["Barber"] = barber?.FullName ?? "—";
            TempData["Date"] = parsedDate.ToString("dd.MM.yyyy HH:mm");
            TempData["Confirmation"] = confirmation;

            return RedirectToAction(nameof(AppointmentConfirmed));
        }

        [HttpGet]
        public IActionResult AppointmentConfirmed()
        {
            if (TempData["Confirmation"] == null)
                return RedirectToAction(nameof(Index));
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}