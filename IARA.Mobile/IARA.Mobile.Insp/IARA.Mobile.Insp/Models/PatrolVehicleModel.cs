using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using TechnoLogica.Xamarin.ViewModels.Base.Models;

namespace IARA.Mobile.Insp.Models
{
    public class PatrolVehicleModel : BaseAssignableModel<PatrolVehicleModel>
    {
        private string _institution;
        private string _patrolVehicleType;

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
    }
}
