using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace IARA.Mobile.Insp.Helpers
{
    public class ReflectionHelper
    {
        public static List<PropertyInfo> GetAllRequiredValidStates(Type targetType)
        {
            PropertyInfo[] properties = targetType.GetProperties();
            List<PropertyInfo> validStateRequiredProperties = new List<PropertyInfo>();
            foreach (PropertyInfo property in properties)
            {
                if (property.GetCustomAttributes(typeof(RequiredAttribute)).Any())
                {
                    validStateRequiredProperties.Add(property);
                }
            }
            return validStateRequiredProperties;
        }
    }
}
