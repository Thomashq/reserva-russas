using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using RR.Core.Services;
using System;
using System.Security.Cryptography;
using System.Text;

namespace RR.Service.Service
{
    public class PasswordService : IPasswordService
    {
        private readonly byte[] _salt = Encoding.UTF8.GetBytes("YourFixedSaltHere12345678901234567890");

        public string HashPassword(string password)
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: _salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hashed;
        }

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            string hashedProvided = HashPassword(providedPassword);
            return hashedPassword == hashedProvided;
        }
    }
}
