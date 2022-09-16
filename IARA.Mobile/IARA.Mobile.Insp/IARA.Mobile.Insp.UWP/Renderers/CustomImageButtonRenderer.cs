using IARA.Mobile.Insp.UWP.Renderers;
using Windows.UI.Xaml;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(Xamarin.Forms.ImageButton), typeof(CustomImageButtonRenderer))]

namespace IARA.Mobile.Insp.UWP.Renderers
{
    public class CustomImageButtonRenderer : ImageButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ImageButton> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.Style = Windows.UI.Xaml.Application.Current.Resources["ButtonStyle"] as Style;
            }
        }
    }
}
