using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using TechnoLogica.Xamarin.ResourceTranslator;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.Utilities
{
    public class PopUpUtility : IPopUp
    {
        private bool _isPopupShowing;

        public void AlertException()
        {
            if (_isPopupShowing)
            {
                return;
            }

            Device.BeginInvokeOnMainThread(async () =>
            {
                _isPopupShowing = true;
                await App.Current.MainPage.DisplayAlert(
                    TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/ExceptionTitle"],
                    TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/ExceptionMessage"],
                    TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Ok"]
                );
                _isPopupShowing = false;
            });
        }

        public void AlertNoInternet()
        {
            if (_isPopupShowing)
            {
                return;
            }

            Device.BeginInvokeOnMainThread(async () =>
            {
                _isPopupShowing = true;
                await App.Current.MainPage.DisplayAlert(
                    TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/WarningNoInternetTitle"],
                    TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/WarningNoInternetMessage"],
                    TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Ok"]
                );
                _isPopupShowing = false;
            });
        }

        public void AlertRequestEntityTooLarge()
        {
            if (_isPopupShowing)
            {
                return;
            }

            Device.BeginInvokeOnMainThread(async () =>
            {
                _isPopupShowing = true;
                await App.Current.MainPage.DisplayAlert(
                    TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/RequestEntityTooLargeTitle"],
                    TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/RequestEntityTooLargeMessage"],
                    TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Ok"]
                );
                _isPopupShowing = false;
            });
        }

        public void AlertUnsuccessfulRequest()
        {
            if (_isPopupShowing)
            {
                return;
            }

            Device.BeginInvokeOnMainThread(async () =>
            {
                _isPopupShowing = true;
                await App.Current.MainPage.DisplayAlert(
                    TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/UnsuccessfulRequestTitle"],
                    TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/UnsuccessfulRequestMessage"],
                    TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Ok"]
                );
                _isPopupShowing = false;
            });
        }
    }
}
