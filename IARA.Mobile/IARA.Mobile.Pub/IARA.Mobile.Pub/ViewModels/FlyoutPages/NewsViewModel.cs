using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Application;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Pub.Application.DTObjects.AddressNomenclatures.LocalDb;
using IARA.Mobile.Pub.Application.DTObjects.News;
using IARA.Mobile.Pub.ViewModels.Base;
using IARA.Mobile.Pub.ViewModels.FlyoutPages.News;
using IARA.Mobile.Pub.Views.FlyoutPages.News;
using IARA.Mobile.Shared.Menu;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.ViewModels.FlyoutPages
{
    public class NewsViewModel : MainPageViewModel
    {
        private bool endReached;
        private bool loadingHistory = true;
        private string _imageUrl;
        private string _freeTextSearch;
        public NewsViewModel()
        {
            News = new TLObservableCollection<NewsDto>();
            NewsDistricts = new TLObservableCollection<DistrictSelectDto>();
            LoadHistory = CommandBuilder.CreateFrom(OnLoadHistory);
            NewsTapped = CommandBuilder.CreateFrom<NewsDto>(OnNewsTapped);
            FilterChanged = CommandBuilder.CreateFrom(OnFilterChenged);
            this.AddValidation();//required when using validstates, even if validation is not required;
        }

        private async Task OnNewsTapped(NewsDto news)
        {
            NewsDetailsDto newsDetails = await NewsTransaction.GetNewsDetail(news.Id);
            if (newsDetails != null)
            {
                NewsDetailsViewModel newsViewModel = new NewsDetailsViewModel
                {
                    Title = newsDetails.Title,
                    PublishStart = newsDetails.PublishStart,
                    MainPhotoUrl = newsDetails.HasImage ? ImageUrl + newsDetails.Id : null,
                    Content = newsDetails.Content,
                };
                await MainNavigator.Current.GoToPageAsync(new NewsDetailsPage(newsViewModel));
            }
        }

        public TLObservableCollection<NewsDto> News { get; set; }
        public TLObservableCollection<DistrictSelectDto> NewsDistricts { get; }
        public bool LoadingHistory
        {
            get => loadingHistory;
            set => SetProperty(ref loadingHistory, value);
        }
        public string ImageUrl
        {
            get => _imageUrl;
            set => SetProperty(ref _imageUrl, value);
        }

        public ValidStateDate DateFrom { get; set; }

        public ValidStateDate DateTo { get; set; }

        public ValidStateMultiSelect<DistrictSelectDto> SelectedDistricts { get; set; }

        public string FreeTextSearch
        {
            get => _freeTextSearch;
            set => SetProperty(ref _freeTextSearch, value);
        }

        public ICommand LoadHistory { get; }
        public ICommand NewsTapped { get; }
        public ICommand FilterChanged { get; }
        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[] { GroupResourceEnum.News };
        }

        public override async Task Initialize(object sender)
        {
            IServerUrl serverUrl = DependencyService.Resolve<IServerUrl>();
            ImageUrl = serverUrl.BuildUrl("News/GetNewsMainPhoto", extension: "Services") + "?Id=";

            List<DistrictSelectDto> districts = NomenclaturesTransaction.GetDistricts();
            NewsDistricts.AddRange(districts);

            List<NewsDto> result = await NewsTransaction.GetPagedNews(null, CommonGlobalVariables.PullItemsCount, 1);

            if (result == null)
            {
                IsBusy = false;
                LoadingHistory = false;
                return;
            }

            if (result.Count < CommonGlobalVariables.PullItemsCount)
            {
                endReached = true;
            }

            News.AddRange(result);

            LoadingHistory = false;
            IsBusy = false;
        }

        private async Task OnLoadHistory()
        {
            if (endReached || IsBusy || LoadingHistory)
            {
                return;
            }

            LoadingHistory = true;

            NewsFiltersDto filters = new NewsFiltersDto
            {
                DateFrom = DateFrom?.Value,
                DateTo = DateTo?.Value,
                DistrictsIds = SelectedDistricts?.Value.Count > 0 ? SelectedDistricts.Select(x => x.Id).ToArray() : null,
                FreeTextSearch = _freeTextSearch,
            };

            List<NewsDto> result = await NewsTransaction.GetPagedNews(filters, CommonGlobalVariables.PullItemsCount, (News.Count / CommonGlobalVariables.PullItemsCount) + 1);

            if (result == null)
            {
                LoadingHistory = false;
                return;
            }

            if (result.Count < CommonGlobalVariables.PullItemsCount)
            {
                endReached = true;
            }

            News.AddRange(result);

            LoadingHistory = false;
        }

        private async Task OnFilterChenged()
        {
            while (LoadingHistory)
            {
                await Task.Delay(300);
            }
            endReached = false;
            News.Clear();
            await OnLoadHistory();
        }
    }
}
