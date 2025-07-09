using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using Konscious.Security.Cryptography;


namespace RegistrationAndAuthorisationViaCookieMinimalAPI.Services.Realisations
{
    public class Argon2PasswordHasher<TUser> : IPasswordHasher<TUser> where TUser : class
    {
        public string HashPassword(TUser user, string password)
        {
            var argon = new Argon2id(
                System.Text.Encoding.UTF8.GetBytes(password));
            argon.Salt = SecureRandomBytes(16);
            argon.DegreeOfParallelism = 8;
            argon.Iterations = 4;
            argon.MemorySize = 1024 * 64;
            var hash = argon.GetBytes(16);
            return Convert.ToBase64String(argon.Salt) + ":" + Convert.ToBase64String(hash);
        }

        public PasswordVerificationResult VerifyHashedPassword(
            TUser user, string hashedPassword, string providedPassword)
        {
            var parts = hashedPassword.Split(':');
            var salt = Convert.FromBase64String(parts[0]);
            var expected = Convert.FromBase64String(parts[1]);

            var argon = new Argon2id(
                System.Text.Encoding.UTF8.GetBytes(providedPassword));
            argon.Salt = salt;
            argon.DegreeOfParallelism = 8;
            argon.Iterations = 4;
            argon.MemorySize = 1024 * 64;
            var actual = argon.GetBytes(16);

            return CryptographicOperations.FixedTimeEquals(actual, expected)
                ? PasswordVerificationResult.Success
                : PasswordVerificationResult.Failed;
        }

        private static byte[] SecureRandomBytes(int length)
        {
            var buffer = new byte[length];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(buffer);
            return buffer;
        }
    }



}
