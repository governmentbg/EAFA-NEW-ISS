using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Domain.Enums;
using TechnoLogica.Xamarin.Converters.Base;
using TechnoLogica.Xamarin.Extensions;
using TechnoLogica.Xamarin.Helpers;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Converters
{
    public class FishingGearCheckTypeToViewConverter : BaseValueConverter<View, InspectedFishingGearEnum?>
    {
        public FishingGearCheckTypeToViewConverter()
            : base(false) { }

        public override View ConvertTo(InspectedFishingGearEnum? value)
        {
            return CreateView(value);
        }

        private View CreateView(InspectedFishingGearEnum? value)
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
                        iconColor = Color.Red;
                        iconText = IconFont.Xmark;
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
