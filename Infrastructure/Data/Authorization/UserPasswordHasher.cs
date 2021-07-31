using System;
using System.Security.Cryptography;
using Infrastructure.Data.Entities.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data.Authorization
{
    public class UserHasher
    {
        public static string HashPassword(string password) =>
            new PasswordHasher<User>().HashPassword(null, password);

        public static string CreateToken(User user) =>
            new PasswordHasher<User>().HashPassword(user, GetUserString(user));

        public static string CreateSecretKey() =>
            Convert.ToBase64String(new HMACSHA512().Key);

        public static PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword = null) =>
            new PasswordHasher<User>().VerifyHashedPassword(user, hashedPassword, providedPassword ?? GetUserString(user));


        private static string GetUserString(User user) => $"{user.Id}{user.SecretKey}{user.Email}{user.PasswordHash}";
    }
}
