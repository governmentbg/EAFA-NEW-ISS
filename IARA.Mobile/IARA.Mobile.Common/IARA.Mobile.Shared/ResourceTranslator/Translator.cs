using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using IARA.Mobile.Domain.Enums;
using TechnoLogica.Xamarin.ResourceTranslator;

namespace IARA.Mobile.Shared.ResourceTranslator
{
    public sealed class Translator : ITranslator
    {
        private readonly Dictionary<GroupResourceEnum, IReadOnlyDictionary<string, string>> resources;
        private readonly List<GroupResourceEnum> safePageResources;

        private Translator()
        {
            resources = new Dictionary<GroupResourceEnum, IReadOnlyDictionary<string, string>>();
            safePageResources = new List<GroupResourceEnum>();
        }

        /// <summary>
        /// The active instance of the translator class
        /// </summary>
        public static Translator Current { get; } = new Translator();

        string ITranslator.this[string groupResource] => GetValue(groupResource);

        public event PropertyChangedEventHandler PropertyChanged;

        public List<GroupResourceEnum> GetResourceGroups()
        {
            return resources.Select(f => f.Key).ToList();
        }

        /// <summary>
        /// Returns only the filters that don't exist in the dictionary
        /// </summary>
        public IEnumerable<GroupResourceEnum> Filter(GroupResourceEnum[] groups)
        {
            foreach (GroupResourceEnum group in groups)
            {
                if (!resources.ContainsKey(group))
                {
                    yield return group;
                }
            }
        }

        /// <summary>
        /// Adds the given resources to the dictionary
        /// </summary>
        public void Add(IReadOnlyDictionary<GroupResourceEnum, IReadOnlyDictionary<string, string>> groupResources)
        {
            foreach (KeyValuePair<GroupResourceEnum, IReadOnlyDictionary<string, string>> resource in groupResources)
            {
                if (resources.ContainsKey(resource.Key))
                {
                    resources.Remove(resource.Key);
                }

                resources.Add(resource.Key, resource.Value);
            }
        }

        /// <summary>
        /// Removes the pages from the resources dictionary
        /// </summary>
        /// <param name="groups">The pages to remove</param>
        public void Remove(GroupResourceEnum[] groups)
        {
            foreach (GroupResourceEnum group in groups)
            {
                resources.Remove(group);
            }
        }

        /// <summary>
        /// Removes all resources except those which are common or used for the menu
        /// </summary>
        public void SoftClear()
        {
            List<GroupResourceEnum> toRemove = resources
                .Where(f => !safePageResources.Contains(f.Key))
                .Select(pair => pair.Key)
                .ToList();

            foreach (GroupResourceEnum key in toRemove)
            {
                resources.Remove(key);
            }
        }

        /// <summary>
        /// Removes every single resource from the dictionary
        /// </summary>
        public void HardClear()
        {
            resources.Clear();
        }

        /// <summary>
        /// Forces all translations to reset (used for when the language is changed)
        /// </summary>
        public void Invalidate()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }

        /// <summary>
        /// Set the resources that won't get removed on soft clear
        /// </summary>
        public void SafeResources(GroupResourceEnum[] safePageResources)
        {
            this.safePageResources.Clear();

            for (int i = 0; i < safePageResources.Length; i++)
            {
                this.safePageResources.Add(safePageResources[i]);
            }
        }

        private string GetValue(string groupResource)
        {
            string[] split = groupResource.Split('/');

            if (!Enum.TryParse(split[0], out GroupResourceEnum group))
            {
                return groupResource;
            }

            return GetValue(group, split[1]);
        }

        private string GetValue(GroupResourceEnum group, string resource)
        {
            if (!resources.TryGetValue(group, out IReadOnlyDictionary<string, string> groupResources))
            {
                return $"{group}/{resource}";
            }

            if (!groupResources.TryGetValue(resource, out string result))
            {
                return $"{group}/{resource}";
            }

            return result;
        }
    }
}
