using System;
using System.Collections.Generic;

namespace IARA.Mobile.Pub.Extensions
{
    public static class DictionaryExtensions
    {
        public static bool TryGetEnumValue<TEnum>(this IDictionary<string, object> dict, string key, out TEnum value)
             where TEnum : struct
        {
            value = default;
            return dict.TryGetValue(key, out object valueObj)
                && Enum.TryParse(valueObj.ToString(), out value);
        }

        public static bool TryGetIntValue(this IDictionary<string, object> dict, string key, out int value)
        {
            value = default;
            return dict.TryGetValue(key, out object valueObj) && int.TryParse(valueObj?.ToString(), out value);
        }

        public static bool TryGetSplitValue(this IDictionary<string, object> dict, string key, out string[] value)
        {
            if (dict.TryGetValue(key, out object valueObj) && valueObj != null)
            {
                value = valueObj.ToString().Split(',');
                return true;
            }

            value = null;
            return false;
        }

        public static string GetValueIfKeyContains(this IDictionary<string, object> dict, string key)
        {
            foreach (KeyValuePair<string, object> item in dict)
            {
                if (item.Key.Contains(key))
                {
                    return item.Value.ToString();
                }
            }

            return null;
        }
    }
}
