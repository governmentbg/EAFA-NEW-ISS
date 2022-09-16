using System;

namespace IARA.Mobile.Application.Extensions
{
    public static class EnumExtensions
    {
        public static TEnum Parse<TEnum>(string value) where TEnum : Enum
        {
            return (TEnum)Enum.Parse(typeof(TEnum), value);
        }
    }
}
