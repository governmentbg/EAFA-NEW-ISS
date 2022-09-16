using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application;
using IARA.Mobile.Insp.FlyoutPages.InspectionsPage;
using IARA.Mobile.Insp.FlyoutPages.ProfilePage;
using IARA.Mobile.Insp.FlyoutPages.SettingsPage;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Shared.Menu;
using IARA.Mobile.Shared.Views;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.ResourceTranslator;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainFlyoutPage : ContentPage, IFlyoutPage
    {
        private int versionTappedCount;
        public MainFlyoutPage()
        {
            InitializeComponent();
            VersionSpan.Text = VersionTracking.CurrentVersion;

            Routes = new Dictionary<string, Func<Page>>
            {
                {
                    nameof(InspectionsPage),
                    () => new InspectionsPage()
                },
                {
                    nameof(ProfilePage),
                    () => new ProfilePage()
                },
                {
                    nameof(SettingsPage),
                    () => new SettingsPage()
                },
                {
                    nameof(SystemInformationPage),
                    () => new SystemInformationPage()
                }
            };

            NavigationItems = new List<NavigationItemView>();

            logoutNavItem.ItemTapped = CommandBuilder.CreateFrom(OnLogout);
            addInspections.ItemTapped = CommandBuilder.CreateFrom(OnOpenAddInspectionDrawer);
            reports.ItemTapped = CommandBuilder.CreateFrom(OnReportsClicked);

            profileNavItem.ItemTapped = CommandBuilder.CreateFrom<string>(OnOpenRootPage);
            settings.ItemTapped = CommandBuilder.CreateFrom<string>(OnOpenRootPage);
            inspections.ItemTapped = CommandBuilder.CreateFrom<string>(OnOpenRootPage);
        }

        public IReadOnlyList<NavigationItemView> NavigationItems { get; }
        public IReadOnlyDictionary<string, Func<Page>> Routes { get; }
        public Action CloseFlyout { get; set; }

        private Task OnOpenAddInspectionDrawer()
        {
            CloseFlyout();
            return LocalPopupHelper.OpenAddInspectionsDrawer();
        }

        private Task OnReportsClicked()
        {
            CloseFlyout();
            return MainNavigator.Current.GoToPageAsync(new ReportsPage());
        }

        private async Task OnLogout()
        {
            bool userAccepted = await App.Current.MainPage.DisplayAlert(
                TranslateExtension.Translator[nameof(GroupResourceEnum.Menu) + "/Logout"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/LogoutMessage"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Yes"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Cancel"]
            );

            if (userAccepted)
            {
                IAuthenticationProvider authentication = DependencyService.Resolve<IAuthenticationProvider>();
                await authentication.Logout();

                DependencyService.Resolve<ICommonLogout>()
                    .DeleteLocalInfo();
            }
        }

        private async Task OnOpenRootPage(string pageCode)
        {
            bool shouldGo = true;

            if (GlobalVariables.IsAddingInspection)
            {
                shouldGo = await App.Current.MainPage.DisplayAlert(
                    TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/AddingInspectionTitle"],
                    TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/AddingInspectionMessage"],
                    TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Yes"],
                    TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/No"]
                );
            }

            if (shouldGo)
            {
                await MainNavigator.Current.GoToPageAsync(pageCode);
            }
        }

        private void ClickVersionLabel_Clicked(object sender, EventArgs e)
        {
            versionTappedCount++;
            if (versionTappedCount == 7)
            {
                versionTappedCount = 0;
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await MainNavigator.Current.GoToPageAsync(new SystemInformationPage());
                });
            }
        }
    }
}