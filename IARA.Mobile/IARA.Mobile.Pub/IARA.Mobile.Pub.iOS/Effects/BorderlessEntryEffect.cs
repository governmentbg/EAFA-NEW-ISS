using IARA.Mobile.Pub.iOS.Effects;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(BorderlessEntryEffect), nameof(BorderlessEntryEffect))]

namespace IARA.Mobile.Pub.iOS.Effects
{
    public class BorderlessEntryEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            Control.Layer.BorderWidth = 0;
            (Control as UITextField).BorderStyle = UITextBorderStyle.None;
        }

        protected override void OnDetached()
        {
        }
    }
}
