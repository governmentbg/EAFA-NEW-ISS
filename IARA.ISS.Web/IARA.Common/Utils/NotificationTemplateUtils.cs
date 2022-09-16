using System.Collections.Generic;
using System.Text;
using IARA.Common.Enums;

namespace IARA.Common.Utils
{
    public static class NotificationTemplateUtils
    {
        public static string BuildNotification(string template, Dictionary<NotificationTemplateCodes, string> emailTemplateCodes)
        {
            StringBuilder builder = new StringBuilder(template);

            foreach (var parameter in emailTemplateCodes)
            {
                string code = $"[{parameter.Key}]";
                if (template.Contains(code))
                {
                    builder.Replace(code, parameter.Value);
                }
            }

            return builder.ToString();
        }

        public static string BuildNotification<T>(string template, T model)
        {
            StringBuilder builder = new StringBuilder(template);

            foreach (var propertyInfo in model.GetType().GetProperties())
            {
                string code = $"[{propertyInfo.Name}]";
                if (template.Contains(code))
                {
                    object value = propertyInfo.GetValue(model);
                    string strValue = value != null ? value.ToString() : string.Empty;

                    builder.Replace(code, strValue);
                }
            }

            return builder.ToString();
        }
    }
}
