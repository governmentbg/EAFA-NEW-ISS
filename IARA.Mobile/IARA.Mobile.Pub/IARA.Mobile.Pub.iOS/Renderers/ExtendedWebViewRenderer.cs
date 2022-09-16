//using Foundation;
//using IARA.Mobile.Pub.iOS.Renderers;
//using IARA.Mobile.Pub.Views.Controls;
//using System;
//using WebKit;
//using Xamarin.Forms;
//using Xamarin.Forms.Platform.iOS;

//[assembly: ExportRenderer(typeof(ExtendedWebView), typeof(ExtendedWebViewRenderer))]
//namespace IARA.Mobile.Pub.iOS.Renderers
//{
//    public class ExtendedWebViewRenderer : WkWebViewRenderer
//    {
//        protected override void OnElementChanged(VisualElementChangedEventArgs e)
//        {
//            try
//            {
//                base.OnElementChanged(e);
//                NavigationDelegate = new AppWKNavigationDelegate(this);
//            }
//            catch (Exception ex)
//            {

//            }
//        }
//    }

//    public class AppWKNavigationDelegate : WKNavigationDelegate
//    {
//        ExtendedWebViewRenderer extendedWebViewRenderer;
//        public AppWKNavigationDelegate(ExtendedWebViewRenderer _extendedWebViewRenderer)
//        {

//            extendedWebViewRenderer = _extendedWebViewRenderer ?? new ExtendedWebViewRenderer();
//        }
//        public override async void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
//        {
//            try
//            {
//                var extendedWebViewModel = extendedWebViewRenderer.Element as ExtendedWebView;
//                if (extendedWebViewModel != null)
//                {
//                    if (webView != null)
//                    {
//                        await System.Threading.Tasks.Task.Delay(100); // wait here till content is rendered
//                        if (webView.ScrollView != null)
//                        {
//                            if (webView.ScrollView.ContentSize != null)
//                            {
//                                extendedWebViewModel.HeightRequest = (double)webView.ScrollView.ContentSize.Height;
//                            }
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {

//            }
//        }
//    }
//}