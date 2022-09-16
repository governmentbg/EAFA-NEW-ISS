using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Application.DTObjects.Reports;
using IARA.Mobile.Application.Interfaces.Transactions;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Shared.ResourceTranslator;
using IARA.Mobile.Shared.ViewModels.Base;
using IARA.Mobile.Shared.ViewModels.Models;
using IARA.Mobile.Shared.ViewModels.Models.ReportParameters;
using System;
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
    public class ReportViewModel : BasePageViewModel, IPageViewModel
    {
        private long _totalRecordsCount;
        private LayoutState _executeState;
        private bool _columnNamesPulled;
        private int _pagesCount;
        private int _currentPage = 1;
        public ReportViewModel()
        {
            ExecuteReport = CommandBuilder.CreateFrom(OnExecuteReport);
            ReportPageSelected = CommandBuilder.CreateFrom<int>(OnReportPageSelected);
            GoToFirstPage = CommandBuilder.CreateFrom(OnGoToFirstPage);
            GoToPreviousPage = CommandBuilder.CreateFrom(OnGoToPreviousPage);
            GoToNextPage = CommandBuilder.CreateFrom(OnGoToNextPage);
            GoToLastPage = CommandBuilder.CreateFrom(OnGoToLastPage);
            Pages = new TLObservableCollection<int>();
            ReportResult = new TLObservableCollection<Dictionary<string, object>>();

            ColumnNames = new TLObservableCollection<ReportColumnNameDto>();
            Parameters = new TLObservableCollection<ReportParameterModel>();
            this.AddValidation();
        }

        protected IReportsTransaction ReportsTransaction =>
            DependencyService.Resolve<IReportsTransaction>();

        protected ITranslationTransaction TranslationTransaction =>
            DependencyService.Resolve<ITranslationTransaction>();

        public ReportDto Report { get; set; }

        public LayoutState ExecuteState
        {
            get => _executeState;
            set => SetProperty(ref _executeState, value);
        }

        public TLObservableCollection<Dictionary<string, object>> ReportResult { get; }

        public bool HasParameters { get; set; }

        public TLObservableCollection<ReportColumnNameDto> ColumnNames { get; }

        public TLObservableCollection<ReportParameterModel> Parameters { get; }

        public long TotalRecordsCount
        {
            get => _totalRecordsCount;
            set => SetProperty(ref _totalRecordsCount, value);
        }
        public int CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }
        public int PagesCount
        {
            get => _pagesCount;
            set => SetProperty(ref _pagesCount, value);
        }

        public TLObservableCollection<int> Pages { get; set; }

        public ICommand ExecuteReport { get; }
        public ICommand ReportPageSelected { get; }
        public ICommand GoToFirstPage { get; }
        public ICommand GoToPreviousPage { get; }
        public ICommand GoToNextPage { get; }
        public ICommand GoToLastPage { get; }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[] { GroupResourceEnum.Report, GroupResourceEnum.Validation };
        }

        public override IReadOnlyDictionary<GroupResourceEnum, IReadOnlyDictionary<string, string>> GetPageResources(out GroupResourceEnum[] filtered)
        {
            filtered = Translator.Current.Filter(GetPageIndexes()).ToArray();
            return TranslationTransaction.GetPagesTranslations(filtered);
        }

        public void Init(List<(string, object)> defaultValues)
        {
            HasParameters = Report.Parameters?.Count > 0;

            if (HasParameters)
            {
                List<ReportParameterModel> parameters = Report.Parameters.ConvertAll(f =>
                {
                    (string, object)? defaultValue = defaultValues?.Find(s => s.Item1 == f.Code);

                    return new ReportParameterModel(CreateReportValidation(f, defaultValue))
                    {
                        Id = f.Id,
                        Code = f.Code,
                        Name = f.Name,
                        Type = f.DataType,
                    };
                });

                foreach (ReportParameterModel parameter in parameters)
                {
                    Validation.OtherValidations.Add(parameter.Validation);
                }

                Parameters.AddRange(parameters);
            }
        }

        public override async Task Initialize(object sender)
        {
            if (!HasParameters)
            {
                await OnExecuteReport();
            }
        }

        private async Task OnExecuteReport()
        {
            Validation.Force();

            if (!Validation.IsValid)
            {
                return;
            }

            List<ExecutionParamDto> parameters = Parameters.Select(f => new ExecutionParamDto
            {
                Name = f.Code,
                Type = f.Type,
                Value = f.ValidParam.GetValue()
            }).ToList();

            if (Report.ReportType == ReportType.SQL)
            {
                IsBusy = true;
                ExecuteState = LayoutState.Loading;
                ReportResult.Clear();
                if (!_columnNamesPulled)
                {
                    List<ReportColumnNameDto> columnNames = await ReportsTransaction.GetColumnNames(Report.Id, parameters);

                    if (columnNames == null)
                    {
                        ExecuteState = LayoutState.Error;
                        IsBusy = false;
                        return;
                    }

                    ColumnNames.AddRange(columnNames);
                    _columnNamesPulled = true;
                }

                ReportResultDto result = await ReportsTransaction.ExecuteSQL(Report.Id, parameters, CurrentPage, CommonGlobalVariables.PullItemsCount);

                if (result == null)
                {
                    ExecuteState = LayoutState.Error;
                }
                else if (result.Records.Count == 0)
                {
                    ExecuteState = LayoutState.Empty;
                }
                else
                {
                    ReportResult.ReplaceRange(result.Records);
                    TotalRecordsCount = result.TotalRecordsCount;
                    UpdatePages();
                    ExecuteState = LayoutState.Success;
                }

                IsBusy = false;
            }
            else
            {
                IDownloader downloader = DependencyService.Resolve<IDownloader>();

                await downloader.DownloadFile(
                    Report.Name + ".pdf",
                    "application/pdf",
                    "Report/DownloadReport",
                    new
                    {
                        id = Report.Id,
                        parameters = parameters
                    }
                );
            }
        }

        private async Task OnReportPageSelected(int selectedPage)
        {
            CurrentPage = selectedPage;
            await OnExecuteReport();
        }

        private void UpdatePages()
        {
            decimal pagesCount = (decimal)TotalRecordsCount / CommonGlobalVariables.PullItemsCount;
            PagesCount = pagesCount > 1 ? (int)Math.Ceiling(pagesCount) : 0;
            if (PagesCount > 1)
            {
                Pages.Clear();
                if (CurrentPage + 2 > PagesCount && PagesCount > 3)
                {
                    Pages.AddRange(new List<int> { PagesCount - 2, PagesCount - 1, PagesCount });
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        int page = CurrentPage + i;
                        if (page <= PagesCount)
                        {
                            Pages.Add(page);
                        }
                    }
                }
            }
        }

        private async Task OnGoToFirstPage()
        {
            CurrentPage = 1;
            await OnExecuteReport();
        }

        private async Task OnGoToLastPage()
        {
            CurrentPage = PagesCount;
            await OnExecuteReport();
        }

        private async Task OnGoToNextPage()
        {
            int nextPage = CurrentPage + 1;
            if (nextPage <= PagesCount)
            {
                CurrentPage = nextPage;
                await OnExecuteReport();
            }
        }

        private async Task OnGoToPreviousPage()
        {
            int previousPage = CurrentPage - 1;
            if (previousPage > 0)
            {
                CurrentPage = previousPage;
                await OnExecuteReport();
            }
        }

        private IReportValidation CreateReportValidation(ReportParameterDto dto, (string, object)? defaultValue)
        {
            string defaultValueLocal = defaultValue?.Item2?.ToString() ?? dto.DefaultValue;

            return dto.DataType switch
            {
                ReportParameterType.Month => new MonthReportValidation(dto.IsMandatory, defaultValueLocal, dto.Pattern, dto.ErrorMessage),
                ReportParameterType.Year => new YearReportValidation(dto.IsMandatory, defaultValueLocal, dto.Pattern, dto.ErrorMessage),
                ReportParameterType.Int => new IntReportValidation(dto.IsMandatory, defaultValueLocal, dto.Pattern, dto.ErrorMessage),
                ReportParameterType.Decimal => new DecimalReportValidation(dto.IsMandatory, defaultValueLocal, dto.Pattern, dto.ErrorMessage),
                ReportParameterType.Date => new DateReportValidation(dto.IsMandatory, defaultValueLocal, dto.Pattern, dto.ErrorMessage),
                ReportParameterType.Time => new TimeReportValidation(dto.IsMandatory, defaultValueLocal, dto.Pattern, dto.ErrorMessage),
                ReportParameterType.DateTime => new DateTimeReportValidation(dto.IsMandatory, defaultValueLocal, dto.Pattern, dto.ErrorMessage),
                ReportParameterType.String => new StringReportValidation(dto.IsMandatory, defaultValueLocal, dto.Pattern, dto.ErrorMessage),
                ReportParameterType.Nomenclature => new NomenclatureReportValidation(dto.IsMandatory, dto.Nomenclatures?.ConvertAll(f => new SelectNomenclatureDto
                {
                    Code = f.Code,
                    Id = f.Value,
                    Name = f.DisplayName
                }), dto.Pattern, dto.ErrorMessage),
                ReportParameterType.NomenclatureMultiSelect => new NomenclatureMultiSelectReportValidation(dto.IsMandatory, dto.Nomenclatures?.ConvertAll(f => new SelectNomenclatureDto
                {
                    Code = f.Code,
                    Id = f.Value,
                    Name = f.DisplayName
                }), dto.Pattern, dto.ErrorMessage),
                _ => null,
            };
        }
    }
}
