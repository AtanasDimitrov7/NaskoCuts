using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaskoCuts.Data;

namespace NaskoCuts.Controllers
{
    public class BarbersController : Controller
    {
        private readonly ApplicationDbContext _db;

        public BarbersController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var barbers = await _db.Barbers
                .Where(b => b.IsActive)
                .OrderBy(b => b.Id)
                .ToListAsync();

            return View(barbers);
        }
    }
}