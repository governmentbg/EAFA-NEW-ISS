using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Base;
using System.ComponentModel.DataAnnotations;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Models
{
    public class PermitLicenseModel : ViewModel
    {
        public PermitLicenseModel()
        {
            this.AddValidation();
        }

        public bool AddedByInspector { get; set; }

        public InspectionPermitDto Dto { get; set; }

        [Required]
        [MaxLength(20)]
        public ValidState LicenseNumber { get; set; }

        [Required]
        public ValidStateMultiToggle Corresponds { get; set; }

        [MaxLength(200)]
        public ValidState Description { get; set; }
    }
}
