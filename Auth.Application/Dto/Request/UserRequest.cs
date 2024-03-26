using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Auth.Application.Dto.Request
{
    public class UserRequest
    {
        public string? UserName { get; set; }
        [EmailAddress(ErrorMessage = "El campo 'Email' debe tener un formato válido de dirección de correo electrónico.")]
        //[Required(ErrorMessage = "El campo 'Email' es obligatorio.")]
        [StringLength(maximumLength: 100, MinimumLength = 8, ErrorMessage = "El campo 'Email' debe tener entre 8 y 100 caracteres.")]
        public required string Email { get; set; }
        public required string Password { get; set; }
        public byte[]? Salt { get; private set; }

        public void Excecute()
        {
            GeneratePasswordHash();
            UserName ??= Email;

        }
        private void GeneratePasswordHash()
        {
            // Generate a salt
            Salt = GenerateSalt();

            // Convert the password to bytes
            byte[] passwordBytes = Encoding.UTF8.GetBytes(Password);

            // Combine the salt and password bytes
            byte[] saltedPasswordBytes = CombineBytes(Salt, passwordBytes);

            // Compute the hash of the salted password bytes
            byte[] hashBytes = ComputeHash(saltedPasswordBytes);

            // Convert the hash bytes to a string
            Password = Convert.ToBase64String(hashBytes);
        }

        private byte[] GenerateSalt()
        {
            // Generate a random salt
            byte[] salt = new byte[16];
            using (System.Security.Cryptography.RNGCryptoServiceProvider rng = new())
            {
                rng.GetBytes(salt);
            }
            return salt;
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
            using System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create();
            return sha256.ComputeHash(bytes);
        }

        public bool ValidatePassword(string enteredPassword, byte[] saltFromDatabase)
        {
            // Convert the password to bytes
            byte[] passwordBytes = Encoding.UTF8.GetBytes(Password);

            // Combine the salt and password bytes
            byte[] saltedPasswordBytes = CombineBytes(saltFromDatabase, passwordBytes);

            // Compute the hash of the salted password bytes
            byte[] hashBytes = ComputeHash(saltedPasswordBytes);

            // Convert the hash bytes to a string
            Password = Convert.ToBase64String(hashBytes);
            return enteredPassword == Password;
        }
    }
}
