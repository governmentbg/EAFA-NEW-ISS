using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Users;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Insp.Application.Interfaces.Utilities;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Pub.Domain.Models;
using IARA.Mobile.Shared.ResourceTranslator;
using TechnoLogica.Xamarin;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Enums;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.FlyoutPages.LoginPage
{
    public class LoginViewModel : PageViewModel
    {
        private readonly IStartupTransaction _startupTransaction;
        private readonly IAuthenticationProvider _authenticationProvider;
        private readonly IBackButton _backButton;
        private readonly ISettings _settings;
        private readonly IAuthenticationTransaction _authenticationTransaction;
        private bool _noInternet;
        private bool _noServerConnection;
        public LoginViewModel(IAuthenticationProvider authenticationProvider, IStartupTransaction startupTransaction, IBackButton backButton, ISettings settings, IAuthenticationTransaction authenticationTransaction)
        {
            _startupTransaction = startupTransaction;
            _authenticationProvider = authenticationProvider;
            _backButton = backButton;
            _settings = settings;
            _authenticationTransaction = authenticationTransaction;

            this.AddValidation();

            Retry = CommandBuilder.CreateFrom(OnRetry);
            LogIn = CommandBuilder.CreateFrom(OnLogin);
        }


        public bool ShowLogInForm => !(NoInternet || NoServerConnection);

        public bool NoInternet
        {
            get => _noInternet;
            set
            {
                SetProperty(ref _noInternet, value);
                OnPropertyChanged(nameof(ShowLogInForm));
            }
        }

        public bool NoServerConnection
        {
            get => _noServerConnection;
            set
            {
                SetProperty(ref _noServerConnection, value);
                OnPropertyChanged(nameof(ShowLogInForm));
            }
        }

        public ICommand Retry { get; }
        public ICommand LogIn { get; }


        [Required]
        public ValidState UsernameState { get; set; }
        [Required]
        public ValidState PasswordState { get; set; }
        public ValidState<bool> RememberMeState { get; set; }

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

        private async Task OnLogin()
        {
            Validation.Force();

            if (Validation.IsValid)
            {
                string username = UsernameState.Value;
                string password = PasswordState.Value;
                bool rememberMe = RememberMeState.Value;

                JwtToken token = await _authenticationTransaction.LogIn(new AuthCredentials()
                {
                    UserName = username,
                    Password = password,
                    RememberMe = rememberMe
                });


                if (token != null)
                {
                    _authenticationProvider.SetAuthenticationProvider(token);
                    await SetUserInfo();
                }
                else
                {
                    await TLSnackbar.Show(/*TranslateExtension.Translator[nameof(GroupResourceEnum.CommonOffline) + "/LoginError"]*/"Error", App.GetResource<Color>("ErrorColor"));
                }
            }
        }

        private async Task OnRetry()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.None)
            {
                NoServerConnection = false;
                NoInternet = true;
                return;
            }

            bool result = await _startupTransaction.HealthCheck();
            if (!result)
            {
                NoInternet = false;
                NoServerConnection = true;
                return;
            }
        }

        private async Task SetUserInfo()
        {
            UserAuthDto userAuth = await _startupTransaction.GetUserAuthInfo();

            if (userAuth == null)
            {
                return;
            }

            ICurrentUser currentUser = CurrentUser;

            if (currentUser.Id != default && userAuth.UserId != currentUser.Id)
            {
                DependencyService.Resolve<ICommonLogout>()
                    .SoftDeleteLocalInfo();
            }

            currentUser.EgnLnch = userAuth.EgnLnch;
            currentUser.FirstName = userAuth.FirstName;
            currentUser.MiddleName = userAuth.MiddleName;
            currentUser.LastName = userAuth.LastName;
            currentUser.Id = userAuth.UserId;
            currentUser.MustChangePassword = userAuth.UserMustChangePassword;

            ConfigureResultType result = await TL.CallConfigure<Startup>(CommonConstants.NewLogin);

            if (result != ConfigureResultType.Successful)
            {
                currentUser.Clear();
                _authenticationProvider.Dispose();
                await App.Current.MainPage.DisplayAlert(
                    "Проблем при изтегляне на данни",
                    "Възникна проблем при опит за изтегляне на необходимите данни, което автоматично е докладвано на ИАРА. Ако след повторен опит получите това съобщение отново, моля, проверете дали използвате стара версия на приложението или се свържете директно с ИАРА.",
                    "Добре"
                );
            }
        }
    }
}
