using TechnoLogica.Xamarin.Converters.Base;

namespace IARA.Mobile.Insp.Converters
{
    public class LastUrlSegmentConverter : BaseValueConverter<string, string>
    {
        public override string ConvertTo(string value)
        {
            string[] split = value.Split('/');
            return split[split.Length - 1];
        }
    }
}
