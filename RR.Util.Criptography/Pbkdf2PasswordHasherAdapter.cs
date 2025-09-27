using Microsoft.AspNetCore.Identity;
using RR.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Util.Criptography
{
    public class Pbkdf2PasswordHasherAdapter : IPasswordHasher<AppUser>
    {
        private readonly HashPass _hash = new();

        public string HashPassword(AppUser user, string password)
            => _hash.HashPassword(password);

        public PasswordVerificationResult VerifyHashedPassword(
            AppUser user, string hashedPassword, string providedPassword)
            => _hash.VerifyPassword(hashedPassword, providedPassword)
                ? PasswordVerificationResult.Success
                : PasswordVerificationResult.Failed;
    }

}
