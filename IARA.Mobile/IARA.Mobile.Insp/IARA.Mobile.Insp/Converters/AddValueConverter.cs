using TechnoLogica.Xamarin.Converters.Base;

namespace IARA.Mobile.Insp.Converters
{
    public class AddValueConverter : BaseValueConverter<double, double>
    {
        public override double ConvertTo(double value, string parameter)
        {
            return value + double.Parse(parameter);
        }
    }
}
