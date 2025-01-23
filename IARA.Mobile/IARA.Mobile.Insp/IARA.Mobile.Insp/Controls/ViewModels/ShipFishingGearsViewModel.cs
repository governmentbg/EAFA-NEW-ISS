using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Models;
using System.ComponentModel.DataAnnotations;
using TechnoLogica.Xamarin.Helpers;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class ShipFishingGearsViewModel : ViewModel
    {
        public ShipFishingGearsViewModel(InspectionPageViewModel inspection, PermitLicensesViewModel permitLicenses, bool hasAttachmentForFishingGear = false)
        {
            Inspection = inspection;

            FishingGears = new FishingGearsViewModel(inspection, permitLicenses, hasAttachmentForFishingGear: hasAttachmentForFishingGear);

            this.AddValidation(others: new[] { FishingGears });

            ObservationsOrViolations.Category = InspectionObservationCategory.FishingGear;
        }

        public InspectionPageViewModel Inspection { get; }

        public FishingGearsViewModel FishingGears { get; }

        private bool hasErrors;
        public bool HasErrors
        {
            get { return hasErrors; }
            set { hasErrors = value; }
        }


        [MaxLength(4000)]
        public ValidStateObservation ObservationsOrViolations { get; set; }
    }
}
