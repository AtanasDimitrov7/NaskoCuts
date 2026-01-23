using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NaskoCuts.Models;

namespace NaskoCuts.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // =========================
        // HOME
        // =========================
        public async Task<IActionResult> Index()
        {
            await Task.CompletedTask;
            return View();
        }

        public async Task<IActionResult> Privacy()
        {
            await Task.CompletedTask;
            return View();
        }

        // =========================
        // BOOK APPOINTMENT
        // =========================

        // GET: show form
        [HttpGet]
        public async Task<IActionResult> BookAppointment()
        {
            await Task.CompletedTask;
            return View();
        }

        // POST: process form
        [HttpPost]
        public async Task<IActionResult> BookAppointment(
            string name,
            string email,
            string phone,
            string service,
            string date,
            string time,
            string notes)
        {
            await SaveAppointmentAsync(name, email, phone, service, date, time, notes);

            // pass fake confirmation data
            TempData["ClientName"] = name;
            TempData["Service"] = service;
            TempData["Date"] = date;
            TempData["Time"] = time;
            TempData["Confirmation"] = $"NC-{DateTime.Now:yyyyMMdd}-{Random.Shared.Next(100, 999)}";

            return RedirectToAction(nameof(AppointmentConfirmed));
        }

        // =========================
        // CONFIRMATION
        // =========================
        [HttpGet]
        public async Task<IActionResult> AppointmentConfirmed()
        {
            await Task.CompletedTask;

            // prevent direct access
            if (TempData["Confirmation"] == null)
                return RedirectToAction(nameof(Index));

            return View();
        }

        // =========================
        // ERROR
        // =========================
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            await Task.CompletedTask;

            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

        // =========================
        // ASYNC MOCK SAVE
        // =========================
        private async Task SaveAppointmentAsync(
            string name,
            string email,
            string phone,
            string service,
            string date,
            string time,
            string notes)
        {
            // simulate async DB / API call
            await Task.Delay(300);

            _logger.LogInformation(
                "New appointment: {Name}, {Email}, {Phone}, {Service}, {Date}, {Time}, {Notes}",
                name, email, phone, service, date, time, notes
            );
        }
    }
}
