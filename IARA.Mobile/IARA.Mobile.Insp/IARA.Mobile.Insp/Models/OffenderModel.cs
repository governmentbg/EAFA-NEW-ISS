using IARA.Mobile.Application.DTObjects.Common;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using TechnoLogica.Xamarin.ViewModels.Base.Models;

namespace IARA.Mobile.Insp.Models
{
    public class OffenderModel : BaseModel
    {
        public EgnLncDto EgnLnc { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public SelectNomenclatureDto Country { get; set; }
    }
}
