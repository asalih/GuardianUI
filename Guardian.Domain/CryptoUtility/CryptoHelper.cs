using System;
using System.Security.Cryptography;

namespace Guardian.Domain.CryptoUtility
{
    public class CryptoHelper
    {
        private const int SaltByteSize = 24;
        private const int HashByteSize = 24;
        private const int HasingIterationsCount = 10101;

        internal static byte[] GenerateSalt(int saltByteSize = SaltByteSize)
        {
            using (var saltGenerator = new RNGCryptoServiceProvider())
            {
                var salt = new byte[saltByteSize];
                saltGenerator.GetBytes(salt);

                return salt;
            }
        }

        internal static byte[] ComputeHash(string text, byte[] salt)
        {
            using (var hashGenerator = new Rfc2898DeriveBytes(text, salt))
            {
                hashGenerator.IterationCount = HasingIterationsCount;

                return hashGenerator.GetBytes(HashByteSize);
            }
        }

        internal static bool CompareHash(string text, string hash, byte[] salt) => Convert.ToBase64String(ComputeHash(text, salt)).Equals(hash);
    }
}
