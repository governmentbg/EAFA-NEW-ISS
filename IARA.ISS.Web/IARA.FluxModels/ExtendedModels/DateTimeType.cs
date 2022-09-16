using System;

namespace IARA.Flux.Models
{
    public partial class DateTimeType
    {
        public static DateTimeType BuildDateTime(DateTime dateTime)
        {
            return new DateTimeType
            {
                Item = dateTime.ToUniversalTime()
            };
            //return new DateTimeType
            //{
            //    Item = new DateTimeTypeDateTimeString
            //    {
            //        Value = dateTime.ToUniversalTime().ToString(DateTimeFormats.FLUX_UTC_DATETIME_FORMAT)
            //    }
            //};
        }

        public static implicit operator DateTimeType(DateTime value)
        {
            return BuildDateTime(value);
        }

        public static explicit operator DateTime(DateTimeType value)
        {
            //if (value.Item is DateTimeTypeDateTimeString)
            //{
            //    var dateTimeStr = value.Item as DateTimeTypeDateTimeString;

            //    return DateTime.ParseExact(dateTimeStr.Value, DateTimeFormats.FLUX_UTC_DATETIME_FORMAT, CultureInfo.InvariantCulture);
            //}
            //else
            //{
            //    return (DateTime)value.Item;
            //}

            return value.Item;
        }
    }
}
