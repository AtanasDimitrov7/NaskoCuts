using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaskoCuts.Data;

namespace NaskoCuts.Controllers
{
    public class ServicesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ServicesController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var services = await _db.Services
                .Where(s => s.IsActive)
                .OrderBy(s => s.Id)
                .ToListAsync();

            return View(services);
        }
    }
}