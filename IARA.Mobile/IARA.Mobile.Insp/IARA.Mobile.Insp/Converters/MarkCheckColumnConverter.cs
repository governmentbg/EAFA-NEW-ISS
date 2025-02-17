using System;
using System.Globalization;
using IARA.Mobile.Insp.Domain.Enums;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Converters
{
    public class MarkCheckColumnConverter : BindableObject, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ViewActivityType viewActivity && viewActivity == ViewActivityType.Review)
            {
                return 3;
            }
            return 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
