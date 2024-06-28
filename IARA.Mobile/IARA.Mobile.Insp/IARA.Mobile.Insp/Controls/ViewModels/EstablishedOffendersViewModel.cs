using IARA.Mobile.Insp.Base;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class EstablishedOffendersViewModel : ViewModel
    {
        public EstablishedOffendersViewModel(InspectionPageViewModel inspection)
        {
            Inspection = inspection;
        }

        public InspectionPageViewModel Inspection { get; set; }
        //public ValidStateValidatableTable<OffenderModel> Offenders { get; set; }
    }
}
