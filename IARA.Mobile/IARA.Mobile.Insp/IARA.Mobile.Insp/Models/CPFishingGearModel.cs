using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using TechnoLogica.Xamarin.ViewModels.Base.Models;

namespace IARA.Mobile.Insp.Models
{
    public class CPFishingGearModel : BaseModel
    {
        public InspectedCPFishingGearDto Dto { get; set; }
        public SelectNomenclatureDto FishingGear { get; set; }
    }
}
