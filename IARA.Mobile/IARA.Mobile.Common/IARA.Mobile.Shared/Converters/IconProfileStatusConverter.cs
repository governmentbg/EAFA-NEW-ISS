using TechnoLogica.Xamarin.Converters.Base;
using TechnoLogica.Xamarin.Helpers;

namespace IARA.Mobile.Shared.Converters
{
    public class IconProfileStatusConverter : BaseValueConverter<string, string>
    {
        public override string ConvertTo(string value)
        {
            return value switch
            {
                "Approved" => IconFont.Check,
                "Blocked" => IconFont.Xmark,
                "Requested" => IconFont.Clock,
                _ => string.Empty,
            };
        }
    }
}
