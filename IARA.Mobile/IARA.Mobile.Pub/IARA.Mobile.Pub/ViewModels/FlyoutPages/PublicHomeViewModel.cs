using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Pub.Application.Interfaces.Utilities;
using IARA.Mobile.Pub.ViewModels.Base;
using IARA.Mobile.Pub.Views.FlyoutPages;
using IARA.Mobile.Shared.Helpers;
using IARA.Mobile.Shared.ResourceTranslator;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.ResourceTranslator;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.ViewModels.FlyoutPages
{
    public class PublicHomeViewModel : PageViewModel
    {
        public PublicHomeViewModel()
        {
            NavigateToLoginPage = CommandBuilder.CreateFrom(OnNavigateToLoginPage);
            NavigateToRegisterPage = CommandBuilder.CreateFrom(OnNavigateToRegisterPage);
            ChangeLanguage = CommandBuilder.CreateFrom(OnChangeLanguage);
        }

        public ICommand NavigateToLoginPage { get; }
        public ICommand NavigateToRegisterPage { get; }
        public ICommand ChangeLanguage { get; }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return Array.Empty<GroupResourceEnum>();
        }

        public override Task Initialize(object sender)
        {
            return Task.CompletedTask;
        }

        private void OnNavigateToLoginPage()
        {
            App.Current.SetMainPage(new LoginPage());
        }

        private void OnNavigateToRegisterPage()
        {
            App.Current.SetMainPage(new RegisterPage());
        }

        private async Task OnChangeLanguage()
        {
            string result = await App.Current.MainPage.DisplayActionSheet(
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/ChooseLanguage"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Cancel"],
                null,
                "Български",
                "English"
            );

            switch (result)
            {
                case "Български":
                    Language(ResourceLanguageEnum.BG);
                    break;
                case "English":
                    Language(ResourceLanguageEnum.EN);
                    break;
            }
        }

        private void Language(ResourceLanguageEnum languageEnum)
        {
            string iso = languageEnum.ToString().ToLower();

            ISettings settings = DependencyService.Resolve<ISettings>();

            if (settings.CurrentResourceLanguage == languageEnum)
            {
                return;
            }

            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfoByIetfLanguageTag(iso);
            settings.CurrentResourceLanguage = languageEnum;

            Translator.Current.LoadOfflineResources();
            Translator.Current.Invalidate();
        }
    }
}
