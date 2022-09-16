using IARA.Mobile.Insp.Domain.Enums;
using TechnoLogica.Xamarin.Converters.Base;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Converters
{
    public class ReviewDialogToWidthConverter : BaseValueConverter<GridLength, ViewActivityType>
    {
        public override GridLength ConvertTo(ViewActivityType value, string parameter)
        {
            return new GridLength(value == ViewActivityType.Review ? 45d : double.Parse(parameter));
        }
    }
}
