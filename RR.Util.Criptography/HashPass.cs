using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RR.Util.Criptography
{
    public class HashPass
    {
        public string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 32);

            // Combina salt + hash em uma única string
            byte[] combined = new byte[48];
            Array.Copy(salt, 0, combined, 0, 16);
            Array.Copy(hash, 0, combined, 16, 32);

            return Convert.ToBase64String(combined);
        }

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            try
            {
                byte[] combined = Convert.FromBase64String(hashedPassword);

                if (combined.Length != 48)
                    return false;

                byte[] salt = new byte[16];
                Array.Copy(combined, 0, salt, 0, 16);

                byte[] originalHash = new byte[32];
                Array.Copy(combined, 16, originalHash, 0, 32);

                byte[] providedHash = KeyDerivation.Pbkdf2(
                    password: providedPassword,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 32);

                return CryptographicOperations.FixedTimeEquals(originalHash, providedHash);
            }
            catch
            {
                // Se houver qualquer erro na conversão, retorna false
                return false;
            }
        }

        public bool IsPasswordStrong(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                return false;

            bool hasUpper = false;
            bool hasLower = false;
            bool hasDigit = false;
            bool hasSpecial = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c)) hasUpper = true;
                else if (char.IsLower(c)) hasLower = true;
                else if (char.IsDigit(c)) hasDigit = true;
                else if (!char.IsLetterOrDigit(c)) hasSpecial = true;
            }

            return hasUpper && hasLower && hasDigit && hasSpecial;
        }
    }
}
