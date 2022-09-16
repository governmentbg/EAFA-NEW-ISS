using System;
using System.Globalization;
using Xamarin.Forms;

namespace IARA.Mobile.Shared.Converters
{
    public class ValueColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (object value in values)
            {
                if (value is null)
                {
                    return (Color)Xamarin.Forms.Application.Current.Resources["Primary"];
                }
            }

            return (int)values[0] == (int)values[1] ? (Color)Xamarin.Forms.Application.Current.Resources["PrimaryDark"] : (Color)Xamarin.Forms.Application.Current.Resources["Primary"];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
