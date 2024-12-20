using System;
using System.Collections;
using System.Globalization;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Converters
{
    public class ErrorColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool showError)
            {
                return showError ? Color.Red : Color.DimGray;
            }
            return Color.DimGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
