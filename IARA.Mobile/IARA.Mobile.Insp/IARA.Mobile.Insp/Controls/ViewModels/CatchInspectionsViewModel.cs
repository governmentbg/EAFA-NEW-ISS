using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Attributes;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.FlyoutPages.Inspections.HarbourInspection;
using IARA.Mobile.Insp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Attributes;
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
        private FishingGearsViewModel _fishingGears;
        private string _summary;
        public CatchInspectionsViewModel(
            InspectionPageViewModel inspection,
            FishingGearsViewModel fishingGears,
            bool showCatchArea = true,
            bool showAllowedDeviation = true,
            bool showOriginShip = false,
            bool showAverageSize = false,
            bool showFishSex = false,
            bool showType = true,
            bool showUndersizedCheck = false,
            bool showTurbotSizeGroups = true,
            bool isUnloadedQuantityRequired = false,
            bool showUnloadedQuantity = true
        )
        {
            Inspection = inspection;
            ShowCatchArea = showCatchArea;
            ShowAllowedDeviation = showAllowedDeviation;
            ShowOriginShip = showOriginShip;
            ShowAverageSize = showAverageSize;
            ShowFishSex = showFishSex;
            ShowType = showType;
            ShowUndersizedCheck = showUndersizedCheck;
            ShowTurbotSizeGroups = showTurbotSizeGroups;
            IsUnloadedQuantityRequired = isUnloadedQuantityRequired;
            ShowUnloadedQuantity = showUnloadedQuantity;
            _fishingGears = fishingGears;

            AddCatch = CommandBuilder.CreateFrom(OnAddCatch);
            RemoveCatch = CommandBuilder.CreateFrom<CatchInspectionViewModel>(OnRemoveCatch);

            this.AddValidation(
                groups: new Dictionary<string, Func<bool>>
                {
                    { Group.IS_TRANSHIPMENT, () => HarbourInspectionViewModel.Static != null ? HarbourInspectionViewModel.Static.HasTranshipment.Value : true}
                });
        }

        public InspectionPageViewModel Inspection { get; }

        public bool ShowCatchArea { get; }
        public bool ShowAllowedDeviation { get; }
        public bool ShowOriginShip { get; }
        public bool ShowAverageSize { get; }
        public bool ShowFishSex { get; }
        public bool ShowType { get; }
        public bool ShowUndersizedCheck { get; }
        public bool ShowTurbotSizeGroups { get; }
        public bool ShowUnloadedQuantity { get; }

        [DuplicateMarketCatches]
        [AtLeastOne(ErrorMessageResourceName = "AtLeastOneCatch")]
        [ValidGroup(Group.IS_TRANSHIPMENT)]
        public ValidStateValidatableTable<CatchInspectionViewModel> Catches { get; set; }

        public void OnSwitchTransshipping(bool hasTransshipping)
        {
            TLValidator validator = Catches.Validations.Where(f => f.Name == nameof(AtLeastOneAttribute)).FirstOrDefault();
            if (hasTransshipping)
            {
                if (validator == null)
                {
                    Catches.Validations.Add(new TLValidator(new AtLeastOneAttribute(), nameof(AtLeastOneAttribute)));
                }
            }
            else
            {
                if (validator != null)
                {
                    Catches.Validations.Remove(validator);
                }
            }
        }

        public bool IsUnloadedQuantityRequired { get; set; }

        public string Summary
        {
            get => _summary;
            set => SetProperty(ref _summary, value);
        }

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
                CatchInspectionViewModel viewModel = new CatchInspectionViewModel(Inspection, this, IsUnloadedQuantityRequired);

                viewModel.CatchQuantity.AssignFrom(f.CatchQuantity);
                viewModel.UnloadedQuantity.AssignFrom(f.UnloadedQuantity);
                viewModel.AllowedDeviation.AssignFrom(f.AllowedDeviation);
                viewModel.CatchType.Value = CatchTypes.Find(x => x.Id == f.CatchInspectionTypeId);
                viewModel.CatchArea.AssignFrom(f.CatchZoneId, CatchAreas);
                viewModel.FishType.AssignFrom(f.FishId, FishTypes);
                viewModel.AverageSize.AssignFrom(f.AverageSize);
                viewModel.FishSex.AssignFrom(f.FishSexId, FishSexTypes);
                viewModel.CatchCount.AssignFrom(f.CatchCount);
                viewModel.TurbotSizeGroup.AssignFrom(f.TurbotSizeGroupId, TurbotSizeGroups);
                viewModel.UndersizedFish.AssignFrom(f.Undersized);
                viewModel.LogBookId = f.LogBookId;
                if (f.OriginShip != null)
                {
                    viewModel.Ship = f.OriginShip;
                    viewModel.ShipText = $"{f.OriginShip.Name} ({f.OriginShip.CFR})";
                }

                return viewModel;
            }));
            Catches.FirstOrDefault()?.CatchInspections.SetSummary();
            Catches.Validation.Force();

            InspectionCatchMeasureDto catchMeasure = catchMeasures.FirstOrDefault();
            if (_fishingGears != null && catchMeasure != null)
            {
                //var gears = _fishingGears.AllFishingGears
                //        .FindAll(fg => fg.Dto.PermittedFishingGear?.Id != null && fg.Dto.PermittedFishingGear.Id.Value == catchMeasure.FishingGearId);

                var gears = _fishingGears.AllFishingGears
                        .FindAll(fg => fg.Dto.PermittedFishingGear?.PermitId != null && fg.Dto.PermittedFishingGear.PermitId.Value == catchMeasure.FishingGearPermitId);

                foreach (var gear in gears)
                {
                    gear.LogBookId = catchMeasure.LogBookId;
                    gear.PermitId = catchMeasure.FishingGearPermitId;
                }

                _fishingGears.FishingGears.Value.AddRange(gears);
            }
        }

        public async Task AddCatches(LogBookPageDto selectedPage)
        {
            if (_fishingGears != null)
            {
                _fishingGears.FishingGears.Value.RemoveRange(_fishingGears.FishingGears.Value.Where(x => x.LogBookId == null).ToList());
                _fishingGears.FishingGears.Value.RemoveRange(_fishingGears.FishingGears.Value.Where(x => x.LogBookId == selectedPage.LogBookId).ToList());
            }
            await TLLoadingHelper.ShowFullLoadingScreen();
            OnEdit(await InspectionsTransaction.GetCatchesFromLogBookPageNumber(selectedPage.LogBookId, selectedPage.PageNum));
            await TLLoadingHelper.HideFullLoadingScreen();
        }

        private void OnAddCatch()
        {
            Catches.Value.Add(new CatchInspectionViewModel(Inspection, this, IsUnloadedQuantityRequired));
        }

        private void OnRemoveCatch(CatchInspectionViewModel catchInspection)
        {
            catchInspection.Unsubscribe();
            Catches.Value.Remove(catchInspection);
            SetSummary();
        }

        public static implicit operator List<InspectionCatchMeasureDto>(CatchInspectionsViewModel viewModel)
        {
            return viewModel.Catches.Select(f =>
            {
                InspectionCatchMeasureDto dto = (InspectionCatchMeasureDto)f;

                if (viewModel.ShowUndersizedCheck)
                {
                    dto.CatchInspectionTypeId = f.UndersizedFish
                        ? viewModel.CatchTypes.Find(s => s.Code == nameof(CatchSizeCodesEnum.BMS))?.Id
                        : viewModel.CatchTypes.Find(s => s.Code == nameof(CatchSizeCodesEnum.LSC))?.Id;
                }

                return dto;
            }).ToList();
        }

        public void SetSummary()
        {
            Summary = Catches.Value.Count == 0 ?
                "" :
                string.Join("; ", Catches.Value.GroupBy(
                    c => c.FishType.Value,
                    c => c,
                    (fishType, catches) =>
                    {
                        if (fishType == null)
                        {
                            return "";
                        }
                        IEnumerable<ValidState> quantity = null;

                        if (IsUnloadedQuantityRequired)
                        {

                            quantity = catches.Select(ci => ci.UnloadedQuantity)
                                .Where(ci => !string.IsNullOrEmpty(ci.Value))
                                .Where(ci => double.TryParse(ci.Value, out double tmp));
                        }
                        else
                        {
                            quantity = catches.Select(ci => ci.CatchQuantity)
                                .Where(ci => !string.IsNullOrEmpty(ci.Value))
                                .Where(ci => double.TryParse(ci.Value, out double tmp));
                        }

                        if (quantity.Count() == 0)
                        {
                            return "";
                        }
                        return $"{fishType.DisplayValue}: {quantity.Sum(x => double.Parse(x.Value)):f2} кг";
                    }
                ).Where(c => !string.IsNullOrEmpty(c)));
        }
    }
}
