using Guardian.Domain.CryptoUtility;
using System;

namespace Guardian.Domain.Account
{
    public class PasswordHelper
    {
        public static string GeneratePassword(string inputPwd, out string salt)
        {
            var saltByte = CryptoHelper.GenerateSalt();
            var password = Convert.ToBase64String(CryptoHelper.ComputeHash(inputPwd, saltByte));

            salt = Convert.ToBase64String(saltByte);

            return password;
        }
    }
}
