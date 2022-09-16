using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace IARA.Common.Utils
{
    public static class CommonUtils
    {
        public static string GenerateUserToken()
        {
            return Guid.NewGuid().ToString().Replace("-", "").ToUpper();
        }

        public static T[] GetSettingsArray<T>(this IConfiguration section, string propertyKey)
        {
            IConfigurationSection property = section.GetSection(propertyKey);
            IEnumerable<IConfigurationSection> children = property.GetChildren();

            return children.Select(x => Convert.ChangeType(x.Value, typeof(T))).Cast<T>().ToArray();
        }

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
            StringBuilder builder = new StringBuilder();
            foreach (byte item in array)
            {
                builder.Append(item.ToString("x2"));
            }

            return builder.ToString();
        }

        public static bool IsEntryActive(IDictionary<string, object> entry)
        {
            if (entry.ContainsKey("IsActive"))
            {
                return (bool)entry["IsActive"];
            }
            else if (entry.ContainsKey("ValidTo"))
            {
                DateTime validTo = (DateTime)entry["ValidTo"];

                if (entry.ContainsKey("ValidFrom"))
                {
                    DateTime validFom = (DateTime)entry["ValidFrom"];
                    return validTo > DateTime.Now && DateTime.Now > validFom;
                }
                else
                {
                    return validTo > DateTime.Now;
                }
            }
            else
            {
                return true;
            }
        }

        public static string Serialize<T>(T model)
        {
            JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            string json = JsonSerializer.Serialize<T>(model, options);

            return json;
        }

        public static string Serialize(object model, Type modelType)
        {
            JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            string json = JsonSerializer.Serialize(model, modelType, options);

            return json;
        }

        public static T Deserialize<T>(string json)
        {
            JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
            T model = JsonSerializer.Deserialize<T>(json, options);

            return model;
        }

    }
}
