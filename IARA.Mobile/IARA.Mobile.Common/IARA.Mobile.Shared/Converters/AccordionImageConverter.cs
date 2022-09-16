using TechnoLogica.Xamarin.Converters.Base;
using TechnoLogica.Xamarin.Helpers;

namespace IARA.Mobile.Shared.Converters
{
    public class AccordionImageConverter : BaseValueConverter<string, bool>
    {
        public override string ConvertTo(bool value)
        {
            return value
                ? IconFont.ChevronUp
                : IconFont.ChevronDown;
        }
    }
}
