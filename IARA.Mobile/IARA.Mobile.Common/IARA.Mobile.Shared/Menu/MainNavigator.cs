using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Shared.ResourceTranslator;
using IARA.Mobile.Shared.ViewModels.Intrerfaces;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using Xamarin.Forms;

namespace IARA.Mobile.Shared.Menu
{
    public class MainNavigator : FlyoutPage
    {
        public static MainNavigator Current { get; private set; }

        private readonly IFlyoutPage flyoutPage;
        private bool isNavigating;
        private Page currentPage;

        public MainNavigator(IFlyoutPage flyoutPage)
        {
            Current = this;
            Flyout = (this.flyoutPage = flyoutPage) as Page;
            Detail = new Page();

            flyoutPage.CloseFlyout = () => IsPresented = false;

            FlyoutLayoutBehavior = FlyoutLayoutBehavior.Popover;

            foreach (NavigationItemView navItem in flyoutPage.NavigationItems)
            {
                navItem.ItemTapped = CommandBuilder.CreateFrom<string>(NavigationItemTapped);
            }
        }

        public Task GoToPageAsync(string pagePath)
        {
            if (isNavigating)
            {
                return Task.CompletedTask;
            }
            isNavigating = true;

            Translator.Current.SoftClear();
            GoToPage(pagePath);

            return TLLoadingHelper.HideFullLoadingScreen();
        }

        public Task GoToPageAsync(Page page)
        {
            SetTitleView(page);
            return Detail.Navigation.PushAsync(page);
        }

        public Task PopPageAsync()
        {
            return Detail.Navigation.PopAsync();
        }

        public Task PopToRootAsync()
        {
            return Detail.Navigation.PopToRootAsync();
        }

        private void GoToPage(string pagePath)
        {
            if (!flyoutPage.Routes.TryGetValue(pagePath, out Func<Page> router))
            {
                throw new Exception($"Router path '{pagePath}' not found.");
            }
            SetCurrentPageAsSelected(pagePath);

            Page newCurrentPage = router();
            newCurrentPage.Appearing += PageAppearing;
            SetTitleView(newCurrentPage);

            if (currentPage is IDisposable disposable)
            {
                disposable.Dispose();
            }
            currentPage = newCurrentPage;

            NavigationPage navPage = new NavigationPage(currentPage)
            {
                BarBackgroundColor = (Color)Xamarin.Forms.Application.Current.Resources["Primary"],
                BarTextColor = Color.White
            };
            Detail = navPage;
            navPage.Popped += PagePopped;
        }

        private void SetTitleView(Page page)
        {
            if (NavigationPage.GetTitleView(page) == null)
            {
                TitleView titleView = new TitleView(page as IBasePage, DependencyService.Resolve<IConnectivity>());

                page.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
                {
                    if (e.PropertyName == TitleProperty.PropertyName)
                    {
                        titleView.TitleText = page.Title;
                    }
                };

                NavigationPage.SetTitleView(page, titleView);
            }
        }

        private void SetCurrentPageAsSelected(string pagePath)
        {
            foreach (NavigationItemView navItem in flyoutPage.NavigationItems)
            {
                navItem.IsSelected = false;
            }

            NavigationItemView selectedItem = flyoutPage.NavigationItems.FirstOrDefault(f => f.Route == pagePath);
            if (selectedItem != null)
            {
                selectedItem.IsSelected = true;
            }
        }

        private void PageAppearing(object sender, EventArgs e)
        {
            currentPage.Appearing -= PageAppearing;
            IsPresented = false;
            isNavigating = false;
        }

        private void PagePopped(object sender, NavigationEventArgs e)
        {
            IBasePage page = e.Page as IBasePage;
            Translator.Current.Remove(page.AddedResources);

            if (page is IDisposable pageDisposable)
            {
                pageDisposable.Dispose();
            }

            if (e.Page.BindingContext is IDisposable viewModelDisposable)
            {
                viewModelDisposable.Dispose();
            }
        }

        private Task NavigationItemTapped(string path)
        {
            return GoToPageAsync(path);
        }
    }
}