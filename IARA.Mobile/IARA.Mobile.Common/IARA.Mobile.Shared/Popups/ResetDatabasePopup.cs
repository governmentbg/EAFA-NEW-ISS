using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Application;
using IARA.Mobile.Shared.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Rg.Plugins.Popup.Pages;
using TechnoLogica.Xamarin.Commands;
using IARA.Mobile.Pub.Application.Interfaces.Utilities;
using IARA.Mobile.Pub.Application.Interfaces.Transactions;
using System.Threading.Tasks;
using TechnoLogica.Xamarin.Helpers;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IStartupTransaction = IARA.Mobile.Insp.Application.Interfaces.Transactions.IStartupTransaction;
using Rg.Plugins.Popup.Services;

namespace IARA.Mobile.Shared.Popups
{
    public class ResetDatabasePopup : PopupPage
    {
        private IAppDbMigration _appDbMigration => DependencyService.Resolve<IAppDbMigration>();
        private IDbSettings _dbSettings => DependencyService.Resolve<IDbSettings>();
        private IStartupTransaction _startupTransaction => DependencyService.Resolve<IStartupTransaction>();
        private INomenclatureDatesClear _nomenclatureDates => DependencyService.Resolve<INomenclatureDatesClear>();
        private IInspectionsTransaction _inspectionsTransaction => DependencyService.Resolve<IInspectionsTransaction>();

        public ResetDatabasePopup()
        {
            Content = new Frame
            {
                HasShadow = false,
                Margin = 50,
                Padding = 0,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Content = new StackLayout
                {
                    Spacing = 0,
                    Children =
                    {
                        new Image
                        {
                            Source = ImageSourceConvert("iara_logo"),
                            HeightRequest = 150,
                            WidthRequest = 150,
                            Margin = new Thickness(0, 15, 0, 0),
                            HorizontalOptions = LayoutOptions.Center
                        },
                        new Label
                        {
                            FontSize = 24,
                            Padding = 5,
                            FontAttributes = FontAttributes.Bold,
                            Text = "Внимание!",
                            HorizontalOptions = LayoutOptions.Center,
                        },
                        new Label
                        {
                            Margin = new Thickness(20, 10),
                            Text = "Изисква се обновяване на базата с данни",
                            LineBreakMode = LineBreakMode.WordWrap,
                            HorizontalTextAlignment = TextAlignment.Center,
                        },
                        new TLButton
                        {
                            Margin = new Thickness(20, 10),
                            Text = "Обнови",
                            Command = CommandBuilder.CreateFrom(OnResetDatabase)
                        }
                    }
                }
            };
        }

        private async Task OnResetDatabase()
        {
            await TLLoadingHelper.ShowFullLoadingScreen();

            _appDbMigration.DropDatabase();
            _nomenclatureDates.Clear();
            _dbSettings.Clear();
            _appDbMigration.CheckForMigrations();
            await _startupTransaction.GetInitialData(true, null, null);
            _ = await _inspectionsTransaction.GetAll(1, reset: true);
            _startupTransaction.ResetDatabase();
            await TLLoadingHelper.HideFullLoadingScreen();

            await PopupNavigation.Instance.PopAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            // Return true if you don't want to close this popup page when a back button is pressed
            return true;
        }

        // Invoked when background is clicked
        protected override bool OnBackgroundClicked()
        {
            // Return false if you don't want to close this popup page when a background of the popup page is clicked
            return false;
        }

        public static FileImageSource ImageSourceConvert(string path)
        {
            if (Device.RuntimePlatform == Device.UWP)
            {
                path = System.IO.Path.Combine($"Images/{path}.png");
            }

            return (FileImageSource)ImageSource.FromFile(path);
        }
    }
}