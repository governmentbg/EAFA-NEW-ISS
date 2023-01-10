using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Attributes;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Helpers;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class CatchInspectionsViewModel : ViewModel
    {
        private List<SelectNomenclatureDto> _fishTypes;
        private List<SelectNomenclatureDto> _catchTypes;
        private List<CatchZoneNomenclatureDto> _catchAreas;
        private List<SelectNomenclatureDto> _fishSexTypes;
        private List<SelectNomenclatureDto> _turbotSizeGroups;

        public CatchInspectionsViewModel(
            InspectionPageViewModel inspection,
            bool showCatchArea = true,
            bool showAllowedDeviation = true,
            bool showUnloadedQuantity = false,
            bool showOriginShip = false,
            bool showAverageSize = false,
            bool showFishSex = false
        )
        {
            Inspection = inspection;
            ShowCatchArea = showCatchArea;
            ShowAllowedDeviation = showAllowedDeviation;
            ShowUnloadedQuantity = showUnloadedQuantity;
            ShowOriginShip = showOriginShip;
            ShowAverageSize = showAverageSize;
            ShowFishSex = showFishSex;

            AddCatch = CommandBuilder.CreateFrom(OnAddCatch);
            RemoveCatch = CommandBuilder.CreateFrom<CatchInspectionViewModel>(OnRemoveCatch);

            this.AddValidation();
        }

        public InspectionPageViewModel Inspection { get; }

        public bool ShowCatchArea { get; }
        public bool ShowAllowedDeviation { get; }
        public bool ShowUnloadedQuantity { get; }
        public bool ShowOriginShip { get; }
        public bool ShowAverageSize { get; }
        public bool ShowFishSex { get; }

        [DuplicateMarketCatches(ErrorMessageResourceName = "DuplicateCatches")]
        [ListMinLength(1)]
        public ValidStateValidatableTable<CatchInspectionViewModel> Catches { get; set; }

        public List<SelectNomenclatureDto> FishTypes
        {
            get => _fishTypes;
            private set => SetProperty(ref _fishTypes, value);
        }
        public List<SelectNomenclatureDto> CatchTypes
        {
            get => _catchTypes;
            private set => SetProperty(ref _catchTypes, value);
        }
        public List<CatchZoneNomenclatureDto> CatchAreas
        {
            get => _catchAreas;
            private set => SetProperty(ref _catchAreas, value);
        }
        public List<SelectNomenclatureDto> FishSexTypes
        {
            get => _fishSexTypes;
            private set => SetProperty(ref _fishSexTypes, value);
        }
        public List<SelectNomenclatureDto> TurbotSizeGroups
        {
            get => _turbotSizeGroups;
            private set => SetProperty(ref _turbotSizeGroups, value);
        }

        public ICommand AddCatch { get; }
        public ICommand RemoveCatch { get; }

        public void Init(
            List<SelectNomenclatureDto> fishTypes,
            List<SelectNomenclatureDto> catchTypes,
            List<CatchZoneNomenclatureDto> catchAreas,
            List<SelectNomenclatureDto> turbotSizeGroups,
            List<SelectNomenclatureDto> fishSexTypes
        )
        {
            FishTypes = fishTypes;
            CatchTypes = catchTypes;
            CatchAreas = catchAreas;
            FishSexTypes = fishSexTypes;
            TurbotSizeGroups = turbotSizeGroups;
        }

        public void OnEdit(List<InspectionCatchMeasureDto> catchMeasures)
        {
            if (catchMeasures == null || catchMeasures.Count == 0)
            {
                return;
            }

            Catches.Value.AddRange(catchMeasures.ConvertAll(f =>
            {
                CatchInspectionViewModel viewModel = new CatchInspectionViewModel(Inspection, this);

                viewModel.CatchQuantity.AssignFrom(f.CatchQuantity);
                viewModel.UnloadedQuantity.AssignFrom(f.UnloadedQuantity);
                viewModel.AllowedDeviation.AssignFrom(f.AllowedDeviation);
                viewModel.CatchType.AssignFrom(f.CatchInspectionTypeId, CatchTypes);
                viewModel.CatchArea.AssignFrom(f.CatchZoneId, CatchAreas);
                viewModel.FishType.AssignFrom(f.FishId, FishTypes);
                viewModel.AverageSize.AssignFrom(f.AverageSize);
                viewModel.FishSex.AssignFrom(f.FishSexId, FishSexTypes);
                viewModel.CatchCount.AssignFrom(f.CatchCount);
                viewModel.TurbotSizeGroup.AssignFrom(f.TurbotSizeGroupId, TurbotSizeGroups);

                if (f.OriginShip != null)
                {
                    viewModel.Ship = f.OriginShip;
                    viewModel.ShipText = $"{f.OriginShip.Name} ({f.OriginShip.CFR})";
                }

                return viewModel;
            }));
        }

        private void OnAddCatch()
        {
            Catches.Value.Add(new CatchInspectionViewModel(Inspection, this));
        }

        private void OnRemoveCatch(CatchInspectionViewModel catchInspection)
        {
            Catches.Value.Remove(catchInspection);
        }

        public static implicit operator List<InspectionCatchMeasureDto>(CatchInspectionsViewModel viewModel)
        {
            return viewModel.Catches.Select(f => (InspectionCatchMeasureDto)f).ToList();
        }
    }
}
