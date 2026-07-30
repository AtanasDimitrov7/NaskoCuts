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

        // Работно време на салона.
        private static readonly TimeSpan WeekdayOpen = new(9, 0, 0);
        private static readonly TimeSpan WeekdayClose = new(19, 0, 0);
        private static readonly TimeSpan WeekendOpen = new(10, 0, 0);
        private static readonly TimeSpan WeekendClose = new(16, 0, 0);

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

            if (!DateTime.TryParse(appointmentDate, out var parsedDate) || parsedDate < DateTime.Now)
                ModelState.AddModelError("appointmentDate", "Изберете валидна бъдеща дата и час.");

            var service = await _db.Services.FindAsync(serviceId);
            if (service == null)
            {
                ModelState.AddModelError("serviceId", "Невалидна услуга.");
            }
            else if (ModelState.IsValid && !IsWithinWorkingHours(parsedDate, service.DurationMinutes))
            {
                ModelState.AddModelError("appointmentDate",
                    "Изберете час в работното време: Пон–Пет 09:00–19:00, Съб–Нед 10:00–16:00 (услугата трябва да приключи преди затваряне).");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Services = await _db.Services.Where(s => s.IsActive).ToListAsync();
                ViewBag.Barbers = await _db.Barbers.Where(b => b.IsActive).ToListAsync();
                return View();
            }

            var parsedDateUtc = DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc);

            var newEnd = parsedDateUtc.AddMinutes(service!.DurationMinutes);

            var overlap = await _db.Appointments.AnyAsync(a =>
                a.BarberId == barberId &&
                a.Status != AppointmentStatus.Cancelled &&
                a.AppointmentDate < newEnd &&
                parsedDateUtc < a.AppointmentDate.AddMinutes(a.Service.DurationMinutes));

            if (overlap)
            {
                var nextSlot = await FindNextAvailableSlotAsync(barberId, service.DurationMinutes, parsedDateUtc);

                ViewBag.Error = nextSlot.HasValue
                    ? $"Този бръснар вече има резервация в това време. Следващият свободен час е {nextSlot.Value:dd.MM.yyyy HH:mm}."
                    : "Този бръснар вече има резервация в това време. Моля изберете друг ден.";

                ViewBag.Services = await _db.Services.Where(s => s.IsActive).ToListAsync();
                ViewBag.Barbers = await _db.Barbers.Where(b => b.IsActive).ToListAsync();
                return View();
            }

            var confirmation = $"NC-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(100, 999)}";

            var appointment = new Appointment
            {
                ClientName = clientName,
                ClientEmail = clientEmail,
                ClientPhone = clientPhone,
                ServiceId = serviceId,
                BarberId = barberId,
                AppointmentDate = parsedDateUtc,
                Notes = notes ?? string.Empty,
                ConfirmationCode = confirmation,
                Status = AppointmentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _db.Appointments.Add(appointment);
            await _db.SaveChangesAsync();

            var barber = await _db.Barbers.FindAsync(barberId);

            TempData["ClientName"] = clientName;
            TempData["Service"] = service.Name;
            TempData["Barber"] = barber?.FullName ?? "—";
            TempData["Date"] = parsedDate.ToString("dd.MM.yyyy HH:mm");
            TempData["Confirmation"] = confirmation;

            return RedirectToAction(nameof(AppointmentConfirmed));
        }

        private static bool IsWithinWorkingHours(DateTime start, int durationMinutes)
        {
            var isWeekend = start.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
            var open = isWeekend ? WeekendOpen : WeekdayOpen;
            var close = isWeekend ? WeekendClose : WeekdayClose;

            var startTime = start.TimeOfDay;
            var endTime = startTime + TimeSpan.FromMinutes(durationMinutes);

            return startTime >= open && endTime <= close;
        }

        private async Task<DateTime?> FindNextAvailableSlotAsync(int barberId, int durationMinutes, DateTime fromUtc)
        {
            var busySlots = await _db.Appointments
                .Where(a => a.BarberId == barberId &&
                            a.Status != AppointmentStatus.Cancelled &&
                            a.AppointmentDate >= fromUtc.Date)
                .Select(a => new { Start = a.AppointmentDate, a.Service.DurationMinutes })
                .OrderBy(a => a.Start)
                .ToListAsync();

            var candidate = fromUtc;
            var searchLimit = fromUtc.AddDays(30);

            while (candidate < searchLimit)
            {
                var isWeekend = candidate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
                var open = isWeekend ? WeekendOpen : WeekdayOpen;
                var close = isWeekend ? WeekendClose : WeekdayClose;

                if (candidate.TimeOfDay < open)
                    candidate = candidate.Date + open;

                if (candidate.TimeOfDay + TimeSpan.FromMinutes(durationMinutes) > close)
                {
                    candidate = candidate.Date.AddDays(1) + WeekdayOpen;
                    continue;
                }

                var candidateEnd = candidate.AddMinutes(durationMinutes);
                var conflict = busySlots.FirstOrDefault(b =>
                    b.Start < candidateEnd && candidate < b.Start.AddMinutes(b.DurationMinutes));

                if (conflict == null)
                    return candidate;

                candidate = conflict.Start.AddMinutes(conflict.DurationMinutes);
            }

            return null;
        }

        [HttpGet]
        public IActionResult AppointmentConfirmed()
        {
            if (TempData["Confirmation"] == null)
                return RedirectToAction(nameof(Index));
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode = null)
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = statusCode ?? HttpContext.Response.StatusCode
            });
        }
    }
}