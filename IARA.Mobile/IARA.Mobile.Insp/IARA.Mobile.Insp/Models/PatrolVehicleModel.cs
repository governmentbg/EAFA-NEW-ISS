using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Attributes;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Helpers;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Models
{
    public class PatrolVehicleModel : ViewModel
    {
        private string _institution;
        private string _patrolVehicleType;

        public PatrolVehicleModel()
        {
            this.AddValidation();

            Validity.IsValid = true;

            (Validity.Validations[0].Validation as PatrolVehicleValidAttribute).AssignParent(this);

            Validity.AddFakeValidation();
        }

        public string Institution
        {
            get => _institution;
            set => SetProperty(ref _institution, value);
        }

        public string PatrolVehicleType
        {
            get => _patrolVehicleType;
            set => SetProperty(ref _patrolVehicleType, value);
        }

        public VesselDuringInspectionDto Dto { get; set; }

        [PatrolVehicleValid]
        public ValidStateBool Validity { get; set; }

        public void AllChanged()
        {
            OnPropertyChanged(null);
            (Validity as IValidState).ForceValidation();
        }
    }
}
