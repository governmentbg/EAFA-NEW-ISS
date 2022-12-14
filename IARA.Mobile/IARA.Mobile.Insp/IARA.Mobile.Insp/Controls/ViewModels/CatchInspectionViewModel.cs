using IARA.Mobile.Application.Attributes;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.ShipPickerDialog;
using IARA.Mobile.Insp.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class CatchInspectionViewModel : ViewModel
    {
        private readonly CatchInspectionsViewModel _catchInspections;
        private string _shipText;

        public CatchInspectionViewModel(InspectionPageViewModel inspection, CatchInspectionsViewModel catchInspections)
        {
            Inspection = inspection;
            _catchInspections = catchInspections;

            FishTypeChosen = CommandBuilder.CreateFrom<SelectNomenclatureDto>(OnFishTypeChosen);
            OpenShipPicker = CommandBuilder.CreateFrom(OnOpenShipPicker);

            this.AddValidation();
        }

        public InspectionPageViewModel Inspection { get; }

        public VesselDuringInspectionDto Ship { get; set; }

        [Required]
        public ValidStateSelect<SelectNomenclatureDto> FishType { get; set; }

        [Required]
        public ValidStateSelect<SelectNomenclatureDto> CatchType { get; set; }

        [Required]
        [TLRange(1, 10000, true)]
        public ValidState CatchQuantity { get; set; }

        [TLRange(1, 10000, true)]
        public ValidState CatchCount { get; set; }

        [TLRange(1, 10000, true)]
        public ValidState UnloadedQuantity { get; set; }

        [TLRange(0, 100, true)]
        public ValidState AllowedDeviation { get; set; }

        public ValidStateSelect<CatchZoneNomenclatureDto> CatchArea { get; set; }

        [TLRange(0, 10000, true)]
        public ValidState AverageSize { get; set; }

        public ValidStateSelect<SelectNomenclatureDto> TurbotSizeGroup { get; set; }

        public ValidStateSelect<SelectNomenclatureDto> FishSex { get; set; }

        public string ShipText
        {
            get => _shipText;
            set => SetProperty(ref _shipText, value);
        }

        public ICommand FishTypeChosen { get; }
        public ICommand OpenShipPicker { get; }

        private void OnFishTypeChosen(SelectNomenclatureDto dto)
        {
            if (_catchInspections.Catches.Any(f => f != this && f.CatchType.Value?.Id == dto.Id))
            {
                FishType.Value = null;
            }
        }

        private async Task OnOpenShipPicker()
        {
            (string text, VesselDuringInspectionDto ship) = await TLDialogHelper.ShowDialog(new ShipPickerDialog(Inspection, Ship));

            Ship = ship;
            ShipText = text;
        }

        public static implicit operator InspectionCatchMeasureDto(CatchInspectionViewModel viewModel)
        {
            return new InspectionCatchMeasureDto
            {
                CatchInspectionTypeId = viewModel.CatchType.Value,
                FishId = viewModel.FishType.Value,
                CatchQuantity = ParseHelper.ParseDecimal(viewModel.CatchQuantity.Value),
                AllowedDeviation = ParseHelper.ParseDecimal(viewModel.AllowedDeviation.Value),
                UnloadedQuantity = ParseHelper.ParseDecimal(viewModel.UnloadedQuantity.Value),
                CatchZoneId = viewModel.CatchArea.Value,
                AverageSize = ParseHelper.ParseDecimal(viewModel.AverageSize.Value),
                FishSexId = viewModel.FishSex.Value,
                CatchCount = ParseHelper.ParseInteger(viewModel.CatchCount.Value),
                OriginShip = viewModel.Ship,
                TurbotSizeGroupId = viewModel.TurbotSizeGroup.Value,
            };
        }
    }
}
