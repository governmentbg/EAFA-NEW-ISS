using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace IARA.Web
{
    internal static class CryptoUtils
    {
        private static Aes aes;

        public static Aes Aes
        {
            get
            {
                if (aes == null)
                {
                    Aes cryptoAlgorithm = Aes.Create();
                    cryptoAlgorithm.Mode = CipherMode.CBC;
                    cryptoAlgorithm.Padding = PaddingMode.PKCS7;
                    cryptoAlgorithm.Key = GetAssemblyKey();
                    //cryptoAlgorithm.GenerateIV();
                    //IV = cryptoAlgorithm.IV;
                    aes = cryptoAlgorithm;
                }

                return aes;
            }
        }

        public static ICryptoTransform Decryptor
        {
            get
            {
                return Aes.CreateDecryptor();
            }
        }

        public static ICryptoTransform Encryptor
        {
            get
            {
                return Aes.CreateEncryptor();
            }
        }

        public static Stream DecryptStream(Stream stream)
        {
            byte[] lengthArray = new byte[sizeof(int)];
            stream.Read(lengthArray, 0, lengthArray.Length);

            byte[] ivs = new byte[BitConverter.ToInt32(lengthArray)];
            stream.Read(ivs, 0, ivs.Length);
            Aes.IV = ivs;

            MemoryStream memStream = new MemoryStream();
            byte[] buffer = new byte[4096];
            int read = 0;

            using (CryptoStream cryptoStream = new CryptoStream(stream, Decryptor, CryptoStreamMode.Read))
            {
                while ((read = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memStream.Write(buffer, 0, read);
                }
            }

            memStream.Position = 0;

            return memStream;
        }

        public static void DeleteNotUsedSettingFiles(string location, string hostingEnvironment, string defaultSettingsFile)
        {
            DirectoryInfo dir = new DirectoryInfo(location);

            if (dir.Exists)
            {
                FileInfo[] files = dir.GetFiles($"appsettings.*.json");

                foreach (var file in files)
                {
                    if (file.Name != $"appsettings.{hostingEnvironment}.json" && file.Name != defaultSettingsFile)
                    {
                        try
                        {
                            RemoveReadonlyAttribute(file.FullName);
                            file.Delete();
                        }
                        catch (UnauthorizedAccessException)
                        {

                        }
                    }
                }
            }
        }

        public static void RemoveReadonlyAttribute(string fileFullPath)
        {
            FileAttributes attributes = File.GetAttributes(fileFullPath);

            if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                // Make the file RW
                RemoveAttribute(attributes, FileAttributes.ReadOnly);
                File.SetAttributes(fileFullPath, attributes);
            }
        }

        private static FileAttributes RemoveAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
        {
            return attributes & ~attributesToRemove;
        }

        public static bool EncryptFile(string filePath, string encryptedFilePath)
        {
            if (!File.Exists(encryptedFilePath))
            {
                byte[] buffer = new byte[4096];

                Aes.GenerateIV();

                byte[] ivs = Aes.IV;

                using (FileStream encryptedFile = new FileStream(encryptedFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    byte[] lengthArray = BitConverter.GetBytes(ivs.Length);
                    encryptedFile.Write(lengthArray);
                    encryptedFile.Write(ivs, 0, ivs.Length);

                    using (CryptoStream cryptoStream = new CryptoStream(encryptedFile, Encryptor, CryptoStreamMode.Write, false))
                    {
                        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            int read = 0;
                            while ((read = fileStream.Read(buffer)) > 0)
                            {
                                cryptoStream.Write(buffer, 0, read);
                            }
                        }
                    }
                }

                return true;
            }

            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                return EncryptFile(filePath, encryptedFilePath);
            }
            catch (UnauthorizedAccessException)
            {
                Console.Error.WriteLine("Can not delete unencrypted configuration file");
            }

            return false;
        }

        private static byte[] GenerateHash(string value)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(value);
            SHA256 hashAlgorithm = SHA256.Create();
            return hashAlgorithm.ComputeHash(buffer);
        }

        private static byte[] GetAssemblyKey()
        {
            Assembly asm = Assembly.GetEntryAssembly();

            string key = asm.GetCustomAttribute<GuidAttribute>()?.Value;

            if (string.IsNullOrEmpty(key))
            {
                key = asm.GetCustomAttribute<UserSecretsIdAttribute>()?.UserSecretsId;
            }

            if (string.IsNullOrEmpty(key))
            {
                FileInfo fileInfo = new FileInfo(Process.GetCurrentProcess().MainModule.FileName);

                if (fileInfo.Exists)
                {
                    byte[] buffer = new byte[256];
                    using var fileStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    fileStream.Read(buffer, 0, buffer.Length);

                    buffer = SHA256.Create().ComputeHash(buffer);
                    return buffer;
                }

                return null;
            }
            else
            {
                return GenerateHash(key);
            }
        }
    }
}
