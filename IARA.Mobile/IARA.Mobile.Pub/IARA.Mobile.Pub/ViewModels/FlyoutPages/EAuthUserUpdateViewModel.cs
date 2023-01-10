using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Application.DTObjects.Common;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Pub.Application.DTObjects.User.API;
using IARA.Mobile.Pub.ViewModels.Base;
using IARA.Mobile.Pub.Views.FlyoutPages;
using IARA.Mobile.Shared.Attributes.PasswordAttributes;
using IARA.Mobile.Shared.ViewModels.Models;
using TechnoLogica.Xamarin.Attributes;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.ViewModels.FlyoutPages
{
    public class EAuthUserUpdateViewModel : PageViewModel
    {
        private ValidState _firstName;
        private ValidState _middleName;
        private ValidState _lastName;
        private EgnLncValidState _egnLnc;
        private ValidState _email;
        private ValidState _password;
        private ValidState _repeatPassword;
        private ValidStateBool _termsAndCondictions;
        private TLObservableCollection<string> _customValidationErrors;
        private readonly IAuthenticationProvider _authProvider;

        public EAuthUserUpdateViewModel(IAuthenticationProvider authenticationProvider)
        {
            _authProvider = authenticationProvider;
            GoToLogin = CommandBuilder.CreateFrom(OnGoToLogin);
            UpdateEAuthUser = CommandBuilder.CreateFrom(OnUpdateEAuthUser);
            GoToHomePage = CommandBuilder.CreateFrom(OnGoToHomePage);
            CustomValidationErrors = new TLObservableCollection<string>();
            this.AddValidation();
        }

        [Required]
        [StringLength(100)]
        public ValidState FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        [StringLength(100)]
        public ValidState MiddleName
        {
            get => _middleName;
            set => SetProperty(ref _middleName, value);
        }

        [Required]
        [StringLength(100)]
        public ValidState LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        [Required]
        [StringLength(20)]
        public EgnLncValidState EgnLnc
        {
            get => _egnLnc;
            set => SetProperty(ref _egnLnc, value);
        }

        [Required]
        [EmailAddress(ErrorMessage = null)]
        [StringLength(100)]
        public ValidState Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        [StringLength(200)]
        [RequireLowerCase]
        [RequireUpperCase]
        [RequireMinLength(8)]
        [RequireSpecialSymbolOrDigit]
        public ValidState Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        [StringLength(200)]
        [EqualTo(nameof(Password))]
        [UpdateFrom(nameof(Password), MustBeDirty = false)]
        public ValidState RepeatPassword
        {
            get => _repeatPassword;
            set => SetProperty(ref _repeatPassword, value);
        }

        [Required]
        public ValidStateBool TermsAndConditions
        {
            get => _termsAndCondictions;
            set => SetProperty(ref _termsAndCondictions, value);
        }

        public TLObservableCollection<string> CustomValidationErrors
        {
            get => _customValidationErrors;
            set => SetProperty(ref _customValidationErrors, value);
        }

        public ICommand GoToLogin { get; }
        public ICommand GoToHomePage { get; }
        public ICommand UpdateEAuthUser { get; }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return Array.Empty<GroupResourceEnum>();
        }

        public override Task Initialize(object sender)
        {
            return Task.CompletedTask;
        }

        private async Task OnGoToLogin()
        {
            await _authProvider.SoftLogout();
            await Device.InvokeOnMainThreadAsync(() => App.Current.SetMainPage(new LoginPage()));
        }

        private async Task OnGoToHomePage()
        {
            await _authProvider.SoftLogout();
            await Device.InvokeOnMainThreadAsync(() => App.Current.SetMainPage(new PublicHomePage()));
        }

        private async Task OnUpdateEAuthUser()
        {
            CustomValidationErrors.Clear();

            Validation.Force();

            if (!Validation.IsValid)
            {
                return;
            }

            await TLLoadingHelper.ShowFullLoadingScreen();

            ResponseApiDto result = await UserTransaction.UpdateUserEAuthData(new UserRegistrationDto
            {
                Email = Email,
                EgnLnc = new EgnLncDto
                {
                    EgnLnc = EgnLnc,
                    IdentifierType = EgnLnc.IdentifierType
                },
                FirstName = FirstName,
                MiddleName = MiddleName,
                LastName = LastName,
                Password = Password,
                HasUserPassLogin = !string.IsNullOrEmpty(Password),
                HasEAuthLogin = true,
                CurrentLoginType = LoginTypeEnum.EAUTH,
            });

            await TLLoadingHelper.HideFullLoadingScreen();

            if (result.IsSuccessful)
            {
                await _authProvider.SoftLogout();
                await Device.InvokeOnMainThreadAsync(() => App.Current.SetMainPage(new CompletedRegistration(Email)));
                await TLSnackbar.Show(TranslateExtension.Translator["Register/SuccessfulRegistration"], System.Drawing.Color.Green);
            }
            else if (result.ErrorMessages?.Count > 0)
            {
                CustomValidationErrors.AddRange(result.ErrorMessages);
            }
            else
            {
                await TLSnackbar.Show(TranslateExtension.Translator["CommonOffline/UnhandledError"], App.GetResource<Color>("ErrorColor"));
            }
        }
    }
}
