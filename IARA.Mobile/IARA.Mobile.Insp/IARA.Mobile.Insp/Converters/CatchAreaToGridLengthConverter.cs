using TechnoLogica.Xamarin.Converters.Base;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Converters
{
    public class CatchAreaToGridLengthConverter : BaseValueConverter<GridLength, bool>
    {
        public override GridLength ConvertTo(bool value)
        {
            return value ? GridLength.Star : new GridLength(0);
        }
    }
}
