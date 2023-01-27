using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using TechnoLogica.Xamarin.ViewModels.Base.Models;

namespace IARA.Mobile.Insp.Models
{
    public class InspectorModel : BaseModel
    {
        private bool _isInCharge;
        private bool _hasIdentified;

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
    }
}
