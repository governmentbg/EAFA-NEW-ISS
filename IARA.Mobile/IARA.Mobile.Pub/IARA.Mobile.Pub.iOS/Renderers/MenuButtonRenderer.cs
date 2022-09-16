using CoreGraphics;
using IARA.Mobile.Pub.iOS.Renderers;
using IARA.Mobile.Shared.Views;
using TechnoLogica.Xamarin.Commands;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TLMenuButton), typeof(MenuButtonRenderer))]

namespace IARA.Mobile.Pub.iOS.Renderers
{
    public class MenuButtonRenderer : ImageButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ImageButton> e)
        {
            base.OnElementChanged(e);

            if (e?.NewElement != null)
            {
                e.NewElement.Command = CommandBuilder.CreateFrom(OnClick);
            }
        }

        private void OnClick()
        {
            CGPoint globalLocation = GetCoordinates(Element);

            (Element as TLMenuButton)?.OnViewTouch(globalLocation.X, globalLocation.Y);
        }

        public CGPoint GetCoordinates(VisualElement element)
        {
            IVisualElementRenderer renderer = Platform.GetRenderer(element);
            UIView nativeView = renderer.NativeView;
            return nativeView.Superview.ConvertPointToView(nativeView.Frame.Location, null);
        }
    }
}
