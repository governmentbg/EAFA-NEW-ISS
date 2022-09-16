using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Users;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Pub.Application.Interfaces.Transactions;
using IARA.Mobile.Pub.ViewModels.Base;
using IARA.Mobile.Pub.Views.FlyoutPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.ViewModels.FlyoutPages
{
    public class LoginViewModel : PageViewModel
    {
        private readonly IStartupTransaction _startupTransaction;
        private readonly IAuthenticationProvider _authenticationProvider;
        private readonly IAuthTokenProvider _authTokenProvider;
        private readonly IBackButton _backButton;
        private bool _noInternet;
        private bool _noServerConnection;

        public LoginViewModel(IAuthenticationProvider authenticationProvider, IStartupTransaction startupTransaction, IAuthTokenProvider authTokenProvider, IBackButton backButton)
        {
            _startupTransaction = startupTransaction;
            _authenticationProvider = authenticationProvider;
            _authTokenProvider = authTokenProvider;
            _backButton = backButton;

            Retry = CommandBuilder.CreateFrom(OnRetry);
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

        public override void OnAppearing()
        {
            _backButton.CloseAppOnBackButtonPressed = true;
        }

        public override void OnDisappearing()
        {
            _backButton.CloseAppOnBackButtonPressed = false;
        }

        public override Task Initialize(object sender)
        {
            return OnRetry();
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
                    await ReturnToMain();
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

                if (userAuth.CurrentLoginType == LoginTypeEnum.PASSWORD ||
                    (userAuth.CurrentLoginType == LoginTypeEnum.EAUTH && userAuth.HasEAuthLogin.GetValueOrDefault()))
                {
                    await TL.CallConfigure<Startup>(CommonConstants.NewLogin);
                    return;
                }

                _authTokenProvider.IsUnregisteredEAuthUser = true;

                //Намерен е съществуващ потребител в системата който не се е регистрирал чрез eAuth, и е с деактивирана парола;
                //Use case: Потребител влиза с eAuth, системата го асоциира с потребител със същото ЕГН, потребителят прекратява достъпът с парола(деактивира го),
                //но не завършва регистрацията до край(Затваря приложението).
                //При повторно влизане с eAuth, асоциираният потребител вече е с HasUserPassLogin = false && HasEAuthLogin = false;
                if (userAuth.CurrentLoginType == LoginTypeEnum.EAUTH
                    && !userAuth.HasUserPassLogin.GetValueOrDefault()
                    && !userAuth.HasEAuthLogin.GetValueOrDefault()
                    && userAuth.Id > 0)
                {
                    //Отваря страница в която потребителя си попълва личните данни и актуализира данните на съществуващия потребител в системата;
                    await Device.InvokeOnMainThreadAsync(() => App.Current.SetMainPage(new EAuthUserUpdatePage(userAuth)));
                    return;
                }

                //Намерен е потребител със същото ЕГН
                if (userAuth.CurrentLoginType == LoginTypeEnum.EAUTH && userAuth.HasUserPassLogin.GetValueOrDefault())
                {
                    //Отваря страница с избор "Потвърди имейл и парола" на съществуващия потребител или "Прекрати достъпът" му и премини към
                    //станица за попълване на лични данни(EAuthUserUpdatePage) които да презапишат съществуващия потребител
                    await Device.InvokeOnMainThreadAsync(() => App.Current.SetMainPage(new ConfirmCredentialsPage(userAuth)));
                    return;
                }

                //Първи вход с eAuth
                await Device.InvokeOnMainThreadAsync(() => App.Current.SetMainPage(new RegisterPage(userAuth)));
            }
            else
            {
                await ReturnToMain();
            }
        }

        private async Task ReturnToMain()
        {
            await Device.InvokeOnMainThreadAsync(() => App.Current.SetMainPage(new PublicHomePage()));
            await TLSnackbar.Show(TranslateExtension.Translator[nameof(GroupResourceEnum.CommonOffline) + "/LoginError"], App.GetResource<Color>("ErrorColor"));
        }
    }
}
