using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Data.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await _db.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == login.Email);


                if (user != null)
                {
                    PasswordHasher<User> passwordHasher = new();
                    var result = passwordHasher.VerifyHashedPassword(user, user.Password, login.Password);

                    if (result != PasswordVerificationResult.Failed)
                    {
                        await AuthenticateAsync(user);

                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
                        else return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError("", "Incorrect login and (or) password");
            }

            return View(login);
        }


        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel signIn)
        {
            if (ModelState.IsValid)
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == signIn.Email);

                if (user == null)
                {
                    user = new()
                    {
                        Email = signIn.Email,
                        Password = new PasswordHasher<User>().HashPassword(null, signIn.Password),
                        Role = await _db.Roles.FirstAsync(r => r.Name == Constants.UserRoleName)
                    };


                    await _db.Users.AddAsync(user);
                    await _db.SaveChangesAsync();

                    await AuthenticateAsync(user);

                    return RedirectToAction("Index", "Home");
                }
                else ModelState.AddModelError("", "Email is already in use");
            }

            return View(signIn);
        }


        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }


        private async Task AuthenticateAsync(User user)
        {
            var claims = new List<Claim>
            {
                new (ClaimsIdentity.DefaultNameClaimType, user.Email),
                new (ClaimsIdentity.DefaultRoleClaimType, user.Role.Name)
            };

            ClaimsIdentity id = new(claims, Constants.ApplicationCookie, ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}
