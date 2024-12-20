using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Domain.Enums;
using TechnoLogica.Xamarin.ViewModels.Base.Models;

namespace IARA.Mobile.Insp.Models
{
    public class FishingGearModel : BaseModel
    {
        public bool IsAddedByInspector { get; set; }

        public SelectNomenclatureDto Type { get; set; }
        public string Marks { get; set; }
        public int Count { get; set; }
        public decimal? NetEyeSize { get; set; }
        public InspectedFishingGearEnum? CheckedValue { get; set; }

        public InspectedFishingGearDto Dto { get; set; }

        public int? LogBookId { get; set; } = null;
        public int? PermitId { get; set; } = null;


        private bool showErrorMessage = false;
        public bool ShowErrorMessage
        {
            get => showErrorMessage;
            set => SetProperty(ref showErrorMessage, value);
        }
    }
}
