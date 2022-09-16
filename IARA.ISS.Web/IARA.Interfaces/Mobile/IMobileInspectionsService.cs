using System;
using System.Collections.Generic;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.ControlActivity.Inspections;
using IARA.DomainModels.DTOModels.Mobile.Inspections;
using IARA.DomainModels.DTOModels.Mobile.Ships;
using IARA.DomainModels.Nomenclatures;

namespace IARA.Interfaces.Mobile
{
    public interface IMobileInspectionsService
    {
        bool IsDeviceAllowed(string imei);
        List<InspectorDTO> GetInspectors(DateTime? afterDate);
        List<NomenclatureDTO> GetPoundNets(DateTime? afterDate);
        List<PoundNetPermitLicenseDTO> GetPoundNetPermitLicenses(DateTime? afterDate);
        List<FishingGearInspectionDTO> GetPoundNetFishingGears(DateTime? afterDate);
        List<FishingGearMarkInspectionDTO> GetPoundNetFishingGearsMarks(DateTime? afterDate);
        List<FishingGearPingerInspectionDTO> GetPoundNetFishingGearsPingers(DateTime? afterDate);
        List<PatrolVehicleNomenclatureDTO> GetPatrolVehicles(DateTime? afterDate);
        List<ShipMobileDTO> GetShips(DateTime? afterDate);
        List<PermitLicenseMobileDTO> GetPermitLicenses(DateTime? afterDate);
        List<ShipOwnerMobileDTO> GetShipsOwners(DateTime? afterDate);
        List<PersonMobileDTO> GetPersons(List<int> personIds);
        List<LegalMobileDTO> GetLegals(List<int> legalIds);
        List<FishingGearInspectionDTO> GetShipsFishingGears(DateTime? afterDate);
        List<FishingGearMarkInspectionDTO> GetFishingGearsMarks(DateTime? afterDate);
        List<FishingGearPingerInspectionDTO> GetFishingGearsPingers(DateTime? afterDate);
        List<LogBookMobileDTO> GetLogBooks(DateTime? afterDate);
        List<LogBookPageMobileDTO> GetLogBookPages(List<int> logBookIds);
        List<BuyerMobileDTO> GetBuyers(DateTime? afterDate);
        List<DeclarationLogBookPageDTO> GetDeclarationLogBookPages(DeclarationLogBookTypeEnum type, int shipUid);
        List<AquacultureMobileDTO> GetAquacultures(DateTime? afterDate);
    }
}
