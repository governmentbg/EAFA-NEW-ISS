using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Domain.Enums;
using System.Globalization;
using System;
using TechnoLogica.Xamarin.Converters.Base;
using TechnoLogica.Xamarin.Extensions;
using TechnoLogica.Xamarin.Helpers;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Converters
{
    public class FishingGearCheckTypeToViewConverter : BindableObject, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is InspectedFishingGearEnum gearEnum && parameter is string boolean && bool.TryParse(boolean, out bool ShowRedX))
            {
                return CreateView(gearEnum, ShowRedX);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private View CreateView(InspectedFishingGearEnum? value, bool ShowRedX)
        {
            Color iconColor = Color.Default;
            string iconText = null;
            string translateResource = null;

            if (value.HasValue)
            {
                switch (value.Value)
                {
                    case InspectedFishingGearEnum.Y:
                        iconColor = Color.Green;
                        iconText = IconFont.Check;
                        translateResource = "Coincides";
                        break;
                    case InspectedFishingGearEnum.N:
                        iconColor = Color.Red;
                        iconText = IconFont.NotEqual;
                        translateResource = "DoesNotCoincide";
                        break;
                    case InspectedFishingGearEnum.R:
                        iconColor = Color.Gray;
                        iconText = IconFont.EyeSlash;
                        translateResource = "Unavailable";
                        break;
                    case InspectedFishingGearEnum.I:
                        iconColor = ShowRedX ? Color.Red : Color.Gray;
                        iconText = ShowRedX ? IconFont.Xmark : IconFont.Book;
                        translateResource = "Unregistered";
                        break;
                }
            }

            if (translateResource == null)
            {
                iconColor = Color.Gray;
                iconText = IconFont.Clock;
                translateResource = "NotChecked";
            }

            return new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    new Label
                    {
                        FontFamily = "FA",
                        FontSize = 24,
                        Text = iconText,
                        TextColor = iconColor,
                    },
                    new Label
                    {
                        LineBreakMode = LineBreakMode.WordWrap,
                    }.BindTranslation(Label.TextProperty, translateResource, nameof(GroupResourceEnum.FishingGear))
                }
            };
        }
    }
}
