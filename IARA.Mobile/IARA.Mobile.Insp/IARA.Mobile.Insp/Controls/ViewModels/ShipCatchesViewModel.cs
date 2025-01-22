using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.Interfaces.Dtos;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Insp.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.Forms.Internals;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class ShipCatchesViewModel : ViewModel
    {
        public ShipCatchesViewModel(InspectionPageViewModel inspection, FishingGearsViewModel fishingGears, bool isUnloadedQuantityRequired = false)
        {
            Inspection = inspection;

            Catches = new CatchInspectionsViewModel(inspection, fishingGears, isUnloadedQuantityRequired: isUnloadedQuantityRequired);

            this.AddValidation(others: new[] { Catches });

            ObservationsOrViolations.Category = InspectionObservationCategory.Catch;
        }

        public InspectionPageViewModel Inspection { get; }

        public CatchInspectionsViewModel Catches { get; }

        public ValidStateValidatableTable<ToggleViewModel> Toggles { get; set; }

        [MaxLength(4000)]
        public ValidStateObservation ObservationsOrViolations { get; set; }

        public void Init(List<SelectNomenclatureDto> fishTypes, List<SelectNomenclatureDto> catchTypes, List<CatchZoneNomenclatureDto> catchAreas, List<SelectNomenclatureDto> turbotSizeGroups)
        {
            Catches.Init(fishTypes, catchTypes, catchAreas, turbotSizeGroups, null);
        }

        public void OnEdit(IFishingShipInspection fishingShipInspection)
        {
            Catches.OnEdit(fishingShipInspection.CatchMeasures);

            Toggles.AssignFrom(fishingShipInspection.Checks);
        }

        public void Reset()
        {
            Catches.Reset();
            Toggles.ForEach(x => x.Reset());
            ObservationsOrViolations.Value = "";
        }
    }
}
