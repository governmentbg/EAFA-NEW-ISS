using IARA.Mobile.Application.Attributes;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Controls.ViewModels;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Base;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.EngineDialog
{
    public class EngineDialogViewModel : TLBaseDialogViewModel<WaterInspectionEngineDto>
    {
        private bool _isTaken;
        private bool _isStored;

        public EngineDialogViewModel()
        {
            Save = CommandBuilder.CreateFrom(OnSave);

            this.AddValidation();
        }

        public InspectionPageViewModel Inspection { get; set; }

        public EnginesViewModel Engines { get; set; }

        public WaterInspectionEngineDto Edit { get; set; }

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

        [MaxLength(50)]
        public ValidState Model { get; set; }

        [TLRange(0, 999.99)]
        public ValidState Power { get; set; }

        [MaxLength(50)]
        public ValidState Type { get; set; }

        [TLRange(0, 1000)]
        public ValidState TotalCount { get; set; }

        [MaxLength(500)]
        public ValidState Location { get; set; }

        [MaxLength(500)]
        public ValidState EngineDescription { get; set; }

        public ICommand Save { get; }

        public override Task Initialize(object sender)
        {
            if (Edit != null)
            {
                Id = Edit.Id;
                IsStored = Edit.IsStored;
                IsTaken = Edit.IsTaken;
                Model.AssignFrom(Edit.Model);
                Power.AssignFrom(Edit.Power);
                Type.AssignFrom(Edit.Type);
                TotalCount.AssignFrom(Edit.TotalCount);
                Location.AssignFrom(Edit.StorageLocation);
                EngineDescription.AssignFrom(Edit.EngineDescription);

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

            if (Validation.IsValid)
            {
                return HideDialog(new WaterInspectionEngineDto
                {
                    Id = Id,
                    IsStored = IsStored,
                    IsTaken = IsTaken,
                    Model = Model,
                    Power = ParseHelper.ParseDecimal(Power),
                    StorageLocation = Location,
                    TotalCount = ParseHelper.ParseInteger(TotalCount),
                    Type = Type,
                    EngineDescription = EngineDescription,
                });
            }

            return Task.CompletedTask;
        }
    }
}
