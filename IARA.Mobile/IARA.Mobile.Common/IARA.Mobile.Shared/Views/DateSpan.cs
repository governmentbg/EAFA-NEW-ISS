using System;
using System.Globalization;
using TechnoLogica.Xamarin.Helpers;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;

namespace IARA.Mobile.Shared.Views
{
    public class DateSpan : Span
    {
        public static readonly BindableProperty DateProperty =
            BindableProperty.Create(nameof(Date), typeof(DateTime?), typeof(Label));

        private static readonly DateTime MaxValue = new DateTime(9999, 1, 1);

        public DateSpan()
        {
            SetBinding(TextProperty, new Binding
            {
                Source = this,
                Path = DateProperty.PropertyName,
                Mode = BindingMode.OneWay,
                Converter = new FuncConverter<DateTime?, string>((date) =>
                {
                    if (date.HasValue)
                    {
                        if (date.Value.Date == MaxValue)
                        {
                            FontFamily = "FA";
                            return IconFont.Infinity;
                        }

                        FontFamily = null;
                        return date.Value.ToString("d", CultureInfo.CurrentUICulture);
                    }

                    return string.Empty;
                })
            });
        }

        public DateTime? Date
        {
            get => (DateTime?)GetValue(DateProperty);
            set => SetValue(DateProperty, value);
        }
    }
}
