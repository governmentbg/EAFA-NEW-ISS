using System;
using System.Globalization;
using Xamarin.Forms;

namespace IARA.Mobile.Shared.Converters
{
    public class AnyTrueMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || !targetType.IsAssignableFrom(typeof(bool)))
            {
                throw new ArgumentException();
            }

            foreach (object value in values)
            {
                if (value != null && bool.TryParse(value.ToString(), out bool result) && result)
                {
                    return true;
                }
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
