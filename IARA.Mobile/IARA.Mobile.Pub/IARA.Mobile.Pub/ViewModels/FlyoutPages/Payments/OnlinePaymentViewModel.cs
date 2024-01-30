using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Pub.Application.Interfaces.Transactions;
using IARA.Mobile.Pub.ViewModels.FlyoutPages.FishingTicket;
using IARA.Mobile.Pub.Views.FlyoutPages.Payments;
using Rg.Plugins.Popup.Services;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.ViewModels.Base.Models;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.ViewModels.FlyoutPages.Payments
{
    public class OnlinePaymentViewModel : BaseModel
    {
        private bool scriptEvaluated;
        private string _paymentInitialUrl;
        public OnlinePaymentViewModel()
        {
            Navigating = CommandBuilder.CreateFrom<WebNavigatingEventArgs>(OnNavigating);
            Navigated = CommandBuilder.CreateFrom<WebNavigatedEventArgs>(OnNavigated);
        }
        public string PaymentOkUrl { get; set; }
        public string PaymentCanceledUrl { get; set; }
        public string PaymentInitialUrl
        {
            get => _paymentInitialUrl;
            set => SetProperty(ref _paymentInitialUrl, value);
        }
        public string PaymentCode { get; set; }
        public string PaymentRequestNum { get; set; }
        public string Token { get; set; }
        public bool IsFromEpay { get; set; }
        public ICommand Navigating { get; }
        public ICommand Navigated { get; }
        public WebView WebView { get; set; }
        private async Task OnNavigating(WebNavigatingEventArgs args)
        {
            if (args != null && WebView != null)
            {
                if (args.Url == PaymentOkUrl)
                {
                    MessagingCenter.Instance.Send(new TicketsChangedArgs(TicketsChangedEnum.PaidTicket), TicketsChangedArgs.TICKETS_CHANGED_EVENT);
                    WebView = null;
                    await PopupNavigation.Instance.PopAsync();
                    await PopupNavigation.Instance.PushAsync(new SuccessfulPaymentPopup());

                    await DependencyService.Resolve<IPaymentTransaction>()
                        .MarkPaymentForProcessing(PaymentRequestNum, false, IsFromEpay);
                }
                else if (args.Url == PaymentCanceledUrl)
                {
                    WebView = null;

                    await PopupNavigation.Instance.PopAllAsync();
                    await DependencyService.Resolve<IPaymentTransaction>()
                        .MarkPaymentForProcessing(PaymentRequestNum, true, IsFromEpay);
                }
            }
        }

        private async Task OnNavigated(WebNavigatedEventArgs args)
        {
            if (args != null && !scriptEvaluated)
            {
                bool isPaymentProcessPage = args.Url == PaymentInitialUrl;
                if (isPaymentProcessPage)
                {
                    string documentLoadedScript = "eval(document.onlinePayment)==undefined";
                    int tryCount = 1;
                    while (bool.Parse(await WebView.EvaluateJavaScriptAsync(documentLoadedScript)) && tryCount < 10)
                    {
                        await Task.Delay(100 * tryCount);
                        System.Console.WriteLine($"tryCount:{tryCount}");
                        tryCount++;
                    }
                    scriptEvaluated = true;
                    string setPaymentParametersScript = $"document.onlinePayment.setPaymentParams('{PaymentRequestNum}','{PaymentCode}','{Token}');";
                    Debug.WriteLine(setPaymentParametersScript);
                    string result = await WebView.EvaluateJavaScriptAsync(setPaymentParametersScript);
                    Debug.WriteLine($"EvaluateJavaScriptAsync Result:{result}");
                }
            }
        }
    }
}
