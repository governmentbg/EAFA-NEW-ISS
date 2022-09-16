using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Attributes;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Helpers;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Models
{
    public class EngineModel : ViewModel
    {
        public EngineModel()
        {
            this.AddValidation();

            Validity.IsValid = true;

            (Validity.Validations[0].Validation as EngineValidAttribute).AssignParent(this);

            Validity.AddFakeValidation();
        }

        public WaterInspectionEngineDto Dto { get; set; }

        [EngineValid]
        public ValidStateBool Validity { get; set; }

        public void AllChanged()
        {
            OnPropertyChanged(null);
            (Validity as IValidState).ForceValidation();
        }
    }
}
