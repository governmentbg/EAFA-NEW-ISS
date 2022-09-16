using System.Collections.Generic;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Address;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.ControlActivity.Inspections;
using IARA.DomainModels.DTOModels.FishingTickets;
using IARA.DomainModels.DTOModels.Mobile.Nomenclatures;
using IARA.DomainModels.Nomenclatures;

namespace IARA.Interfaces.Nomenclatures
{
    public interface IMobileNomenclaturesService
    {
        List<MobileNomenclatureDTO> GetNomenclatureTables(MobileTypeEnum mobileType);

        List<NomenclatureDTO> GetCountries();

        List<NomenclatureDTO> GetDistricts();

        List<NomenclatureDTO> GetDocumentTypes();

        List<NomenclatureDTO> GetFishes();

        List<NomenclatureDTO> GetPermitReasons();

        List<NomenclatureDTO> GetFileTypes();

        List<NomenclatureDTO> GetViolationSignalTypes();

        List<MunicipalityNomenclatureExtendedDTO> GetMunicipalities();

        List<InspectionObservationToolNomenclatureDTO> GetObservationTools();

        List<PopulatedAreaNomenclatureExtendedDTO> GetPopulatedAreas();

        List<TicketTypeNomenclatureDTO> GetTicketTypes();

        List<NomenclatureDTO> GetTicketPeriods();

        List<TicketPeriodPriceDTO> GetTicketTariffs();

        List<MobileVersionNomenclatureDTO> GetMobileVersions();

        List<NomenclatureDTO> GetGenders();

        List<SystemParameterNomenclatureDTO> GetSystemParameters();

        List<NomenclatureDTO> GetInspectionStates();

        List<NomenclatureDTO> GetInspectionTypes();

        List<NomenclatureDTO> GetInstitutions();

        List<PatrolVehicleTypeNomenclatureDTO> GetPatrolVehicleTypes();

        List<InspectionVesselActivityNomenclatureDTO> GetVesselActivities();

        List<NomenclatureDTO> GetVesselTypes();

        List<RequiredFileTypeNomenclatureDTO> GetRequiredFileTypes();

        List<NomenclatureDTO> GetShipAssociations();

        List<InspectionCheckTypeNomenclatureDTO> GetInspectionCheckTypes();

        List<NomenclatureDTO> GetCatchInspectionTypes();

        List<NomenclatureDTO> GetFishingGears();

        List<NomenclatureDTO> GetPaymentTypes();

        List<NomenclatureDTO> GetInspectedPersonTypes();

        List<NomenclatureCatchZoneDTO> GetCatchZones();

        List<NomenclatureDTO> GetWaterBodyTypes();

        List<NomenclatureDTO> GetPorts();

        List<NomenclatureDTO> GetFishingGearMarkStatuses();

        List<NomenclatureDTO> GetFishingGearPingerStatuses();

        List<NomenclatureDTO> GetTransportVehicleTypes();

        List<NomenclatureDTO> GetPermitLicenseTypes();

        List<NomenclatureDTO> GetFleetTypes();

        List<NomenclatureDTO> GetFishPresentations();

        List<NomenclatureDTO> GetFishSex();

        List<NomenclatureDTO> GetFishingGearCheckReasons();

        List<NomenclatureDTO> GetFishingGearRecheckReasons();
    }
}
