using System.ComponentModel.DataAnnotations;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Helpers;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Models
{
    public class PermitModel : ViewModel
    {
        public PermitModel()
        {
            this.AddValidation();

            Number.AddFakeValidation();
        }

        public bool AddedByInspector { get; set; }

        public InspectionPermitDto Dto { get; set; }

        [Required]
        [MaxLength(20)]
        public ValidState Number { get; set; }

        [Required]
        public ValidStateMultiToggle Corresponds { get; set; }

        [MaxLength(200)]
        public ValidState Description { get; set; }
    }
}
