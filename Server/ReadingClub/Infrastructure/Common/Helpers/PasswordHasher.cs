using System.Security.Cryptography;

namespace ReadingClub.Infrastructure.Common.Helpers
{
    public static class PasswordHasher
    {
        public static string GenerateSalt(int saltLength = 16)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] saltBytes = new byte[saltLength];
                rng.GetBytes(saltBytes);
                return Convert.ToBase64String(saltBytes);
            }
        }

        public static string HashPassword(string password, string salt)
        {
            int iterations = 1000; // Adjust the number of iterations according to your security needs
            byte[] saltBytes = Convert.FromBase64String(salt);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, iterations, HashAlgorithmName.SHA256))
            {
                byte[] hashBytes = pbkdf2.GetBytes(32); // 32 bytes for a 256-bit key
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
