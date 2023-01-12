using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Shared.ResourceTranslator;

namespace IARA.Mobile.Shared.Helpers
{
    public static class ResourceHelper
    {
        public static void LoadOfflineResources(this Translator translator)
        {
            translator.HardClear();

            Type[] resourceTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(f => f.IsClass && f.Namespace == "IARA.Mobile.Shared.OfflineResources")
                .ToArray();

            Dictionary<GroupResourceEnum, IReadOnlyDictionary<string, string>> groupTranslations = new Dictionary<GroupResourceEnum, IReadOnlyDictionary<string, string>>();

            foreach (Type resourceType in resourceTypes)
            {
                GroupResourceEnum group = (GroupResourceEnum)Enum.Parse(typeof(GroupResourceEnum), resourceType.Name.Replace("Resources", string.Empty));

                IEnumerable<DictionaryEntry> resources = new ResourceManager(resourceType)
                       .GetResourceSet(CultureInfo.CurrentUICulture, true, true)
                       .Cast<DictionaryEntry>();

                Dictionary<string, string> translations = new Dictionary<string, string>();

                foreach (DictionaryEntry resource in resources)
                {
                    translations.Add(resource.Key.ToString(), resource.Value.ToString());
                }

                groupTranslations.Add(group, translations);
            }

            translator.Add(groupTranslations);
        }
    }
}
