using IARA.Mobile.Pub.Domain.Enums;
using System;
using System.Globalization;
using TechnoLogica.Xamarin.Converters.Base;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.Converters
{

    public class TicketValidToConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (object value in values)
            {
                if (value is null)
                {
                    return string.Empty;
                }
            }

            string periodCode = values[0].ToString();
            if (periodCode == nameof(TicketPeriodEnum.NOPERIOD))
            {
                return "∞";
            }

            DateTime date = System.Convert.ToDateTime(values[1]);
            return date.ToString("g", CultureInfo.CurrentUICulture);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}