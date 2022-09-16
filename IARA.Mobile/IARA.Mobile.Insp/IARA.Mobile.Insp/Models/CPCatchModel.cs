using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using TechnoLogica.Xamarin.ViewModels.Base.Models;

namespace IARA.Mobile.Insp.Models
{
    public class CPCatchModel : BaseModel
    {
        public SelectNomenclatureDto Fish { get; set; }
        public InspectedCPCatchDto Dto { get; set; }
    }
}
