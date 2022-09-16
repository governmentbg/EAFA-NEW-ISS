using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Attributes;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Models
{
    public class FishingGearModel : ViewModel
    {
        public FishingGearModel()
        {
            this.AddValidation();

            Validity.IsValid = true;

            (Validity.Validations[0].Validation as FishingGearValidAttribute).AssignParent(this);

            Validity.AddFakeValidation();
        }

        public bool IsAddedByInspector { get; set; }

        public SelectNomenclatureDto Type { get; set; }
        public string Marks { get; set; }
        public int Count { get; set; }
        public decimal? NetEyeSize { get; set; }
        public InspectedFishingGearEnum? CheckedValue { get; set; }

        public InspectedFishingGearDto Dto { get; set; }

        [FishingGearValid]
        public ValidStateBool Validity { get; set; }

        public void AllChanged()
        {
            OnPropertyChanged(null);
            (Validity as IValidState).ForceValidation();
        }
    }
}
