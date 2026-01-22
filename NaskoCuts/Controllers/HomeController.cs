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

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // --------------------------
        // BookAppointment Actions
        // --------------------------

        // GET: Show the appointment form
        public IActionResult BookAppointment()
        {
            return View();
        }

        // POST: Handle form submission
        [HttpPost]
        public IActionResult BookAppointment(
            string name,
            string email,
            string phone,
            string service,
            string date,
            string time,
            string notes)
        {
            // For now, just log the data
            _logger.LogInformation("New appointment: {Name}, {Email}, {Phone}, {Service}, {Date}, {Time}, {Notes}",
                name, email, phone, service, date, time, notes);

            TempData["Message"] = "Your appointment has been booked successfully!";
            return RedirectToAction("BookAppointment");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
