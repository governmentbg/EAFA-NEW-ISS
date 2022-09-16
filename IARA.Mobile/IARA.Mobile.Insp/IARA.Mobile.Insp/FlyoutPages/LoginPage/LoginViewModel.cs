using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Users;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Insp.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Enums;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.FlyoutPages.LoginPage
{
    public class LoginViewModel : PageViewModel
    {
        private readonly IStartupTransaction _startupTransaction;
        private readonly IAuthenticationProvider _authenticationProvider;
        private readonly IBackButton _backButton;
        private bool _noInternet;
        private bool _noServerConnection;
        private bool _browserClosed;

        public LoginViewModel(IAuthenticationProvider authenticationProvider, IStartupTransaction startupTransaction, IBackButton backButton)
        {
            _startupTransaction = startupTransaction;
            _authenticationProvider = authenticationProvider;
            _backButton = backButton;

            Retry = CommandBuilder.CreateFrom(OnRetry);
        }

        public bool BrowserClosed
        {
            get => _browserClosed;
            set => SetProperty(ref _browserClosed, value);
        }

        public bool NoInternet
        {
            get => _noInternet;
            set => SetProperty(ref _noInternet, value);
        }

        public bool NoServerConnection
        {
            get => _noServerConnection;
            set => SetProperty(ref _noServerConnection, value);
        }

        public ICommand Retry { get; }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return Array.Empty<GroupResourceEnum>();
        }

        public override IReadOnlyDictionary<GroupResourceEnum, IReadOnlyDictionary<string, string>> GetPageResources(out GroupResourceEnum[] filtered)
        {
            filtered = Array.Empty<GroupResourceEnum>();
            return new Dictionary<GroupResourceEnum, IReadOnlyDictionary<string, string>>();
        }

        public override Task Initialize(object sender)
        {
            return OnRetry();
        }

        public override void OnAppearing()
        {
            _backButton.CloseAppOnBackButtonPressed = true;
        }

        public override void OnDisappearing()
        {
            _backButton.CloseAppOnBackButtonPressed = false;
        }

        private async Task OnRetry()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.None)
            {
                NoServerConnection = false;
                NoInternet = true;
                BrowserClosed = false;
                return;
            }

            bool result = await _startupTransaction.HealthCheck();
            if (!result)
            {
                NoInternet = false;
                NoServerConnection = true;
                BrowserClosed = false;
                return;
            }

            await OpenLogin();
        }

        private async Task OpenLogin()
        {
            bool successfullyLoggedIn = await _authenticationProvider.Login();

            if (successfullyLoggedIn)
            {
                UserAuthDto userAuth = await _startupTransaction.GetUserAuthInfo();
                if (userAuth == null)
                {
                    return;
                }

                ICurrentUser currentUser = CurrentUser;

                if (currentUser.Id != default && userAuth.Id != currentUser.Id)
                {
                    DependencyService.Resolve<ICommonLogout>()
                        .SoftDeleteLocalInfo();
                }

                currentUser.EgnLnch = userAuth.EgnLnch;
                currentUser.FirstName = userAuth.FirstName;
                currentUser.MiddleName = userAuth.MiddleName;
                currentUser.LastName = userAuth.LastName;
                currentUser.Id = userAuth.Id;
                currentUser.MustChangePassword = userAuth.UserMustChangePassword;

                ConfigureResultType result = await TL.CallConfigure<Startup>(CommonConstants.NewLogin);

                if (result != ConfigureResultType.Successful)
                {
                    BrowserClosed = true;
                    currentUser.Clear();
                    _authenticationProvider.Dispose();
                    await App.Current.MainPage.DisplayAlert(
                        "Проблем при изтегляне на данни",
                        "Възникна проблем при опит за изтегляне на необходимите данни, което автоматично е докладвано на ИАРА. Ако след повторен опит получите това съобщение отново, моля, проверете дали използвате стара версия на приложението или се свържете директно с ИАРА.",
                        "Добре"
                    );
                }
            }
            else
            {
                BrowserClosed = !NoInternet && !NoServerConnection;
            }
        }
    }
}
