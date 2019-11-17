using Guardian.Domain.CryptoUtility;
using System;

namespace Guardian.Domain.Account
{
    public class TokenHelper
    {
        public static string GenerateToken() => Convert.ToBase64String(CryptoHelper.ComputeHash(Guid.NewGuid().ToString(), CryptoHelper.GenerateSalt()));
    }
}
