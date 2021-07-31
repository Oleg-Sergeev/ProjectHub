using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Infrastructure.Data.Authorization;
using Infrastructure.Data.Entities;
using Infrastructure.Data.Entities.Authorization;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.ViewModels.Account;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationContext _db;

        private readonly IEmailSender _emailSender;

        private readonly IAsyncFileTemplateParser _fileParser;

        private readonly string _emailTemplatePath;


        public AccountController(ApplicationContext db, IEmailSender emailSender, IWebHostEnvironment environment, IAsyncFileTemplateParser fileParser)
        {
            _db = db;
            _emailSender = emailSender;
            _fileParser = fileParser;

            _emailTemplatePath = @$"{environment.WebRootPath}\EmailVerificationTemplate.html";
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


                    var result = UserHasher.VerifyHashedPassword(user, user.PasswordHash, login.Password);

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

                if (user is null)
                {
                    user = new()
                    {
                        Email = signIn.Email,
                        PasswordHash = UserHasher.HashPassword(signIn.Password),
                        Role = await _db.Roles.FirstAsync(r => r.Name == Constants.UserRoleName),
                        SecretKey = UserHasher.CreateSecretKey()
                    };

                    await _db.Users.AddAsync(user);
                    await _db.SaveChangesAsync();

                    var token = UserHasher.CreateToken(user);

                    var confirmUrl = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new { userId = user.Id, token },
                        protocol: HttpContext.Request.Scheme);


                    var body = await _fileParser.ParseFileAsync(_emailTemplatePath, new()
                    {
                        { "Header", "Confirm email" },
                        { "Body", "Welcome to Project Hub!\r\nClick the button below to verify your email address." },
                        { "ButtonText", "Confirm email" },
                        { "Action", "confirm email" },
                        { "Link", confirmUrl }
                    });

                    var verificationMailRequest = new MailRequest()
                    {
                        ToEmail = user.Email,
                        Subject = "Complete registration",
                        Body = body
                    };

                    await _emailSender.SendEmailAsync(verificationMailRequest);


                    return View("SignInConfirm");
                }

                ModelState.AddModelError("", "Email is already in use");
            }

            return View(signIn);
        }


        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(int? userId, string token)
        {
            var signIn = nameof(SignIn);

            ModelState.AddModelError("", "Email is not confirmed");


            if (userId is null || token is null) return View(signIn);


            var user = await _db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user is null) return View(signIn);


            try
            {
                var result = UserHasher.VerifyHashedPassword(user, token);

                if (result == PasswordVerificationResult.Failed) return View(signIn);
            }
            catch
            {
                return View(signIn);
            }


            await AuthenticateAsync(user);

            user.SecretKey = UserHasher.CreateSecretKey();
            user.HasConfirmedEmail = true;
            await _db.SaveChangesAsync();


            ModelState.Remove("");

            return View("ConfirmEmailSuccess");
        }



        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgotPassword)
        {
            if (ModelState.IsValid)
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == forgotPassword.Email);

                if (user != null)
                {
                    var token = UserHasher.CreateToken(user);

                    var confirmUrl = Url.Action(
                        "ResetPassword",
                        "Account",
                        new { token, email = user.Email },
                        protocol: HttpContext.Request.Scheme);


                    var body = await _fileParser.ParseFileAsync(_emailTemplatePath, new()
                    {
                        { "Header", "Reset password" },
                        { "Body", "Tap the button below to reset your password" },
                        { "ButtonText", "Reset password" },
                        { "Action", "reset password" },
                        { "Link", confirmUrl }
                    });

                    var resetPasswordRequest = new MailRequest()
                    {
                        ToEmail = user.Email,
                        Subject = "Reset password",
                        Body = body
                    };

                    await _emailSender.SendEmailAsync(resetPasswordRequest);

                    return View("ForgotPasswordConfirm");
                }
            }

            ModelState.AddModelError("", "Email is not exists");

            return View(forgotPassword);
        }


        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            var forgotPassword = nameof(ForgotPassword);

            if (email is null) return BadRequest();


            ModelState.AddModelError("", "Reset password link is invalid");

            if (token is null) return View(forgotPassword);


            var user = await _db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user is null) return View(forgotPassword);


            try
            {
                var result = UserHasher.VerifyHashedPassword(user, token);

                if (result == PasswordVerificationResult.Failed) return View(forgotPassword);
            }
            catch
            {
                return View(forgotPassword);
            }


            ModelState.Remove("");

            var model = new ResetPasswordViewModel
            {
                Token = token,
                Email = email
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPassword)
        {
            if (!ModelState.IsValid) return View(resetPassword);


            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == resetPassword.Email);

            if (user is null) View(resetPassword);


            try
            {
                var result = UserHasher.VerifyHashedPassword(user, resetPassword.Token);

                if (result == PasswordVerificationResult.Failed) return View(resetPassword);
            }
            catch
            {
                return View(resetPassword);
            }

            user.PasswordHash = UserHasher.HashPassword(resetPassword.Password);
            user.SecretKey = UserHasher.CreateSecretKey();
            await _db.SaveChangesAsync();

            return RedirectToAction("Login", new { returnUrl = "~/" });
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
