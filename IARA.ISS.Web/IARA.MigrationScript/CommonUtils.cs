using System.Security.Cryptography;
using System.Text;

namespace IARA.MigrationScript
{
    public static class CommonUtils
    {
        public static string GetPasswordHash(string password, string salt)
        {
            KeyedHashAlgorithm hashAlgorithm = HMAC.Create("HMACSHA256");

            salt = salt.ToUpper().Substring(0, 5);

            hashAlgorithm.Key = Encoding.UTF8.GetBytes(salt);
            byte[] hashedBytes = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(password));
            string hashedPassword = GetHex(hashedBytes);

            return hashedPassword;
        }

        private static string GetHex(byte[] array)
        {
            StringBuilder builder = new();
            foreach (byte item in array)
            {
                _ = builder.Append(item.ToString("x2"));
            }

            return builder.ToString();
        }
    }
}
