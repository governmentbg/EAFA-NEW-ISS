using System;
using System.Globalization;
using IARA.Mobile.Pub.Domain.Enums;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.Converters
{

    public class TicketNameConverter : IMultiValueConverter
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
            string typeName = values[1].ToString();
            string periodName = values[2].ToString();
            if (periodCode == nameof(TicketPeriodEnum.UNTIL14))
            {
                return typeName;
            }

            return typeName + " - " + periodName;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
