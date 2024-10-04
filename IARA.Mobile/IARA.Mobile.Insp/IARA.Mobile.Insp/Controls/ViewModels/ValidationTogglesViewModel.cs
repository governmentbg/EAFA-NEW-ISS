using IARA.Mobile.Insp.Base;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class ValidationTogglesViewModel : ViewModel
    {
        public ValidationTogglesViewModel(InspectionPageViewModel inspection)
        {
            Inspection = inspection;
            this.AddValidation();
        }

        public InspectionPageViewModel Inspection { get; set; }

        public ValidStateValidatableTable<ToggleViewModel> Toggles { get; set; }
    }
}
