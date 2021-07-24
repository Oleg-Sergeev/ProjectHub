using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.ViewModels.Account;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationContext _db;

        public AccountController(ApplicationContext db)
        {
            _db = db;
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            var user = await _db.Users.FirstAsync(u => u.Email == login.Email && u.Password == login.Password);

            await AuthenticateAsync(user.Email);

            return RedirectToAction("Index", "Home");
        }


        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel register)
        {
            await _db.Users.AddAsync(new User { Email = register.Email, Password = register.Password });

            await _db.SaveChangesAsync();

            await AuthenticateAsync(register.Email);

            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }


        private async Task AuthenticateAsync(string userName)
        {
            var claims = new List<Claim>
            {
                new (ClaimsIdentity.DefaultNameClaimType, userName),
                new (ClaimsIdentity.DefaultRoleClaimType, userName.Contains("admin") ? "admin" : "user")
            };

            ClaimsIdentity id = new(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}
