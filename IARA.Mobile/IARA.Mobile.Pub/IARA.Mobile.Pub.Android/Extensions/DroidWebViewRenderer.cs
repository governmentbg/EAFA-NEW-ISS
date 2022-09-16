using Android.Content;
using Android.Views;
using IARA.Mobile.Pub.Droid.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(WebView), typeof(DroidWebViewRenderer))]
namespace IARA.Mobile.Pub.Droid.Extensions
{
    public class DroidWebViewRenderer : WebViewRenderer
    {
        public DroidWebViewRenderer(Context context) : base(context)
        {
        }

        public override bool DispatchTouchEvent(MotionEvent e)
        {
            Parent.RequestDisallowInterceptTouchEvent(true);
            return base.DispatchTouchEvent(e);
        }


        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.WebView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                Control.SetWebViewClient(new MyFormsWebViewClient(this));
            }
        }

        internal class MyFormsWebViewClient : FormsWebViewClient
        {
            DroidWebViewRenderer _renderer;

            public MyFormsWebViewClient(DroidWebViewRenderer renderer) : base(renderer)
            {
                _renderer = renderer;
            }

            public override void OnReceivedSslError(Android.Webkit.WebView view, Android.Webkit.SslErrorHandler handler, Android.Net.Http.SslError error)
            {
                handler.Proceed();
            }

            public override void OnPageFinished(Android.Webkit.WebView view, string url)
            {
                base.OnPageFinished(view, url);
            }

            public override void OnLoadResource(Android.Webkit.WebView view, string url)
            {
                base.OnLoadResource(view, url);
            }
        }
    }
}