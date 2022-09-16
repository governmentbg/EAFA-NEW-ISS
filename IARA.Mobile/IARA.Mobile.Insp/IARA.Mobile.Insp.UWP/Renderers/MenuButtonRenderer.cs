using IARA.Mobile.Insp.UWP.Renderers;
using IARA.Mobile.Shared.Views;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using WPoint = Windows.Foundation.Point;

[assembly: ExportRenderer(typeof(TLMenuButton), typeof(MenuButtonRenderer))]

namespace IARA.Mobile.Insp.UWP.Renderers
{
    public class MenuButtonRenderer : ImageButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ImageButton> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.Style = Windows.UI.Xaml.Application.Current.Resources["ButtonStyle"] as Windows.UI.Xaml.Style;
            }

            if (e?.NewElement != null)
            {
                Element.Command = new Command(OnClick);
            }
        }

        private void OnClick()
        {
            GeneralTransform ttv = Control.TransformToVisual(Window.Current.Content);
            WPoint screenCoords = ttv.TransformPoint(new WPoint(0, 0));

            (Element as TLMenuButton).OnViewTouch(screenCoords.X, screenCoords.Y);
        }
    }
}
