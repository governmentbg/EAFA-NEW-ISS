using CoreGraphics;
using Foundation;
using IARA.Mobile.Pub.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Page), typeof(KeyboardFixPageRenderer))]

namespace IARA.Mobile.Pub.iOS.Renderers
{
    public class KeyboardFixPageRenderer : PageRenderer
    {
        private NSObject observerHideKeyboard;
        private NSObject observerShowKeyboard;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (Element is ContentPage)
            {
                foreach (UIGestureRecognizer g in View.GestureRecognizers)
                {
                    g.CancelsTouchesInView = false;
                }
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            observerHideKeyboard = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
            observerShowKeyboard = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            NSNotificationCenter.DefaultCenter.RemoveObserver(observerHideKeyboard);
            NSNotificationCenter.DefaultCenter.RemoveObserver(observerShowKeyboard);
        }

        private void OnKeyboardNotification(NSNotification notification)
        {
            try
            {
                if (!IsViewLoaded)
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            CGRect frameBegin = UIKeyboard.FrameBeginFromNotification(notification);
            CGRect frameEnd = UIKeyboard.FrameEndFromNotification(notification);

            if (Element is ContentPage page && !(page.Content is ScrollView))
            {
                Thickness padding = page.Padding;
                try
                {
                    page.Padding = new Thickness(padding.Left, padding.Top, padding.Right, padding.Bottom + frameBegin.Top - frameEnd.Top);
                }
                catch
                {
                    // Fail silently
                }
            }
        }
    }
}