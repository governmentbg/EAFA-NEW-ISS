using TechnoLogica.Xamarin.Converters.Base;

namespace IARA.Mobile.Shared.Converters
{
    public class NotEqualToConverter : BaseValueConverter<bool, int>
    {
        public override bool ConvertTo(int value, string parameter)
        {
            return value != int.Parse(parameter);
        }
    }
}
