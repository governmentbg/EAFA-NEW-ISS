using System.Collections.Generic;
using TechnoLogica.Xamarin.Converters.Base;

namespace IARA.Mobile.Pub.Converters
{
    public class ListContainsConverter : BaseValueConverter<bool, List<string>>
    {
        public override bool ConvertTo(List<string> value, string parameter)
        {
            return value?.Exists(f => f == parameter) == true;
        }
    }
}
