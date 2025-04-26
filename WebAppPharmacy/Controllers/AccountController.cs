using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using WebAppPharmacy.Models;
using System.Security.Claims;

namespace WebAppPharmacy.Controllers
{
    public class AccountController:Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [Route("register")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                ModelState.AddModelError("", "Username and password are required.");
                return View(user);
            }

            var userExists = _context.Users
                .FromSqlRaw("SELECT * FROM users WHERE \"Username\" = {0}", user.Username)
                .Any();

            if (userExists)
            {
                ModelState.AddModelError("", "Username already exists.");
                return View(user);
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

            _context.Database.ExecuteSqlRaw(
                "INSERT INTO users (\"Username\", \"PasswordHash\") VALUES ({0}, {1})",
                user.Username, passwordHash
            );

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(User user)
        {
            // Получаем пользователя через SQL
            var userInDb = _context.Users
                .FromSqlRaw("SELECT * FROM users WHERE \"Username\" = {0}", user.Username)
                .AsEnumerable() // чтобы выполнить запрос и перейти к обычному C#
                .FirstOrDefault();

            if (user == null || !BCrypt.Net.BCrypt.Verify(user.PasswordHash, userInDb.PasswordHash))
            {
                ModelState.AddModelError("", "Неверное имя пользователя или пароль");
                return View();
            }

            // Авторизация через куки
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var authProps = new AuthenticationProperties
            {
                IsPersistent = true
            };

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProps).Wait();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Удаляем все куки аутентификации
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Перенаправляем на страницу входа или главную
            return RedirectToAction("Login", "Account");
        }
    }
}
