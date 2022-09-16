using IARA.Mobile.Insp.Domain.Enums;
using TechnoLogica.Xamarin.Converters.Base;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Converters
{
    public class ReviewToWidthConverter : BaseValueConverter<GridLength, ViewActivityType>
    {
        public override GridLength ConvertTo(ViewActivityType value, string parameter)
        {
            return new GridLength(value == ViewActivityType.Review ? 0 : double.Parse(parameter));
        }
    }
}
