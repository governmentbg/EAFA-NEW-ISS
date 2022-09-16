using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using IARA.Common.Attributes;

namespace IARA.DomainModels.RequestModels
{
    public abstract class BaseRequestModel
    {
        public string FreeTextSearch { get; set; }

        public bool ShowInactiveRecords { get; set; }

        public bool HasAnyFilters(bool excludeFreeTextSearch = true)
        {
            IEnumerable<PropertyInfo> properties = this.GetType().GetProperties();

            properties = properties.Where(x => x.Name != nameof(ShowInactiveRecords) && !Attribute.IsDefined(x, typeof(IgnoreAttribute)));

            if (excludeFreeTextSearch)
            {
                properties = properties.Where(x => x.Name != nameof(FreeTextSearch));
            }

            foreach (var propInfo in properties)
            {
                bool isDefaultValue = true;

                if (propInfo.PropertyType.IsClass)
                {
                    object value = propInfo.GetValue(this);
                    isDefaultValue = value == null;
                    if (propInfo.PropertyType == typeof(string))
                    {
                        isDefaultValue = isDefaultValue || (value as string) == string.Empty;
                    }
                }
                else if (propInfo.PropertyType.IsPrimitive || !propInfo.PropertyType.IsNotPublic)
                {
                    isDefaultValue = propInfo.GetValue(this) == Activator.CreateInstance(propInfo.PropertyType);
                }
                else if (propInfo.PropertyType.IsClass)
                {
                    isDefaultValue = propInfo.GetValue(this) == null;
                }

                if (!isDefaultValue)
                    return true;
            }

            return false;
        }

        public bool HasFreeTextSearch()
        {
            return !string.IsNullOrEmpty(this.FreeTextSearch);
        }
    }

    public static class FilterExtensions
    {
        public static bool HasValue<T>(this T model, Expression<Func<T, object>> propertyLambda)
        {
            MemberExpression member = propertyLambda.Body as MemberExpression;
            PropertyInfo propInfo = member.Member as PropertyInfo;

            if (propInfo.PropertyType == typeof(string))
            {
                return !string.IsNullOrEmpty(propInfo.GetValue(model) as string);
            }
            else if (propInfo.PropertyType.IsPrimitive || !propInfo.PropertyType.IsNotPublic)
            {
                return propInfo.GetValue(model) != Activator.CreateInstance(propInfo.PropertyType);
            }
            else
            {
                return false;
            }
        }
    }
}
