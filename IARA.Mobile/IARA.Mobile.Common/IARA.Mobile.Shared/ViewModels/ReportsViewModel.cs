using IARA.Mobile.Application.DTObjects.Reports;
using IARA.Mobile.Application.Interfaces.Transactions;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Shared.Menu;
using IARA.Mobile.Shared.ResourceTranslator;
using IARA.Mobile.Shared.ViewModels.Base;
using IARA.Mobile.Shared.ViewModels.Models;
using IARA.Mobile.Shared.Views;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace IARA.Mobile.Shared.ViewModels
{
    public class ReportsViewModel : BasePageViewModel
    {
        private LayoutState _state;
        private string _searchText = string.Empty;
        private List<ReportNodeDto> _reports;
        private bool _firstExecuteUWP = false;

        public ReportsViewModel()
        {
            IsBusy = false;
            Reload = CommandBuilder.CreateFrom(OnReload);
            GoToReport = CommandBuilder.CreateFrom<ReportNodeModel>(OnGoToReport);
            ExpandReport = CommandBuilder.CreateFrom<ReportModel>(OnExpandReport);
            StoppedTyping = CommandBuilder.CreateFrom(OnStoppedTyping);

            Reports = new TLObservableCollection<ReportModel>();
        }

        protected IReportsTransaction ReportsTransaction =>
            DependencyService.Resolve<IReportsTransaction>();

        protected ITranslationTransaction TranslationTransaction =>
            DependencyService.Resolve<ITranslationTransaction>();

        public LayoutState State
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public TLObservableCollection<ReportModel> Reports { get; }

        public ICommand Reload { get; }
        public ICommand GoToReport { get; }
        public ICommand ExpandReport { get; }
        public ICommand StoppedTyping { get; }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[] { GroupResourceEnum.Reports };
        }
        public override IReadOnlyDictionary<GroupResourceEnum, IReadOnlyDictionary<string, string>> GetPageResources(out GroupResourceEnum[] filtered)
        {
            filtered = Translator.Current.Filter(GetPageIndexes()).ToArray();
            return TranslationTransaction.GetPagesTranslations(filtered);
        }

        public override Task Initialize(object sender)
        {
            return OnReload();
        }

        private async Task OnReload()
        {
            if (IsBusy)
            {
                return;
            }

            State = LayoutState.Loading;
            IsBusy = true;
            _reports = await ReportsTransaction.GetAll();

            if (_reports == null)
            {
                State = LayoutState.Error;
                IsBusy = false;
                return;
            }

            Reports.ReplaceRange(MapModels(_reports));

            State = LayoutState.Success;
            IsBusy = false;
        }

        private void OnStoppedTyping()
        {
            if (_reports == null || (Device.RuntimePlatform == Device.UWP && !_firstExecuteUWP))
            {
                _firstExecuteUWP = true;
                return;
            }

            Reports.ReplaceRange(MapModels(_reports));
        }

        private async Task OnGoToReport(ReportNodeModel dto)
        {
            await TLLoadingHelper.ShowFullLoadingScreen();
            ReportDto report = await ReportsTransaction.Get(dto.Id);

            if (report != null)
            {
                await MainNavigator.Current.GoToPageAsync(new ReportPage(report));
            }

            await TLLoadingHelper.HideFullLoadingScreen();
        }

        private void OnExpandReport(ReportModel model)
        {
            if (model.HiddenChildren != null)
            {
                model.AddRange(model.HiddenChildren);
                model.HiddenChildren = null;
                model.IsExpanded = true;
            }
            else
            {
                model.HiddenChildren = model.ToList();
                model.Clear();
                model.IsExpanded = false;
            }
        }

        private List<ReportModel> MapModels(List<ReportNodeDto> dtos)
        {
            return dtos
                .Where(f => string.IsNullOrWhiteSpace(SearchText) || f.Children.Any(s => s.Name.Contains(SearchText)))
                .Select(f =>
                {
                    ReportModel model = new ReportModel
                    {
                        Id = f.Id,
                        Name = f.Name,
                    };

                    List<ReportNodeModel> children = f.Children
                        .Where(s => s.Name.Contains(SearchText))
                        .Select(s => new ReportNodeModel
                        {
                            Id = s.Id,
                            Name = s.Name,
                            Icon = s.Icon,
                            Parent = model
                        }).ToList();

                    model.AddRange(children);

                    return model;
                }).ToList();
        }
    }
}
