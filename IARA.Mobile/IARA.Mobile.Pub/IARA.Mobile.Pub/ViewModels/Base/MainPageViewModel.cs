using System;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Pub.Views.FlyoutPages;
using IARA.Mobile.Shared.Menu;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.ViewModels.Base
{
    public abstract class MainPageViewModel : PageViewModel
    {
        private readonly IBackButton _backButton;

        protected MainPageViewModel()
        {
            _backButton = DependencyService.Resolve<IBackButton>();
        }

        public override void OnAppearing()
        {
            _backButton.PreventBackButtonPress = true;
            _backButton.BackButtonPressed += OnBackButtonPressed;
        }

        public override void OnDisappearing()
        {
            _backButton.PreventBackButtonPress = false;
            _backButton.BackButtonPressed -= OnBackButtonPressed;
        }

        private void OnBackButtonPressed(object sender, EventArgs e)
        {
            MainNavigator.Current.GoToPageAsync(nameof(HomePage));
        }
    }
}
