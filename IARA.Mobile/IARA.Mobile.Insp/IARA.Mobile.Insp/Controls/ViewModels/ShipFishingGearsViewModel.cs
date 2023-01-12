using System.ComponentModel.DataAnnotations;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Models;
using TechnoLogica.Xamarin.Helpers;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class ShipFishingGearsViewModel : ViewModel
    {
        public ShipFishingGearsViewModel(InspectionPageViewModel inspection)
        {
            Inspection = inspection;

            FishingGears = new FishingGearsViewModel(inspection);

            this.AddValidation(others: new[] { FishingGears });

            ObservationsOrViolations.Category = InspectionObservationCategory.FishingGear;
        }

        public InspectionPageViewModel Inspection { get; }

        public FishingGearsViewModel FishingGears { get; }

        [MaxLength(4000)]
        public ValidStateObservation ObservationsOrViolations { get; set; }
    }
}
