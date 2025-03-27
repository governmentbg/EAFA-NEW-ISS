using IARA.Mobile.Application.Attributes;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.ShipPickerDialog;
using IARA.Mobile.Insp.Helpers;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class CatchInspectionViewModel : ViewModel, IDisposable
    {
        private string _shipText;
        private bool isDisposed = false;
        private bool _isUnloadedQuantityRequired;
        public int LogBookId { get; set; }
        public CatchInspectionViewModel(InspectionPageViewModel inspection, CatchInspectionsViewModel catchInspections, bool isUnloadedQuantityRequired = false)
        {
            Inspection = inspection;
            CatchInspections = catchInspections;
            _isUnloadedQuantityRequired = isUnloadedQuantityRequired;

            FishTypeChosen = CommandBuilder.CreateFrom<SelectNomenclatureDto>(OnFishTypeChosen);
            OpenShipPicker = CommandBuilder.CreateFrom(OnOpenShipPicker);

            this.AddValidation();

            if (!catchInspections.ShowType)
            {
                CatchType.Validations.RemoveAt(CatchType.Validations.FindIndex(f => f.Name == nameof(RequiredAttribute)));
            }

            if (!isUnloadedQuantityRequired)
            {
                UnloadedQuantity.Validations.RemoveAt(UnloadedQuantity.Validations.FindIndex(f => f.Name == nameof(RequiredAttribute)));
                UnloadedQuantity.HasAsterisk = false;
                OnPropertyChanged(nameof(UnloadedQuantity));
            }

            UnloadedQuantity.PropertyChanged += OnValueChanged;
            TurbotSizeGroup.PropertyChanged += OnValueChanged;
            CatchQuantity.PropertyChanged += OnValueChanged;
        }
        public void Unsubscribe()
        {
            UnloadedQuantity.PropertyChanged -= OnValueChanged;
            TurbotSizeGroup.PropertyChanged -= OnValueChanged;
            CatchQuantity.PropertyChanged -= OnValueChanged;
        }
        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                Unsubscribe();
            }
        }

        private void OnValueChanged(object s, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ValidState.Value))
            {
                CatchInspections.SetSummary();
            }
        }

        public CatchInspectionsViewModel CatchInspections { get; set; }

        public InspectionPageViewModel Inspection { get; }

        public VesselDuringInspectionDto Ship { get; set; }

        [Required]
        public ValidStateSelect<SelectNomenclatureDto> FishType { get; set; }

        public ValidStateBool UndersizedFish { get; set; }

        [Required]
        public ValidStateSelect<SelectNomenclatureDto> CatchType { get; set; }

        [Required]
        [TLRange(0, 100000, true)]
        public ValidState CatchQuantity { get; set; }

        [TLRange(0, 100000, true)]
        public ValidState CatchCount { get; set; }

        [Required]
        [TLRange(0, 100000, true)]
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
            if (CatchInspections.Catches.Any(f => f != this && f.CatchType.Value?.Id == dto.Id))
            {
                FishType.Value = null;
            }
            CatchInspections.SetSummary();
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
                Undersized = viewModel.UndersizedFish.Value,
            };
        }
    }
}
