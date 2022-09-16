using IARA.Mobile.Insp.Droid.Effects;
using TechnoLogica.Xamarin.Effects.Base;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName(BaseEffect.GroupName)]
[assembly: ExportEffect(typeof(BorderlessEntryEffect), nameof(BorderlessEntryEffect))]

namespace IARA.Mobile.Insp.Droid.Effects
{
    public class BorderlessEntryEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            Control.Background = null;
        }

        protected override void OnDetached()
        {
        }
    }
}
