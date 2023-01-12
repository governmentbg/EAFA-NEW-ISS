using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Pub.ViewModels.Base;
using IARA.Mobile.Pub.Views.FlyoutPages.Payments;
using Rg.Plugins.Popup.Services;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.ViewModels.FlyoutPages.Payments
{
    public class ChoosePaymentTypeViewModel : PageViewModel
    {
        private readonly IServerUrl _urlProvider;
        private readonly IAuthTokenProvider _tokenProvider;

        public ChoosePaymentTypeViewModel(IServerUrl urlProvider, IAuthTokenProvider tokenProvider)
        {
            PaymentTypeTapped = CommandBuilder.CreateFrom<string>(OnPaymentTypeTapped);
            PaymentTypes = new TLObservableCollection<NomenclatureDto>();
            _urlProvider = urlProvider;
            _tokenProvider = tokenProvider;
        }
        public int ApplicationId { get; set; }
        public decimal TotalPrice { get; set; }
        public ICommand PaymentTypeTapped { get; }

        public TLObservableCollection<NomenclatureDto> PaymentTypes { get; }
        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[] {
                GroupResourceEnum.Payments,
            };
        }

        public override Task Initialize(object sender)
        {
            List<NomenclatureDto> paymentTypes = NomenclaturesTransaction.GetPaymentTypes(new List<string>
            {
                PaymentTypesConstants.PAY_EGOV_EPAYBG,
                PaymentTypesConstants.PAY_EGOV_EPOS,
                PaymentTypesConstants.PAY_EGOV_BANK,
            });

            PaymentTypes.AddRange(paymentTypes);
            return Task.CompletedTask;
        }

        private async Task OnPaymentTypeTapped(string paymentTypeCode)
        {
            switch (paymentTypeCode)
            {
                case PaymentTypesConstants.PAY_EGOV_BANK:
                    await HandlePayEgovBankPayment();
                    break;
                case PaymentTypesConstants.PAY_EGOV_EPAYBG:
                    await HandleOnlinePayment("EPAY");
                    break;
                case PaymentTypesConstants.PAY_EGOV_EPOS:
                    await HandleOnlinePayment("CARD");
                    break;
            }
        }

        private async Task HandleOnlinePayment(string paymentCode)
        {
            await TLLoadingHelper.ShowFullLoadingScreen();
            await PopupNavigation.Instance.PushAsync(new OnlinePaymentPopup(BuildPaymentModel(paymentCode)));
            await TLLoadingHelper.HideFullLoadingScreen();
        }

        private async Task HandlePayEgovBankPayment()
        {
            await TLLoadingHelper.ShowFullLoadingScreen();
            string paymentNumber = await PaymentTransaction.RegisterOfflinePayment(ApplicationId);

            if (!string.IsNullOrEmpty(paymentNumber))
            {
                await PopupNavigation.Instance.PushAsync(new PayEGovBankPopup(new PayEGovBankViewModel { PayEGovUrl = _urlProvider.GetEnvironmentUri("PAY_EGOV"), Code = paymentNumber }));
            }
            else
            {
                await TLSnackbar.Show(TranslateExtension.Translator[nameof(GroupResourceEnum.Payments) + "/GeneratePaymentCodeWentWrong"], Color.Red);
            }
            await TLLoadingHelper.HideFullLoadingScreen();
        }

        private OnlinePaymentViewModel BuildPaymentModel(string paymentCode)
        {
            OnlinePaymentViewModel onlinePayment = new OnlinePaymentViewModel
            {
                PaymentOkUrl = _urlProvider.BuildUrl(environment: "PAYMENT_OK").TrimEnd(new char[] { '/' }),
                PaymentCanceledUrl = _urlProvider.BuildUrl(environment: "PAYMENT_CANCELED").TrimEnd(new char[] { '/' }),
                PaymentInitialUrl = _urlProvider.GetEnvironmentBaseUrl() + "/online-payment",
                ApplicationId = ApplicationId,
                Token = _tokenProvider.Token,
                PaymentCode = paymentCode
            };
            return onlinePayment;
        }
    }
}
