using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Attributes;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Helpers;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Models
{
    public class DeclarationCatchModel : ViewModel
    {
        private string _presentation;
        private string _catchZone;
        private string _catchType;
        private string _type;

        public DeclarationCatchModel()
        {
            this.AddValidation();

            Validity.IsValid = true;

            (Validity.Validations[0].Validation as DeclarationCatchValidAttribute).AssignParent(this);

            Validity.AddFakeValidation();
        }

        public string Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        public string CatchType
        {
            get => _catchType;
            set => SetProperty(ref _catchType, value);
        }

        public string Presentation
        {
            get => _presentation;
            set => SetProperty(ref _presentation, value);
        }

        public string CatchZone
        {
            get => _catchZone;
            set => SetProperty(ref _catchZone, value);
        }

        public InspectedDeclarationCatchDto Dto { get; set; }

        [DeclarationCatchValid]
        public ValidStateBool Validity { get; set; }

        public void AllChanged()
        {
            OnPropertyChanged(null);
            (Validity as IValidState).ForceValidation();
        }
    }
}
