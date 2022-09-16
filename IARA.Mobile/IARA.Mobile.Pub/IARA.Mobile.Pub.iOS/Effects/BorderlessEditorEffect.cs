using IARA.Mobile.Pub.iOS.Effects;
using TechnoLogica.Xamarin.Effects.Base;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName(BaseEffect.GroupName)]
[assembly: ExportEffect(typeof(BorderlessEditorEffect), nameof(BorderlessEditorEffect))]

namespace IARA.Mobile.Pub.iOS.Effects
{
    public class BorderlessEditorEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            Control.Layer.BorderWidth = 0;
        }

        protected override void OnDetached()
        {
        }
    }
}
