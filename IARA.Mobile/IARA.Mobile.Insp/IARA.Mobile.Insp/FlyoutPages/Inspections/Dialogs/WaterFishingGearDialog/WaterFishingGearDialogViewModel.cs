using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Application.Attributes;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Controls.ViewModels;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Insp.Models;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Base;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.WaterFishingGearDialog
{
    public class WaterFishingGearDialogViewModel : TLBaseDialogViewModel<WaterFishingGearModel>
    {
        private List<SelectNomenclatureDto> _markStatuses;
        private List<SelectNomenclatureDto> _fishingGearTypes;
        private bool _isTaken;
        private bool _isStored;

        public WaterFishingGearDialogViewModel()
        {
            Save = CommandBuilder.CreateFrom(OnSave);

            this.AddValidation();
        }

        public InspectionPageViewModel Inspection { get; set; }

        public WaterFishingGearsViewModel WaterFishingGears { get; set; }

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

        [Required]
        public ValidStateSelect<SelectNomenclatureDto> FishingGearType { get; set; }

        [Required]
        [TLRange(1, 10000)]
        public ValidState Count { get; set; }

        [TLRange(1, 10000, true)]
        public ValidState NetEyeSize { get; set; }

        [TLRange(0, 10000)]
        public ValidState HookCount { get; set; }

        [TLRange(1, 10000, true)]
        public ValidState Length { get; set; }

        [TLRange(1, 10000, true)]
        public ValidState Height { get; set; }

        [MaxLength(4000)]
        public ValidState Description { get; set; }

        [MaxLength(500)]
        public ValidState Location { get; set; }

        public List<SelectNomenclatureDto> FishingGearTypes
        {
            get => _fishingGearTypes;
            private set => SetProperty(ref _fishingGearTypes, value);
        }
        public List<SelectNomenclatureDto> MarkStatuses
        {
            get => _markStatuses;
            private set => SetProperty(ref _markStatuses, value);
        }

        public ICommand Save { get; }

        public override Task Initialize(object sender)
        {
            INomenclatureTransaction nomTransaction = DependencyService.Resolve<INomenclatureTransaction>();

            FishingGearTypes = nomTransaction.GetFishingGears();
            MarkStatuses = nomTransaction.GetFishingGearMarkStatuses();

            FishingGearType.Value = FishingGearTypes[0];

            if (Edit != null)
            {
                Id = Edit.Dto.Id;
                IsTaken = Edit.Dto.IsTaken;
                IsStored = Edit.Dto.IsStored;
                FishingGearType.AssignFrom(Edit.Dto.TypeId, FishingGearTypes);
                Count.AssignFrom(Edit.Dto.Count);
                NetEyeSize.AssignFrom(Edit.Dto.NetEyeSize);
                HookCount.AssignFrom(Edit.Dto.HookCount);
                Length.AssignFrom(Edit.Dto.Length);
                Height.AssignFrom(Edit.Dto.Height);
                Description.AssignFrom(Edit.Dto.Description);
                Location.AssignFrom(Edit.Dto.StorageLocation);

                if (FishingGearType.Value == null)
                {
                    FishingGearType.Value = FishingGearTypes[0];
                }

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
                Type = FishingGearType.Value,
                Dto = new WaterInspectionFishingGearDto
                {
                    Id = Id,
                    Count = ParseHelper.ParseInteger(Count) ?? 0,
                    Description = Description,
                    Height = ParseHelper.ParseDecimal(Height),
                    HookCount = ParseHelper.ParseInteger(HookCount),
                    NetEyeSize = ParseHelper.ParseDecimal(NetEyeSize),
                    TypeId = FishingGearType.Value,
                    Length = ParseHelper.ParseDecimal(Length),
                    IsTaken = IsTaken,
                    IsStored = IsStored,
                    StorageLocation = Location,
                }
            });
        }
    }
}
