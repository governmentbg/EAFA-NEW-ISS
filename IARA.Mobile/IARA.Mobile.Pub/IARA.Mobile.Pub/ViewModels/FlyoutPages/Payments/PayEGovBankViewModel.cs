using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Pub.Views.FlyoutPages;
using IARA.Mobile.Shared.Menu;
using Rg.Plugins.Popup.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Base.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.ViewModels.FlyoutPages.Payments
{
    public class PayEGovBankViewModel : BaseModel
    {
        private string _currentState;
        public PayEGovBankViewModel()
        {
            CopyToClipboard = CommandBuilder.CreateFrom(OnCopyToClipboard);
            OpenUrl = CommandBuilder.CreateFrom(OnOpenUrl);
            CurrentState = "FirstStep";
        }
        public string Code { get; set; }
        public Uri PayEGovUrl { get; set; }
        public ICommand CopyToClipboard { get; }
        public ICommand OpenUrl { get; }
        public ICommand BackToFirstStep { get; }
        public string CurrentState
        {
            get => _currentState;
            set => SetProperty(ref _currentState, value);
        }

        private async Task OnCopyToClipboard()
        {
            CurrentState = "SecondStep";
            await Clipboard.SetTextAsync(Code);
            await TLSnackbar.Show(TranslateExtension.Translator[nameof(GroupResourceEnum.Payments) + "/PaymentCodeCopied"], Color.Green);
        }

        private async Task OnOpenUrl()
        {
            try
            {
                await Browser.OpenAsync(PayEGovUrl, BrowserLaunchMode.SystemPreferred);
                await PopupNavigation.Instance.PopAllAsync();
                await MainNavigator.Current.GoToPageAsync(nameof(HomePage));
            }
            catch (Exception)
            {
                // An unexpected error occured. No browser may be installed on the device.
            }
        }
    }
}
