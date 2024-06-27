using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Controls.ViewModels;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Insp.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Base;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.WaterFishingGearDialog
{
    public class WaterFishingGearDialogViewModel : TLBaseDialogViewModel<WaterFishingGearModel>
    {
        private List<SelectNomenclatureDto> _markStatuses;
        private bool _isTaken;
        private bool _isStored;

        public WaterFishingGearDialogViewModel()
        {
            FishingGearGeneralInfo = new FishingGearGeneralInfoViewModel(ViewActivityType.Add, true, false, true);

            Save = CommandBuilder.CreateFrom(OnSave);


            this.AddValidation(others: new IValidatableViewModel[]
            {
                FishingGearGeneralInfo
            });
        }

        public InspectionPageViewModel Inspection { get; set; }

        public WaterFishingGearsViewModel WaterFishingGears { get; set; }
        public FishingGearGeneralInfoViewModel FishingGearGeneralInfo { get; set; }

        public WaterFishingGearModel Edit { get; set; }

        public ViewActivityType DialogType { get; set; }

        public int? Id { get; set; }

        public bool IsTaken
        {
            get => _isTaken;
            set => SetProperty(ref _isTaken, value);
        }
        public bool IsStored
        {
            get => _isStored;
            set => SetProperty(ref _isStored, value);
        }

        [MaxLength(500)]
        public ValidState Location { get; set; }
        public List<SelectNomenclatureDto> MarkStatuses
        {
            get => _markStatuses;
            private set => SetProperty(ref _markStatuses, value);
        }

        public ICommand Save { get; }

        public override Task Initialize(object sender)
        {
            INomenclatureTransaction nomTransaction = DependencyService.Resolve<INomenclatureTransaction>();
            List<FishingGearSelectNomenclatureDto> fishingGearTypes = nomTransaction.GetFishingGears().Select(x => new FishingGearSelectNomenclatureDto()
            {
                Name = x.DisplayValue,
                Code = x.Code,
                Id = x.Id,
                HasHooks = x.HasHooks,
            }).OrderBy(x => x.Code).ToList();
            FishingGearSelectNomenclatureDto Poundnet = fishingGearTypes.FirstOrDefault(x => x.Code == "DLN");
            int index = fishingGearTypes.IndexOf(Poundnet);
            fishingGearTypes.RemoveAt(index);

            FishingGearGeneralInfo.Init(fishingGearTypes);
            FishingGearGeneralInfo.DialogType = DialogType;

            MarkStatuses = nomTransaction.GetFishingGearMarkStatuses();

            if (Edit != null)
            {
                Id = Edit.Dto.Id;
                IsTaken = Edit.Dto.IsTaken.Value;
                IsStored = Edit.Dto.IsStored.Value;
                Location.AssignFrom(Edit.Dto.StorageLocation);
                FishingGearGeneralInfo.AssignEdit(Edit.Dto);

                if (DialogType == ViewActivityType.Edit)
                {
                    Validation.Force();
                }
            }

            return Task.CompletedTask;
        }

        private Task OnSave()
        {
            Validation.Force();

            if (!Validation.IsValid)
            {
                return Task.CompletedTask;
            }

            return HideDialog(new WaterFishingGearModel
            {
                Type = FishingGearGeneralInfo.FishingGearType.Value,
                Dto = new WaterInspectionFishingGearDto
                {
                    Id = Id,
                    Count = ParseHelper.ParseInteger(FishingGearGeneralInfo.Count) ?? 0,
                    Description = FishingGearGeneralInfo.Description,
                    Height = ParseHelper.ParseDecimal(FishingGearGeneralInfo.Height),
                    HookCount = ParseHelper.ParseInteger(FishingGearGeneralInfo.HookCount),
                    NetEyeSize = ParseHelper.ParseDecimal(FishingGearGeneralInfo.NetEyeSize),
                    Length = ParseHelper.ParseDecimal(FishingGearGeneralInfo.Length),
                    CordThickness = ParseHelper.ParseDecimal(FishingGearGeneralInfo.CordThickness),
                    TowelLength = ParseHelper.ParseInteger(FishingGearGeneralInfo.TowelLength),
                    HouseLength = ParseHelper.ParseInteger(FishingGearGeneralInfo.HouseLength),
                    HouseWidth = ParseHelper.ParseInteger(FishingGearGeneralInfo.HouseWidth),
                    TypeId = FishingGearGeneralInfo.FishingGearType.Value,
                    IsTaken = IsTaken,
                    IsStored = IsStored,
                    StorageLocation = Location,
                }
            });
        }
    }
}
