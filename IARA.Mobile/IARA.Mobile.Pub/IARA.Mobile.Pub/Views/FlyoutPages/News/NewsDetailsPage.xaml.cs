using IARA.Mobile.Pub.ViewModels.FlyoutPages.News;
using IARA.Mobile.Shared.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Pub.Views.FlyoutPages.News
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewsDetailsPage : BasePage<NewsDetailsViewModel>
    {
        private double width = 0;
        private double height = 0;
        public NewsDetailsPage(NewsDetailsViewModel news)
        {
            BindingContext = news;
            InitializeComponent();
            webView.Source = new HtmlWebViewSource
            {
                Html = BuildHtml(news),
            };
        }

        private string BuildHtml(NewsDetailsViewModel news)
        {
            return $@"<!DOCTYPE html>
                           <html lang=""en"">
                               <head>
                                   <style>

                                   .tl-content,.tl-auto{{margin-left:auto;margin-right:auto}}.tl-content{{max-width:980px}}.tl-auto{{max-width:1140px}}

                                   .tl-margin-top{{margin-top:16px!important}}

                                   .tl-image{{background-color: #f0f0f0;}}

                                   .tl-title {{font-size: 20px;font-weight: bold;}}

                                   .tl-time{{font-size: 12px;color:#aba9a6;}}

                                   .center {{
                                     display: block;
                                     margin-left: auto;
                                     margin-right: auto;
                                     width: 70%;
                                   }}
                                   </style>
                                   <meta charset=""UTF-8"">
                                   <meta name=""viewport"" content=""width=device-width, initial-scale=1.0, maximum-scale=2.0, minimum-scale=1.0, user-scalable=yes"" >
                               </head>
                               <body>
                                   <div class=""tl-content tl-margin-top"" id=""portfolio"">

                                         <div>
                                           <label class=""tl-title"">{news.Title}</label>
                                         <div>
  
                                         <div>
                                           <label class=""tl-time"">{news.PublishStart:yyyy-MM-dd HH\:mm}</label>
                                         </div> " +
                                     (string.IsNullOrEmpty(news.MainPhotoUrl) ? null : $@" <div class=""tl-image""><img src=""{news.MainPhotoUrl}"" class=""center"" alt=""News""></div>") +
                                     $@"{news.Content}
                                   </div>
                               </body>
                           </html>";
        }
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height); //must be called
            if (this.width != width || this.height != height)
            {
                this.width = width;
                this.height = height;
                NewsDetailsViewModel vm = (NewsDetailsViewModel)this.BindingContext;
                WebView wView = new WebView
                {
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Margin = new Thickness(5, 0),
                    Source = new HtmlWebViewSource
                    {
                        Html = BuildHtml(vm),
                    }
                };
                webView = wView;
            }
        }
    }
}