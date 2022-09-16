using System;
using System.Globalization;
using TechnoLogica.Xamarin.Helpers;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.Converters
{
    public class NewsImageSourceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
            {
                throw new ArgumentNullException();
            }

            if (values[0] != null && values[1] != null && values[2] != null)
            {
                if (bool.TryParse(values[0].ToString(), out bool result) && result)
                {
                    string id = values[1].ToString();
                    string url = values[2].ToString();
                    return url + id;
                }
            }

            return new FontImageSource { Size = 280, Glyph = IconFont.Image, Color = Color.White, FontFamily = "FA" };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
