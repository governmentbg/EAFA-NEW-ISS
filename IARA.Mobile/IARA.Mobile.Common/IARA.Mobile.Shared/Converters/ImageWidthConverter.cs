using TechnoLogica.Xamarin.Converters.Base;
using Xamarin.Forms;

namespace IARA.Mobile.Shared.Converters
{
    internal class ImageWidthConverter : BaseValueConverter<double, ImageSource>
    {
        public override double ConvertTo(ImageSource value)
        {
            return value != null ? 45d : 0d;
        }
    }
}
