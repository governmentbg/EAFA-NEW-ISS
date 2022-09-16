using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Attributes;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Helpers;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Models
{
    public class WaterVesselModel : ViewModel
    {
        public WaterVesselModel()
        {
            this.AddValidation();

            Validity.IsValid = true;

            (Validity.Validations[0].Validation as WaterVesselValidAttribute).AssignParent(this);

            Validity.AddFakeValidation();
        }

        public WaterInspectionVesselDto Dto { get; set; }

        [WaterVesselValid]
        public ValidStateBool Validity { get; set; }

        public void AllChanged()
        {
            OnPropertyChanged(null);
            (Validity as IValidState).ForceValidation();
        }
    }
}
