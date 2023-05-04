using System.Collections.Generic;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Domain.Enums;

namespace IARA.Mobile.Insp.Application.Interfaces.Transactions
{
    public interface INomenclatureTransaction
    {
        List<SelectNomenclatureDto> GetCountries();

        List<SelectNomenclatureDto> GetDistricts();

        List<SelectNomenclatureDto> GetDocumentTypes();

        List<SelectNomenclatureDto> GetFileTypes(string pageCode);

        List<SelectNomenclatureDto> GetFileTypes();

        SelectNomenclatureDto GetFileType(string code);

        List<SelectNomenclatureDto> GetInstitutions();

        List<SelectNomenclatureDto> GetMuncipalities(int districtId);

        List<ObservationToolNomenclatureDto> GetObservationTools();

        List<SelectNomenclatureDto> GetPatrolVehicleTypes(bool? isWaterVehicle);

        List<SelectNomenclatureDto> GetPopulatedAreas(int municipalityId);

        List<VesselActivityDto> GetVesselActivities();

        List<SelectNomenclatureDto> GetVesselTypes();

        List<InspectorNomenclatureDto> GetInspectors(int page, int count, string search = null);

        List<SelectNomenclatureDto> GetBuyers(int page, int count, string search = null);

        List<ShipSelectNomenclatureDto> GetShips(int page, int count, string search = null);

        ShipSelectNomenclatureDto GetShipNomenclature(int id);

        ShipDto GetShip(int id);

        List<InspectionCheckTypeDto> GetInspectionCheckTypes(InspectionType inspectionType);

        List<SelectNomenclatureDto> GetCatchInspectionTypes();

        List<SelectNomenclatureDto> GetFishes();

        List<SelectNomenclatureDto> GetFishingGears();

        List<SelectNomenclatureDto> GetPatrolVehicles(bool? isWaterVehicle, int page, int count, string search = null);

        VesselDuringInspectionDto GetPatrolVehicle(int id);

        List<ShipPersonnelDto> GetShipPersonnel(int shipUid);

        List<ShipPersonnelDto> GetPoundNetOwners(int poundNetId);

        ShipPersonnelDetailedDto GetDetailedShipPerson(int entryId, InspectedPersonType type);

        ShipPersonnelDetailedDto GetDetailedPoundNetPerson(int entryId, InspectedPersonType type);

        List<CatchZoneNomenclatureDto> GetCatchZones();

        List<SelectNomenclatureDto> GetWaterBodyTypes();

        List<SelectNomenclatureDto> GetPorts(int page, int count, string search = null);

        List<SelectNomenclatureDto> GetFishingGearMarkStatuses();

        List<SelectNomenclatureDto> GetFishingGearPingerStatuses();

        List<SelectNomenclatureDto> GetPoundNets(int page, int count, string search = null);

        SelectNomenclatureDto GetPoundNet(int id);

        List<PermitNomenclatureDto> GetPoundNetPermits(int poundNetId, int page, int count, string search = null);

        List<PermitNomenclatureDto> GetPermits(int shipUid, int page, int count, string search = null);

        List<SelectNomenclatureDto> GetTransportVehicleTypes();

        List<SelectNomenclatureDto> GetGenders();

        List<PermitLicenseDto> GetPermitLicenses(int shipUid);

        List<PermitDto> GetPermits(int shipUid);

        List<LogBookDto> GetLogBooks(int shipUid);

        List<SelectNomenclatureDto> GetAssociations();

        SelectNomenclatureDto GetAssociation(int id);

        InspectionSubjectPersonnelDto GetBuyer(int id);

        List<SelectNomenclatureDto> GetFleetTypes();

        List<SelectNomenclatureDto> GetFishPresentations();

        BuyerUtilityDto GetBuyerUtility(int id);

        List<SelectNomenclatureDto> GetAquacultures(int page, int count, string search = null);

        List<SelectNomenclatureDto> GetFishSex();

        SelectNomenclatureDto GetAquaculture(int id);

        PermitNomenclatureDto GetPoundNetPermit(int id);

        PermitNomenclatureDto GetPermit(int id);

        List<SelectNomenclatureDto> GetFishingGearRecheckReasons();

        List<SelectNomenclatureDto> GetFishingGearCheckReasons();

        ShipPersonnelDetailedDto GetAquacultureOwner(int aquacultureId);

        List<SelectNomenclatureDto> GetTurbotSizeGroups();

        List<SelectNomenclatureDto> GetInspectionVesselTypes();
    }
}
