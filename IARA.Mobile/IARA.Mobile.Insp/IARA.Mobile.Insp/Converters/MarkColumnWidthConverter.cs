using System;
using System.Globalization;
using IARA.Mobile.Insp.Domain.Enums;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Converters
{
    public class MarkColumnWidthConverter : BindableObject, IValueConverter
    {
        public static readonly BindableProperty HasMoveMarkProperty =
            BindableProperty.Create(nameof(HasMoveMark), typeof(bool), typeof(MarkColumnWidthConverter), false);

        public bool HasMoveMark
        {
            get => (bool)GetValue(HasMoveMarkProperty);
            set => SetValue(HasMoveMarkProperty, value);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (HasMoveMark)
            {
                return new GridLength(40);
            }

            return new GridLength((ViewActivityType)value == ViewActivityType.Review ? 0 : 40);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
