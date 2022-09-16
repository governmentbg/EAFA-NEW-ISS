using TechnoLogica.Xamarin.Converters.Base;
using TechnoLogica.Xamarin.Helpers;

namespace IARA.Mobile.Shared.Converters
{
    public class IconProfileStatusConverter : BaseValueConverter<string, string>
    {
        public override string ConvertTo(string value)
        {
            switch (value)
            {
                case "Approved":
                    return IconFont.Check;
                case "Blocked":
                    return IconFont.Xmark;
                case "Requested":
                    return IconFont.Clock;
                default:
                    return string.Empty;
            }
        }
    }
}
