using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Insp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class LogBooksViewModel : ViewModel
    {
        private ShipCatchesViewModel _shipCatches;
        private FishingGearsViewModel _fishingGears;
        public LogBooksViewModel(InspectionPageViewModel inspection, ShipCatchesViewModel shipCatches, FishingGearsViewModel fishingGears)
        {
            Inspection = inspection;
            _shipCatches = shipCatches;
            _fishingGears = fishingGears;
            Add = CommandBuilder.CreateFrom(OnAdd);
            Remove = CommandBuilder.CreateFrom<LogBookModel>(OnRemove);

            this.AddValidation();
        }

        public InspectionPageViewModel Inspection { get; }

        public ValidStateValidatableTable<LogBookModel> LogBooks { get; set; }

        public ICommand Add { get; }
        public ICommand Remove { get; }

        public async Task OnEdit(List<InspectionLogBookDto> logBooks)
        {
            if (logBooks == null || logBooks.Count == 0)
            {
                return;
            }

            List<LogBookPageDto> pages = await InspectionsTransaction.GetLogBookPages(logBooks.FindAll(f => f.LogBookId.HasValue).ConvertAll(f => f.LogBookId.Value));

            LogBooks.Value.AddRange(logBooks.ConvertAll(f =>
            {
                LogBookModel model = new LogBookModel(_shipCatches, _fishingGears)
                {
                    Dto = f
                };

                model.EndPage.Value = f.EndPage?.ToString() ?? "0";
                model.AddedByInspector = f.LogBookId == null;
                model.Corresponds.Value = f.CheckValue?.ToString();
                model.Number.AssignFrom(f.Number);
                model.Description.AssignFrom(f.Description);
                model.PageText.AssignFrom(f.PageNum);

                if (f.LogBookId != null && f.PageNum != null)
                {
                    model.PageSelect.Value = new LogBookPageDto
                    {
                        Id = f.PageId ?? -1,
                        LogBookId = f.LogBookId.Value,
                        PageNum = f.PageNum,
                    };
                    model.Pages = pages?.FindAll(s => s.LogBookId == f.LogBookId.Value).OrderByDescending(x => x.IssuedOn).ToList() ?? new List<LogBookPageDto>();
                }

                return model;
            }));
        }

        private void OnAdd()
        {
            LogBookModel model = new LogBookModel(_shipCatches, _fishingGears)
            {
                AddedByInspector = true,
                Dto = new InspectionLogBookDto(),
            };
            model.Corresponds.Value = nameof(CheckTypeEnum.N);
            LogBooks.Value.Add(model);
        }

        private async Task OnRemove(LogBookModel model)
        {
            bool result = await App.Current.MainPage.DisplayAlert(null,
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/DeleteMessage"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Yes"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/No"]
            );

            if (result)
            {
                LogBooks.Value.Remove(model);
            }
        }

        public static implicit operator List<InspectionLogBookDto>(LogBooksViewModel viewModel)
        {
            return viewModel == null
                ? new List<InspectionLogBookDto>()
                : viewModel.LogBooks
                    .Select(f => new InspectionLogBookDto
                    {
                        CheckValue = Enum.TryParse(f.Corresponds.Value, out CheckTypeEnum checkType)
                            ? (CheckTypeEnum?)checkType
                            : null,
                        Description = f.Description,
                        Number = f.Number,
                        From = f.Dto.From,
                        Id = f.Dto.Id,
                        LogBookId = f.Dto.LogBookId,
                        EndPage = f.Dto.EndPage,
                        StartPage = f.Dto.StartPage,
                        PageId = f.PageSelect.Value == null || f.PageSelect.Value.Id == -1
                            ? null
                            : f.PageSelect.Value?.Id,
                        PageNum = f.AddedByInspector
                            ? f.PageText.Value
                            : f.PageSelect.Value?.PageNum,
                    })
                    .ToList();
        }
    }
}
