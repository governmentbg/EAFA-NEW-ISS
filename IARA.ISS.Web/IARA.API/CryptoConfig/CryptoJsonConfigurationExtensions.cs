using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace IARA.Web
{
    public static class CryptoJsonConfigurationExtensions
    {
        public static IConfigurationBuilder AddEncryptedJsonFile(this IConfigurationBuilder builder, string path)
        {
            return AddEncryptedJsonFile(builder, path, true);
        }

        public static IConfigurationBuilder AddEncryptedJsonFile(this IConfigurationBuilder builder, string path, bool reloadOnChange)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("File path must be a non-empty string.");
            }

            string encryptedFilePath = path + ".enc";

            if (File.Exists(path))
            {
                if (File.Exists(encryptedFilePath))
                {
                    CryptoUtils.RemoveReadonlyAttribute(encryptedFilePath);
                    File.Delete(encryptedFilePath);
                }

                if (!CryptoUtils.EncryptFile(path, encryptedFilePath))
                {
                    throw new FileLoadException($"Can not load settings file: {encryptedFilePath}");
                }
                else
                {
                    CryptoUtils.RemoveReadonlyAttribute(path);
                    File.Delete(path);
                }
            }

            if (File.Exists(encryptedFilePath))
            {
                var source = new CryptoJsonConfigurationSource
                {
                    FileProvider = null,
                    Path = encryptedFilePath,
                    Optional = false,
                    ReloadOnChange = reloadOnChange,
                };

                //source.ResolveFileProvider();

                builder.Add(source);
            }

            return builder;
        }
    }
}
