using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using IARA.Mobile.Application.Interfaces.Transactions;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Pub.Application.Interfaces.Transactions;
using IARA.Mobile.Pub.Application.Interfaces.Utilities;
using IARA.Mobile.Pub.Views.FlyoutPages;
using IARA.Mobile.Shared.Menu;
using IARA.Mobile.Shared.ResourceTranslator;
using IARA.Mobile.Shared.Views;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.ResourceTranslator;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Pub.Views.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainFlyoutPage : ContentPage, IFlyoutPage
    {
        private int versionTappedCount;

        public MainFlyoutPage(
            bool hasScientificFishingPermission,
            bool hasTicketPermission,
            bool hasCatchRecordsPermission,
            bool hasReportsPermission,
            bool hasReportViolationPermission,
            bool hasProfilePermission)
        {
            InitializeComponent();

            scientificFishingPage.IsVisible = hasScientificFishingPermission;
            fishingTicketPage.IsVisible = hasTicketPermission;
            myTicketsPage.IsVisible = hasTicketPermission;
            catchRecordsPage.IsVisible = hasCatchRecordsPermission;
            reportsPage.IsVisible = hasReportsPermission;
            reportViolationPage.IsVisible = hasReportViolationPermission;
            profileNavItem.IsVisible = hasProfilePermission;

            VersionSpan.Text = VersionTracking.CurrentVersion;

            Routes = new Dictionary<string, Func<Page>>
            {
                {
                    nameof(HomePage),
                    () => new HomePage()
                },
                {
                    nameof(FishingTicketPage),
                    () => new FishingTicketPage()
                },
                {
                    nameof(ProfilePage),
                    () => new ProfilePage()
                },
                {
                    nameof(CatchRecordsPage),
                    () => new CatchRecordsPage()
                },
                {
                    nameof(ReportsPage),
                    () => new ReportsPage()
                },
                {
                    nameof(NewsPage),
                    () => new NewsPage()
                },
                {
                    nameof(ScientificFishingPage),
                    () => new ScientificFishingPage()
                },
                {
                    nameof(MyTicketsPage),
                    () => new MyTicketsPage()
                },
                {
                    nameof(ReportViolationPage),
                    () => new ReportViolationPage()
                },
                {
                    nameof(SystemInformationPage),
                    () => new SystemInformationPage()
                }
            };

            NavigationItems = new List<NavigationItemView>(FilterNavItems(NavigationItemsLayout.Children))
            {
                profileNavItem
            };

            logoutNavItem.ItemTapped = CommandBuilder.CreateFrom(OnLogout);
            languageNavItem.ItemTapped = CommandBuilder.CreateFrom(OnChangeLanguage);
        }

        public IReadOnlyList<NavigationItemView> NavigationItems { get; }
        public IReadOnlyDictionary<string, Func<Page>> Routes { get; }
        public Action CloseFlyout { get; set; }

        private IEnumerable<NavigationItemView> FilterNavItems(IEnumerable<View> views)
        {
            foreach (View view in views)
            {
                if (view is NavigationItemView navView)
                {
                    yield return navView;
                }
                else if (view is DropdownNavigationItemView dropdownNavView)
                {
                    foreach (NavigationItemView item in FilterNavItems((dropdownNavView.Content as StackLayout).Children))
                    {
                        yield return item;
                    }
                }
            }
        }

        private async Task OnLogout()
        {
            bool userAccepted = await App.Current.MainPage.DisplayAlert(TranslateExtension.Translator[nameof(GroupResourceEnum.Menu) + "/Logout"],
            TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/LogoutMessage"],
            TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Yes"], TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Cancel"]);

            if (userAccepted)
            {
                IAuthenticationProvider authentication = DependencyService.Resolve<IAuthenticationProvider>();
                if(await DependencyService.Resolve<IAuthenticationTransaction>().LogOut())
                {
                    await authentication.Logout();
                    DependencyService.Resolve<ICommonLogout>()
                        .DeleteLocalInfo();
                }
            }
        }

        private async Task OnChangeLanguage()
        {
            string result = await App.Current.MainPage.DisplayActionSheet(
                TranslateExtension.Translator[nameof(GroupResourceEnum.Menu) + "/ChooseLanguage"],
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

            ITranslationTransaction translationTransaction = DependencyService.Resolve<ITranslationTransaction>();

            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfoByIetfLanguageTag(iso);
            settings.CurrentResourceLanguage = languageEnum;

            List<GroupResourceEnum> groups = Translator.Current.GetResourceGroups();
            IReadOnlyDictionary<GroupResourceEnum, IReadOnlyDictionary<string, string>> resources =
                translationTransaction.GetPagesTranslations(groups);

            Translator.Current.HardClear();
            Translator.Current.Add(resources);
            Translator.Current.Invalidate();
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