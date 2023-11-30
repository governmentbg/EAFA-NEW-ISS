using Microsoft.Extensions.Configuration.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TechnoLogica.IdentityServer
{
    public class WritableJsonConfigurationProvider : JsonConfigurationProvider
    {
        public WritableJsonConfigurationProvider(JsonConfigurationSource source) : base(source)
        {
        }

        public override void Set(string key, string value)
        {
            base.Set(key, value);

            //Get Whole json file and change only passed key with passed value. It requires modification if you need to support change multi level json structure
            var fileFullPath = base.Source.FileProvider.GetFileInfo(base.Source.Path).PhysicalPath;
            dynamic jsonObj = new object();
            if (File.Exists(fileFullPath))
            {
                string json = File.ReadAllText(fileFullPath);
                jsonObj = JsonConvert.DeserializeObject(json);
            }
            var propertyToSet = jsonObj;
            var splittedKey = key.Split(':');
            for (int i = 0; i < splittedKey.Length; i++)
            {
                if (i == splittedKey.Length - 1)
                {
                    propertyToSet[splittedKey[i]] = value;
                }
                else
                {
                    propertyToSet = propertyToSet[splittedKey[i]];
                }
            }
            string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            File.WriteAllText(fileFullPath, output);
        }

        public void Set<T>(string key, T value)
            where T : class
        {
            //Get Whole json file and change only passed key with passed value. It requires modification if you need to support change multi level json structure
            var fileFullPath = base.Source.FileProvider.GetFileInfo(base.Source.Path).PhysicalPath;
            dynamic jsonObj = new object();
            if (File.Exists(fileFullPath))
            {
                string json = File.ReadAllText(fileFullPath);
                jsonObj = JsonConvert.DeserializeObject(json);
            }
            jsonObj[key] = value;
            string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            File.WriteAllText(fileFullPath, output);
        }
    }
}
