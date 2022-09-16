using TechnoLogica.Xamarin.Converters.Base;

namespace IARA.Mobile.Shared.Converters
{
    public class StringNotEmptyConverter : BaseValueConverter<bool, string>
    {
        public override bool ConvertTo(string value)
        {
            return !string.IsNullOrEmpty(value);
        }
    }
}
