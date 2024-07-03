using TechnoLogica.Xamarin.Converters.Base;

namespace IARA.Mobile.Insp.Converters
{
    public class IsNotEmptyOrNotNullConverter : BaseValueConverter<bool, string>
    {
        public override bool ConvertTo(string value)
        {
            return !string.IsNullOrEmpty(value);
        }
    }
}
