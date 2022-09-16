using System;
using IARA.Common.Utils;

namespace IARA.HMACUtil
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("Enter password: ");
                string password = Console.ReadLine();
                Console.Write("Enter email: ");
                string salt = Console.ReadLine();

                string hashedPassword = CommonUtils.GetPasswordHash(password, salt);
                Console.WriteLine("Hashed password(HMAC SHA256): ");
                Console.WriteLine(hashedPassword);
                Console.ReadLine();
                Console.Clear();
            }
        }
    }
}
