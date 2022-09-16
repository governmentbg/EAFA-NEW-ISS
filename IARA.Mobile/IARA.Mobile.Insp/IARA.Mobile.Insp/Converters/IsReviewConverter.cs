using IARA.Mobile.Insp.Domain.Enums;
using TechnoLogica.Xamarin.Converters.Base;

namespace IARA.Mobile.Insp.Converters
{
    public class IsReviewConverter : BaseValueConverter<bool, ViewActivityType>
    {
        public IsReviewConverter()
        {
            ConvertDefaultReturn = true;
        }

        public override bool ConvertTo(ViewActivityType value)
        {
            return ConvertInternal(value);
        }

        public static bool ConvertInternal(ViewActivityType value)
        {
            return value == ViewActivityType.Review;
        }
    }
}
