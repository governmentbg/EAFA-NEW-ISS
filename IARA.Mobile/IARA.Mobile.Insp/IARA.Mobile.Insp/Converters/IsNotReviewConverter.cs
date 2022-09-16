using IARA.Mobile.Insp.Domain.Enums;
using TechnoLogica.Xamarin.Converters.Base;

namespace IARA.Mobile.Insp.Converters
{
    public class IsNotReviewConverter : BaseValueConverter<bool, ViewActivityType>
    {
        public IsNotReviewConverter()
        {
            ConvertDefaultReturn = true;
        }

        public override bool ConvertTo(ViewActivityType value)
        {
            return !IsReviewConverter.ConvertInternal(value);
        }
    }
}
