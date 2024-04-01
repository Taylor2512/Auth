using Auth.Shared.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Application.Security
{
    public class PasswordHandler
    {
        public byte[] GenerateSalt()
        {
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] randomBytes = new byte[64];
            rng.GetBytes(randomBytes);
            return randomBytes;
        }

        public string HashPassword(string password, byte[] salt)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltedPasswordBytes = CombineBytes(salt, passwordBytes);
            byte[] hashBytes = ComputeHash(saltedPasswordBytes);
            return Convert.ToBase64String(hashBytes);
        }

        public bool ValidatePassword(string enteredPassword, string hashedPassword, byte[] salt)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(enteredPassword);
            byte[] saltedPasswordBytes = CombineBytes(salt, passwordBytes);
            byte[] hashBytes = ComputeHash(saltedPasswordBytes);
            string hashedEnteredPassword = Convert.ToBase64String(hashBytes);
            return hashedEnteredPassword == hashedPassword;
        }

        private byte[] CombineBytes(byte[] first, byte[] second)
        {
            byte[] combined = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, combined, 0, first.Length);
            Buffer.BlockCopy(second, 0, combined, first.Length, second.Length);
            return combined;
        }

        private byte[] ComputeHash(byte[] bytes)
        {
            using SHA256 sha256 = SHA256.Create();
            return sha256.ComputeHash(bytes);
        }
    }

}
