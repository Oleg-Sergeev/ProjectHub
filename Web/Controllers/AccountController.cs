using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Data.Authorization;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using Web.ViewModels.Account;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationContext _db;

        private readonly IEmailSender _emailSender;

        private readonly IWebHostEnvironment _environment;


        public AccountController(ApplicationContext db, IEmailSender emailSender, IWebHostEnvironment environment)
        {
            _db = db;
            _emailSender = emailSender;
            _environment = environment;
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
                    if (!user.HasConfirmedEmail)
                    {
                        ModelState.AddModelError("", "Email is not confirmed");

                        return View(login);
                    }

                    PasswordHasher<User> passwordHasher = new();
                    var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, login.Password);

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
                        PasswordHash = new PasswordHasher<User>().HashPassword(null, signIn.Password),
                        Role = await _db.Roles.FirstAsync(r => r.Name == Constants.UserRoleName),
                        SecretKey = Convert.ToBase64String(new HMACSHA256().Key)
                    };

                    await _db.Users.AddAsync(user);
                    await _db.SaveChangesAsync();

                    var timeStep = DateTime.Now.Ticks;
                    var code = new PasswordHasher<User>().HashPassword(null, $"{user.SecretKey}{user.Email}{user.PasswordHash}");

                    var confirmUrl = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new { userId = user.Id, code },
                        protocol: HttpContext.Request.Scheme);


                    string filePath = @$"{_environment.WebRootPath}\EmailVerificationTemplate.html";

                    var body = "";
                    using (StreamReader str = new(filePath))
                    {
                        body = await str.ReadToEndAsync();
                    }
                    body = body.Replace("[confirmUrl]", confirmUrl).Replace("[email]", user.Email);


                    var verificationMailRequest = new MailRequest()
                    {
                        ToEmail = user.Email,
                        Subject = "Complete registration",
                        Body = body,
                        MessagePriority = MimeKit.MessagePriority.Urgent
                    };

                    await _emailSender.SendEmailAsync(verificationMailRequest);


                    return Content("Confirm your email");
                }

                ModelState.AddModelError("", "Email is already in use");
            }

            return View(signIn);
        }


        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(int? userId, string code)
        {
            var signIn = nameof(SignIn);

            ModelState.AddModelError("", "Email is not confirmed");


            if (userId == null || code == null) return View(signIn);


            var user = await _db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);


            if (user == null) return View(signIn);


            PasswordHasher<User> passwordHasher = new();

            try
            {
                var result = passwordHasher.VerifyHashedPassword(user, code, $"{user.SecretKey}{user.Email}{user.PasswordHash}");

                if (result == PasswordVerificationResult.Failed) return View(signIn);
            }
            catch
            {
                return View(signIn);
            }



            await AuthenticateAsync(user);

            user.HasConfirmedEmail = true;
            await _db.SaveChangesAsync();


            ModelState.Remove("");

            return RedirectToAction("Index", "Home");
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
