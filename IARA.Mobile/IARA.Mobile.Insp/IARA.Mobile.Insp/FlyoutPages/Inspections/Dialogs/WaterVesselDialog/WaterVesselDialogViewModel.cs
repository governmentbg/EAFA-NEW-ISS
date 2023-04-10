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
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Base;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.WaterVesselDialog
{
    public class WaterVesselDialogViewModel : TLBaseDialogViewModel<WaterInspectionVesselDto>
    {
        private List<SelectNomenclatureDto> _vesselTypes;
        private bool _isTaken;
        private bool _isStored;

        public WaterVesselDialogViewModel()
        {
            Save = CommandBuilder.CreateFrom(OnSave);

            this.AddValidation();
        }

        public InspectionPageViewModel Inspection { get; set; }

        public WaterVesselsViewModel Vessels { get; set; }

        public WaterInspectionVesselDto Edit { get; set; }

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

        public ValidStateSelect<SelectNomenclatureDto> Type { get; set; }

        [MaxLength(50)]
        public ValidState Number { get; set; }

        [MaxLength(50)]
        public ValidState Color { get; set; }

        [TLRange(1, 10000, true)]
        public ValidState Length { get; set; }

        [TLRange(1, 10000, true)]
        public ValidState Width { get; set; }

        [MaxLength(50)]
        public ValidState TotalCount { get; set; }

        [MaxLength(500)]
        public ValidState Location { get; set; }

        public List<SelectNomenclatureDto> VesselTypes
        {
            get => _vesselTypes;
            set => SetProperty(ref _vesselTypes, value);
        }

        public ICommand Save { get; }

        public override Task Initialize(object sender)
        {
            INomenclatureTransaction nomTransaction = DependencyService.Resolve<INomenclatureTransaction>();

            VesselTypes = nomTransaction.GetVesselTypes();

            if (Edit != null)
            {
                Id = Edit.Id;
                IsStored = Edit.IsStored;
                IsTaken = Edit.IsTaken;
                Type.AssignFrom(Edit.VesselTypeId, VesselTypes);
                Number.AssignFrom(Edit.Number);
                Color.AssignFrom(Edit.Color);
                Length.AssignFrom(Edit.Length);
                Width.AssignFrom(Edit.Width);
                TotalCount.AssignFrom(Edit.TotalCount);
                Location.AssignFrom(Edit.StorageLocation);

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

            return HideDialog(new WaterInspectionVesselDto
            {
                Id = Id,
                VesselTypeId = Type.Value,
                IsTaken = IsTaken,
                IsStored = IsStored,
                Number = Number,
                Color = Color,
                Length = ParseHelper.ParseDecimal(Length),
                Width = ParseHelper.ParseDecimal(Width),
                StorageLocation = Location,
                TotalCount = ParseHelper.ParseInteger(TotalCount),
            });
        }
    }
}
