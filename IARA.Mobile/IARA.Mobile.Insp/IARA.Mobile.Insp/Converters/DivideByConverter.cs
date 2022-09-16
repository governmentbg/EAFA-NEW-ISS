using TechnoLogica.Xamarin.Converters.Base;

namespace IARA.Mobile.Insp.Converters
{
    public class DivideByConverter : BaseValueConverter<long, long>
    {
        public override long ConvertTo(long value, string parameter)
        {
            return value / int.Parse(parameter);
        }
    }
}
