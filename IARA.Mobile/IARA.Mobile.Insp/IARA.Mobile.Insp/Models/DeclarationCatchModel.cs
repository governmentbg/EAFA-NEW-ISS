using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using System;
using TechnoLogica.Xamarin.ViewModels.Base.Models;

namespace IARA.Mobile.Insp.Models
{
    public class DeclarationCatchModel : BaseModel
    {
        public string DocumentType { get; set; }
        public string PageNumber { get; set; }
        public DateTime? PageDate { get; set; }
        public string Information { get; set; }
        public InspectionLogBookPageDto InspectionLogBookPage { get; set; }
    }
}
