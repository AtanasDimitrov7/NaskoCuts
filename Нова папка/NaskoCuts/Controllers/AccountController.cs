using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaskoCuts.Data;
using NaskoCuts.Models.Entities;

namespace NaskoCuts.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _db;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (_signInManager.IsSignedIn(User))
                return RedirectToAction(nameof(Profile));
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string fullName, string email, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Моля попълнете всички полета.";
                return View();
            }

            if (password != confirmPassword)
            {
                ViewBag.Error = "Паролите не съвпадат.";
                return View();
            }

            var user = new ApplicationUser
            {
                FullName = fullName,
                UserName = email,
                Email = email
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                ViewBag.Error = string.Join(" ", result.Errors.Select(e => e.Description));
                return View();
            }

            await _userManager.AddToRoleAsync(user, "Client");
            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction(nameof(Profile));
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (_signInManager.IsSignedIn(User))
                return RedirectToAction(nameof(Profile));
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Моля въведете имейл и парола.";
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                ViewBag.Error = "Грешен имейл или парола.";
                return View();
            }

            return RedirectToAction(nameof(Profile));
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            if (!_signInManager.IsSignedIn(User))
                return RedirectToAction(nameof(Login));

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction(nameof(Login));

            var appointments = await _db.Appointments
                .Include(a => a.Service)
                .Include(a => a.Barber)
                .Where(a => a.ClientEmail == user.Email)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            ViewBag.User = user;
            return View(appointments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // ── Временно, за диагностика на ролята "Admin" (само чете, нищо не променя) ──

        [HttpGet]
        public async Task<IActionResult> DebugCheckAdmin(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Content($"Няма акаунт с имейл: {email}");

            var roles = await _userManager.GetRolesAsync(user);

            return Content($"User: {email} (Id: {user.Id})\nРоли: {(roles.Any() ? string.Join(", ", roles) : "(няма)")}");
        }
    }
}