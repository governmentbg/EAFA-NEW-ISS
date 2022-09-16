using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace IARA.Common.Utils
{
    public static class CommonExtensions
    {
        public static string GetTableName(this Type t)
        {
            TableAttribute tableAttr = t.GetCustomAttribute(typeof(TableAttribute)) as TableAttribute;
            return tableAttr?.Name;
        }

        public static string GetDbSetTableName(this Type t)
        {
            TableAttribute tableAttr = t.GetGenericArguments()[0].GetCustomAttribute(typeof(TableAttribute)) as TableAttribute;
            return tableAttr?.Name;
        }
    }
}
