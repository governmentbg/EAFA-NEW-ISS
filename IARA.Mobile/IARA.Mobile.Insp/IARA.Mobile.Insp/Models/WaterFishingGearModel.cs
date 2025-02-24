using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using TechnoLogica.Xamarin.ViewModels.Base.Models;

namespace IARA.Mobile.Insp.Models
{
    public class WaterFishingGearModel : BaseAssignableModel<WaterFishingGearModel>
    {
        public SelectNomenclatureDto Type { get; set; }
        public string Marks { get; set; }

        public WaterInspectionFishingGearDto Dto { get; set; }
    }
}
