using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Pub.ViewModels.Base;
using IARA.Mobile.Pub.Views.FlyoutPages;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.ViewModels.FlyoutPages
{
    public class CompletedRegistrationViewModel : PageViewModel
    {
        public CompletedRegistrationViewModel()
        {
            GoToHomePage = CommandBuilder.CreateFrom(OnGoToHomePage);
            ResentEmail = CommandBuilder.CreateFrom(OnResentEmail);
        }
        public string Email { get; set; }
        public ICommand GoToHomePage { get; }
        public ICommand ResentEmail { get; }

        public override Task Initialize(object sender)
        {
            return Task.CompletedTask;
        }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return Array.Empty<GroupResourceEnum>();
        }

        private Task OnGoToHomePage()
        {
            return Device.InvokeOnMainThreadAsync(() => App.Current.SetMainPage(new PublicHomePage()));
        }

        private async Task OnResentEmail()
        {
            bool accept = await App.Current.MainPage.DisplayAlert(
                TranslateExtension.Translator[nameof(GroupResourceEnum.Register) + "/ResendEmailTitle"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Register) + "/ResendEmailMessage"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Yes"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Cancel"]
            );

            if (accept)
            {
                await TLLoadingHelper.ShowFullLoadingScreen();
                bool success = await UserTransaction.ResentEmail(Email);
                await TLLoadingHelper.HideFullLoadingScreen();

                if (success)
                {
                    await TLSnackbar.Show(TranslateExtension.Translator[nameof(GroupResourceEnum.Register) + "/ResentEmailSuccessRequest"], Color.Green);
                }
                else
                {
                    await TLSnackbar.Show(TranslateExtension.Translator[nameof(GroupResourceEnum.Register) + "/ResentEmailRequestFailed"], App.GetResource<Color>("ErrorColor"));
                }
            }
        }
    }
}
