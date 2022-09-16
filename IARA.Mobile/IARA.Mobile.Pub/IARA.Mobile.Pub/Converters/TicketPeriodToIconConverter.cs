using TechnoLogica.Xamarin.Converters.Base;
using TechnoLogica.Xamarin.Helpers;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.Converters
{
    public class TicketPeriodToIconConverter : BaseValueConverter<ImageSource, string>
    {
        public override ImageSource ConvertTo(string periodCode)
        {
            switch (periodCode)
            {
                case "ANNUAL":
                    return FromFA(IconFont.CalendarCheck);
                case "HALFYEARLY":
                    return FromFA(IconFont.Calendar);
                case "MONTHLY":
                    return FromFA(IconFont.CalendarDays);
                case "WEEKLY":
                    return FromFA(IconFont.CalendarWeek);
                default:
                    return FromFA(IconFont.Calendar);
            }
        }

        private ImageSource FromFA(string iconFont)
        {
            return new FontImageSource
            {
                FontFamily = "FA",
                Glyph = iconFont,
                Size = 100,
                Color = Color.White
            };
        }
    }
}
