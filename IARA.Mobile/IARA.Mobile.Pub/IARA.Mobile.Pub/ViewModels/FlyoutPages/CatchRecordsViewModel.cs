using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Pub.Application.DTObjects.CatchRecords;
using IARA.Mobile.Pub.ViewModels.Base;
using IARA.Mobile.Pub.Views.FlyoutPages.CatchRecords;
using IARA.Mobile.Shared.Menu;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using Xamarin.CommunityToolkit.UI.Views;

namespace IARA.Mobile.Pub.ViewModels.FlyoutPages
{
    public class CatchRecordsViewModel : MainPageViewModel
    {
        private LayoutState _state;

        public CatchRecordsViewModel()
        {
            _state = LayoutState.Loading;

            CatchRecords = new TLObservableCollection<CatchRecordDto>();

            Review = CommandBuilder.CreateFrom<CatchRecordDto>(OnReview);
            Add = CommandBuilder.CreateFrom(OnAdd);
            Edit = CommandBuilder.CreateFrom<CatchRecordDto>(OnEdit);
            Delete = CommandBuilder.CreateFrom<CatchRecordDto>(OnDelete);
            Reload = CommandBuilder.CreateFrom(OnReload);
            ShowOfflineInfo = CommandBuilder.CreateFrom(OnShowOfflineInfo);

            CommonConnectivity.ConnectivityChanged += OnConnectivityChanged;
        }

        public TLObservableCollection<CatchRecordDto> CatchRecords { get; }

        public LayoutState State
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }

        public ICommand Review { get; }
        public ICommand Add { get; }
        public ICommand Edit { get; }
        public ICommand Delete { get; }
        public ICommand Reload { get; }
        public ICommand ShowOfflineInfo { get; }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[] { GroupResourceEnum.CatchRecords };
        }

        public override async void OnAppearing()
        {
            await OnReload();
        }

        public override Task Initialize(object sender)
        {
            return Task.CompletedTask;
        }

        private async Task OnReload()
        {
            State = LayoutState.Loading;

            List<CatchRecordDto> catchRecords = await CatchRecordsTransaction.GetCatchRecords(null);
            if (catchRecords == null)
            {
                return;
            }

            if (catchRecords.Count == 0)
            {
                State = LayoutState.Empty;
            }
            else
            {
                CatchRecords.ReplaceRange(catchRecords);
                State = LayoutState.Success;
            }
        }

        private Task OnReview(CatchRecordDto dto)
        {
            return MainNavigator.Current.GoToPageAsync(new AddCatchRecordPage(dto));
        }

        private Task OnAdd()
        {
            if (FishingTicketsTransaction.GetActiveTicketsCount() == 0)
            {
                return App.Current.MainPage.DisplayAlert(
                    TranslateExtension.Translator[nameof(GroupResourceEnum.CatchRecords) + "/NoTicketsTitle"],
                    TranslateExtension.Translator[nameof(GroupResourceEnum.CatchRecords) + "/NoTicketsMessage"],
                    TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Ok"]
                );
            }

            return MainNavigator.Current.GoToPageAsync(new AddCatchRecordPage(canEdit: true, isAdd: true));
        }

        private Task OnEdit(CatchRecordDto dto)
        {
            return MainNavigator.Current.GoToPageAsync(new AddCatchRecordPage(dto, canEdit: true));
        }

        private async Task OnDelete(CatchRecordDto dto)
        {
            bool result = await App.Current.MainPage.DisplayAlert(
                TranslateExtension.Translator[nameof(GroupResourceEnum.CatchRecords) + "/DeleteCatchTitle"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.CatchRecords) + "/DeleteCatchMessage"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Yes"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/No"]
            );

            if (result)
            {
                await CatchRecordsTransaction.DeleteCatchRecord(dto.Id);

                CatchRecords.Remove(dto);

                if (CatchRecords.Count == 0)
                {
                    State = LayoutState.Empty;
                }
            }
        }

        private Task OnShowOfflineInfo()
        {
            return App.Current.MainPage.DisplayAlert(
                null,
                TranslateExtension.Translator[nameof(GroupResourceEnum.CatchRecords) + "/OfflineCatchMessage"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Ok"]
            );
        }

        private async void OnConnectivityChanged(object sender, InternetStatus e)
        {
            await OnReload();
        }
    }
}
