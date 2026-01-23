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

        // --------------------------
        // Home Pages
        // --------------------------

        public async Task<IActionResult> Index()
        {
            // Simulate async work (e.g. loading featured services)
            await Task.CompletedTask;
            return View();
        }

        public async Task<IActionResult> Privacy()
        {
            await Task.CompletedTask;
            return View();
        }

        // --------------------------
        // BookAppointment Actions
        // --------------------------

        // GET: Show the appointment form
        [HttpGet]
        public async Task<IActionResult> BookAppointment()
        {
            await Task.CompletedTask;
            return View();
        }

        // POST: Handle form submission
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
            // Simulate async database save
            await SaveAppointmentAsync(name, email, phone, service, date, time, notes);

            TempData["Message"] = "Your appointment has been booked successfully!";
            return RedirectToAction("AppointmentConfirmed");
        }

        // --------------------------
        // Confirmation Page
        // --------------------------

        [HttpGet]
        public async Task<IActionResult> AppointmentConfirmed()
        {
            await Task.CompletedTask;
            return View();
        }

        // --------------------------
        // Error Handling
        // --------------------------

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            await Task.CompletedTask;

            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }


        private async Task SaveAppointmentAsync(
            string name,
            string email,
            string phone,
            string service,
            string date,
            string time,
            string notes)
        {
            await Task.Delay(300);

            _logger.LogInformation(
                "New appointment: {Name}, {Email}, {Phone}, {Service}, {Date}, {Time}, {Notes}",
                name, email, phone, service, date, time, notes
            );
        }
    }
}
