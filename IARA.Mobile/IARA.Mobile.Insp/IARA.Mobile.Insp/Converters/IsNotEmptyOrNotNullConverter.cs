using TechnoLogica.Xamarin.Converters.Base;

namespace IARA.Mobile.Insp.Converters
{
    public class IsNotEmptyOrNotNullConverter : BaseValueConverter<bool, object>
    {
        public override bool ConvertTo(object value)
        {
            if (value is string stringValue)
            {
                return !string.IsNullOrEmpty(stringValue);
            }
            else
            {
                return value != null;
            }
        }
    }
}
