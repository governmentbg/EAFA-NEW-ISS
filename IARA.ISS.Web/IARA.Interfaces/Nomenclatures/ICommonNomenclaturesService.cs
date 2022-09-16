using System.Collections.Generic;
using IARA.DomainModels.DTOModels.Address;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.ControlActivity.Inspections;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.Nomenclatures;

namespace IARA.Interfaces.Nomenclatures
{
    public interface ICommonNomenclaturesService
    {
        List<MunicipalityNomenclatureExtendedDTO> GetMuncipalities();

        List<NomenclatureDTO> GetCountries();

        List<NomenclatureDTO> GetDistricts();

        List<PopulatedAreaNomenclatureExtendedDTO> GetPopulatedAreas();

        List<FishNomenclatureDTO> GetFishes();

        List<NomenclatureDTO> GetTerritoryUnits();

        List<NomenclatureDTO> GetSectors();

        List<NomenclatureDTO> GetDepartments();

        List<NomenclatureDTO> GetDocumentTypes();

        AddressNomenclaturesDTO GetAddressNomenclatures();

        List<NomenclatureDTO> GetFileTypes();

        List<NomenclatureDTO> GetPermissions();

        List<NomenclatureDTO> GetUserNames();

        List<NomenclatureDTO> GetOfflinePaymentTypes();

        List<NomenclatureDTO> GetOnlinePaymentTypes();

        List<NomenclatureDTO> GetGenders();

        List<SubmittedByRoleNomenclatureDTO> GetSubmittedByRoles();

        List<CancellationReasonDTO> GetCancellationReasons();

        List<PermittedFileTypeDTO> GetPermittedFileTypes();

        List<ApplicationDeliveryTypeDTO> GetDeliveryTypes();

        List<ChangeOfCircumstancesTypeDTO> GetChangeOfCircumstancesTypes();

        List<FishingGearNomenclatureDTO> GetFishingGear();

        List<NomenclatureDTO> GetFishingGearMarkStatuses();

        List<NomenclatureDTO> GetFishingGearPingerStatuses();

        List<NomenclatureDTO> GetInstitutions();

        List<NomenclatureDTO> GetVesselTypes();

        List<PatrolVehicleTypeNomenclatureDTO> GetPatrolVehicleTypes();

        List<NomenclatureDTO> GetCatchCheckTypes();

        List<NomenclatureDTO> GetPorts();

        List<InspectionObservationToolNomenclatureDTO> GetObservationTools();

        List<InspectionVesselActivityNomenclatureDTO> GetVesselActivities();

        List<CatchZoneNomenclatureDTO> GetCatchZones();

        List<NomenclatureDTO> GetUsageDocumentTypes();

        IEnumerable<ShipNomenclatureDTO> GetShips();

        List<NomenclatureDTO> GetLogBookTypes();

        List<NomenclatureDTO> GetLogBookStatuses();

        List<NomenclatureDTO> GetInspectionTypes();

        List<TariffNomenclatureDTO> GetPaymentTariffs();

        List<NomenclatureDTO> GetShipAssociations();

        List<NomenclatureDTO> GetCatchInspectionTypes();

        List<NomenclatureDTO> GetTransportVehicleTypes();

        List<NomenclatureDTO> GetFishSex();

        List<NomenclatureDTO> GetWaterBodyTypes();

        List<NomenclatureDTO> GetCatchPresentations();

        List<NomenclatureDTO> GetPoundNets();

        List<NomenclatureDTO> GetMarkReasons();

        List<NomenclatureDTO> GetRemarkReasons();
    }
}
