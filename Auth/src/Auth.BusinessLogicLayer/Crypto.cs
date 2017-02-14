using System;
using System.Security.Cryptography;
using System.Text;
using Auth.BusinessLogicLayer.Interfaces;

namespace Auth.BusinessLogicLayer
{
    public class Crypto : ICrypto
    {
        public string ComputeHash(string text, string salt)
        {
            using (SHA512 sha = SHA512.Create())
            {
                var hashedBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(text + salt));
                return BitConverter.ToString(hashedBytes).Replace("-", string.Empty).ToUpper();
            }
        }

        public string GenerateSalt()
        {
            var bytes = new byte[16];
            using (var keyGenerator = RandomNumberGenerator.Create())
            {
                keyGenerator.GetBytes(bytes);
                return BitConverter.ToString(bytes).Replace("-", string.Empty).ToUpper();
            }
        }
    }
}