using System;
using System.Globalization;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.Converters
{

    public class StatusReasonVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (object value in values)
            {
                if (value is null)
                {
                    return false;
                }
            }

            string applicationStatusCode = values[0].ToString();
            string statusReason = values[1].ToString();
            if ((applicationStatusCode == ApplicationStatuses.CORR_BY_USR_NEEDED ||
                    applicationStatusCode == ApplicationStatuses.FILL_BY_APPL) &&
                    !string.IsNullOrEmpty(statusReason))
            {
                return true;
            }

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
