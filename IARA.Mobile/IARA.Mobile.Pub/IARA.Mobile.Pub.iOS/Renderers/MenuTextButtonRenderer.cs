using CoreGraphics;
using IARA.Mobile.Pub.iOS.Renderers;
using IARA.Mobile.Shared.Views;
using TechnoLogica.Xamarin.Commands;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TLMenuTextButton), typeof(MenuTextButtonRenderer))]

namespace IARA.Mobile.Pub.iOS.Renderers
{
    public class MenuTextButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
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

            (Element as TLMenuTextButton)?.OnViewTouch(globalLocation.X, globalLocation.Y);
        }

        public CGPoint GetCoordinates(VisualElement element)
        {
            IVisualElementRenderer renderer = Platform.GetRenderer(element);
            UIView nativeView = renderer.NativeView;
            return nativeView.Superview.ConvertPointToView(nativeView.Frame.Location, null);
        }
    }
}
