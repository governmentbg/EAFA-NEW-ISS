using IARA.Mobile.Application.Attributes;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Controls.ViewModels;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Insp.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Base;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.EngineDialog
{
    public class EngineDialogViewModel : TLBaseDialogViewModel<EngineModel>
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

        public EngineModel Edit { get; set; }

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
        [MaxLength(50)]
        public ValidState Model { get; set; }

        [Required]
        [TLRange(1, 999.99)]
        public ValidState Power { get; set; }

        [Required]
        [MaxLength(50)]
        public ValidState Type { get; set; }

        [Required]
        [TLRange(1, 1000)]
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
                Id = Edit.Dto.Id;
                IsStored = Edit.Dto.IsStored;
                IsTaken = Edit.Dto.IsTaken;
                Model.AssignFrom(Edit.Dto.Model);
                Power.AssignFrom(Edit.Dto.Power);
                Type.AssignFrom(Edit.Dto.Type);
                TotalCount.AssignFrom(Edit.Dto.TotalCount);
                Location.AssignFrom(Edit.Dto.StorageLocation);
                EngineDescription.AssignFrom(Edit.Dto.EngineDescription);

                if (DialogType == ViewActivityType.Edit)
                {
                    Validation.Force();
                }
            }

            return Task.CompletedTask;
        }

        private Task OnSave()
        {
            return HideDialog(new EngineModel
            {
                Dto = new WaterInspectionEngineDto
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
                }
            });
        }
    }
}
