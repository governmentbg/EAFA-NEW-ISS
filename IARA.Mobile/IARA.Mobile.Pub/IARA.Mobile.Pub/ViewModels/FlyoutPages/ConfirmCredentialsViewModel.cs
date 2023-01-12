using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Users;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Pub.ViewModels.Base;
using IARA.Mobile.Pub.Views.FlyoutPages;
using TechnoLogica.Xamarin;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.ViewModels.FlyoutPages
{
    public class ConfirmCredentialsViewModel : PageViewModel
    {
        private ValidState _email;
        private ValidState _password;
        private readonly IAuthenticationProvider _authenticationProvider;

        public ConfirmCredentialsViewModel(IAuthenticationProvider authenticationProvider)
        {
            Confirm = CommandBuilder.CreateFrom(OnConfirm);
            CancelAccount = CommandBuilder.CreateFrom(OnCancelAccount);
            GoToLogin = CommandBuilder.CreateFrom(OnGoToHomePage);
            this.AddValidation();
            _authenticationProvider = authenticationProvider;
        }

        public UserAuthDto UserAuth { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = null)]
        public ValidState Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        [Required]
        public ValidState Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public ICommand GoToLogin { get; }
        public ICommand Confirm { get; }
        public ICommand CancelAccount { get; }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return Array.Empty<GroupResourceEnum>();
        }

        public override Task Initialize(object sender)
        {
            return Task.CompletedTask;
        }

        private async Task OnGoToHomePage()
        {
            await _authenticationProvider.SoftLogout();
            await Device.InvokeOnMainThreadAsync(() => App.Current.SetMainPage(new PublicHomePage()));
        }

        private async Task OnCancelAccount()
        {
            await TLLoadingHelper.ShowFullLoadingScreen();

            bool userPasswordAccountDeactivated = await UserTransaction.DeactivateUserPasswordAccount(UserAuth.EgnLnch);

            await TLLoadingHelper.HideFullLoadingScreen();

            if (userPasswordAccountDeactivated)
            {
                await Device.InvokeOnMainThreadAsync(() => App.Current.SetMainPage(new EAuthUserUpdatePage(UserAuth)));
            }
            else
            {
                await TLSnackbar.Show(TranslateExtension.Translator["CommonOffline/UnhandledError"], App.GetResource<Color>("ErrorColor"));
            }
        }

        private async Task OnConfirm()
        {
            Validation.Force();

            if (!Validation.IsValid)
            {
                return;
            }

            await TLLoadingHelper.ShowFullLoadingScreen();

            bool? isConfirmed = await UserTransaction.ConfirmEmailAndPassword(new ConfirmUserDataDto
            {
                Email = Email,
                Password = Password,
                FirstName = UserAuth.FirstName,
                MiddleName = UserAuth.MiddleName,
                LastName = UserAuth.LastName
            });

            await TLLoadingHelper.HideFullLoadingScreen();

            if (isConfirmed == null)
            {
                await TLSnackbar.Show(TranslateExtension.Translator["CommonOffline/UnhandledError"], App.GetResource<Color>("ErrorColor"));
            }
            else if (!isConfirmed.GetValueOrDefault())
            {
                await TLSnackbar.Show(TranslateExtension.Translator["Register/CredentialsMismatch"], App.GetResource<Color>("ErrorColor"));
            }
            else
            {
                await TL.CallConfigure<Startup>(CommonConstants.NewLogin);
            }
        }
    }
}
