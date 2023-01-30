using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using TechnoLogica.Xamarin.ViewModels.Base.Models;

namespace IARA.Mobile.Insp.Models
{
    public class DeclarationCatchModel : BaseModel
    {
        private string _presentation;
        private string _catchZone;
        private string _catchType;
        private string _type;

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
    }
}
