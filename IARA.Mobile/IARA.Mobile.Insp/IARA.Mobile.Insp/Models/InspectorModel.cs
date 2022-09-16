using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Attributes;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Helpers;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Models
{
    public class InspectorModel : ViewModel
    {
        private bool _isInCharge;
        private bool _hasIdentified;

        public InspectorModel()
        {
            this.AddValidation();

            Validity.IsValid = true;

            (Validity.Validations[0].Validation as InspectorValidAttribute).AssignParent(this);

            Validity.AddFakeValidation();
        }

        public bool IsCurrentInspector { get; set; }

        public bool IsInCharge
        {
            get => _isInCharge;
            set => SetProperty(ref _isInCharge, value);
        }
        public bool HasIdentified
        {
            get => _hasIdentified;
            set => SetProperty(ref _hasIdentified, value);
        }

        public string Institution { get; set; }

        public InspectorDuringInspectionDto Dto { get; set; }

        [InspectorValid]
        public ValidStateBool Validity { get; set; }

        public void AllChanged()
        {
            OnPropertyChanged(null);
            (Validity as IValidState).ForceValidation();
        }
    }
}
