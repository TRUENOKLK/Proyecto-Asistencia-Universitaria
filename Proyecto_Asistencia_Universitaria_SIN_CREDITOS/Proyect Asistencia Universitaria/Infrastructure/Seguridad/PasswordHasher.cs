using Aplication.Interface;
using System;
using System.Security.Cryptography;

namespace Infrastructure.Seguridad
{

    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSizeBytes = 16;   
        private const int KeySizeBytes = 32;    
        private const int Iterations = 100_000; 

        
        public string Hash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("La contraseña no puede estar vacía.", nameof(password));

            using var pbkdf2 = new Rfc2898DeriveBytes(password, SaltSizeBytes, Iterations, HashAlgorithmName.SHA256);
            byte[] salt = pbkdf2.Salt;
            byte[] key = pbkdf2.GetBytes(KeySizeBytes);

            return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
        }

        /// <summary>
        /// Verifica que una contraseña en texto plano coincida con un hash previamente
        /// generado por <see cref="Hash"/>. Úsalo en el login.
        /// </summary>
        public bool Verify(string password, string? hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashedPassword))
                return false;

            var parts = hashedPassword.Split('.', 3);
            if (parts.Length != 3) return false;

            try
            {
                int iterations = int.Parse(parts[0]);
                byte[] salt = Convert.FromBase64String(parts[1]);
                byte[] key = Convert.FromBase64String(parts[2]);

                using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
                byte[] keyToCheck = pbkdf2.GetBytes(key.Length);

                
                return CryptographicOperations.FixedTimeEquals(keyToCheck, key);
            }
            catch
            {
                
                return false;
            }
        }
    }
}
