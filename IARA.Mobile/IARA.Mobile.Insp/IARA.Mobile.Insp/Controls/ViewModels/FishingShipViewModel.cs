using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.Interfaces.Dtos;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Insp.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class FishingShipViewModel : ViewModel
    {
        public FishingShipViewModel(InspectionPageViewModel inspection, bool canPickLocation = true, bool hasLastPort = false)
        {
            Inspection = inspection;
            HasLastPort = hasLastPort;

            ShipData = new InspectedShipDataViewModel(inspection, canPickLocation);
            ShipOwner = new InspectedPersonViewModel(inspection, InspectedPersonType.OwnerPers, InspectedPersonType.OwnerLegal);
            ShipUser = new InspectedPersonViewModel(inspection, InspectedPersonType.LicUsrPers, InspectedPersonType.LicUsrLgl);
            ShipRepresentative = new InspectedPersonViewModel(inspection, InspectedPersonType.ReprsPers)
            {
                IsRepresenter = true
            };
            ShipCaptain = new InspectedPersonViewModel(inspection, InspectedPersonType.CaptFshmn);

            IValidatableViewModel[] viewModels;

            if (hasLastPort)
            {
                LastHarbour = new InspectionHarbourViewModel(inspection);

                viewModels = new IValidatableViewModel[]
                {
                    ShipData,
                    ShipOwner,
                    ShipUser,
                    ShipRepresentative,
                    ShipCaptain,
                    LastHarbour,
                };
            }
            else
            {
                viewModels = new IValidatableViewModel[]
                {
                    ShipData,
                    ShipOwner,
                    ShipUser,
                    ShipRepresentative,
                    ShipCaptain,
                };
            }

            this.AddValidation(others: viewModels);

            ObservationsOrViolations.Category = InspectionObservationCategory.ShipData;
        }

        public InspectionPageViewModel Inspection { get; }

        public bool HasLastPort { get; }

        public InspectedShipDataViewModel ShipData { get; }
        public InspectedPersonViewModel ShipOwner { get; }
        public InspectedPersonViewModel ShipUser { get; }
        public InspectedPersonViewModel ShipRepresentative { get; }
        public InspectedPersonViewModel ShipCaptain { get; }
        public InspectionHarbourViewModel LastHarbour { get; }

        public ValidStateValidatableTable<ToggleViewModel> Toggles { get; set; }

        [MaxLength(4000)]
        public ValidStateObservation ObservationsOrViolations { get; set; }

        public void Init(List<SelectNomenclatureDto> nationalities, List<SelectNomenclatureDto> shipTypes, List<CatchZoneNomenclatureDto> quadrants)
        {
            ShipData.Init(nationalities, shipTypes, quadrants);
            ShipOwner.Init(nationalities);
            ShipUser.Init(nationalities);
            ShipRepresentative.Init(nationalities);
            ShipCaptain.Init(nationalities);
            LastHarbour?.Init(nationalities);
        }

        public void OnEdit(IFishingShipInspection fishingShipInspection, PortVisitDto port = null)
        {
            ShipData.OnEdit(fishingShipInspection.InspectedShip);

            if (Inspection.ActivityType != ViewActivityType.Review && ShipData.Ship.Value != null)
            {
                ShipData.ShipSelected.Execute(ShipData.Ship.Value);
            }

            Toggles.AssignFrom(fishingShipInspection.Checks);

            if (fishingShipInspection.Personnel == null)
            {
                return;
            }

            foreach (InspectionSubjectPersonnelDto person in fishingShipInspection.Personnel)
            {
                switch (person.Type)
                {
                    case InspectedPersonType.OwnerPers:
                    case InspectedPersonType.OwnerLegal:
                        ShipOwner.OnEdit(person);
                        break;
                    case InspectedPersonType.LicUsrPers:
                    case InspectedPersonType.LicUsrLgl:
                        ShipUser.OnEdit(person);
                        break;
                    case InspectedPersonType.ReprsPers:
                        ShipRepresentative.OnEdit(person);
                        break;
                    case InspectedPersonType.CaptFshmn:
                        ShipCaptain.OnEdit(person);
                        break;
                }
            }

            LastHarbour?.OnEdit(port);
        }

        public static implicit operator List<InspectionSubjectPersonnelDto>(FishingShipViewModel viewModel)
        {
            return new InspectionSubjectPersonnelDto[]
            {
                viewModel.ShipOwner,
                viewModel.ShipRepresentative,
                viewModel.ShipUser,
                viewModel.ShipCaptain
            }.Where(f => f != null).ToList();
        }
    }
}
