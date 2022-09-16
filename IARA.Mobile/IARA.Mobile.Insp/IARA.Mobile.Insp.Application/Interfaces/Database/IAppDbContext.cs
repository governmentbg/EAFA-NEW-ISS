using IARA.Mobile.Application.Interfaces.Database;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Insp.Domain.Entities.Inspections;
using IARA.Mobile.Insp.Domain.Entities.Nomenclatures;

namespace IARA.Mobile.Insp.Application.Interfaces.Database
{
    public interface IAppDbContext : IDbContext
    {
        TLTableQuery<Ship> Ships { get; }
        TLTableQuery<NPort> NPorts { get; }
        TLTableQuery<Legal> Legals { get; }
        TLTableQuery<Buyer> Buyers { get; }
        TLTableQuery<NFish> NFishes { get; }
        TLTableQuery<Person> Persons { get; }
        TLTableQuery<NGender> NGenders { get; }
        TLTableQuery<LogBook> LogBooks { get; }
        TLTableQuery<NFishSex> NFishSex { get; }
        TLTableQuery<PoundNet> PoundNets { get; }
        TLTableQuery<NCountry> NCountries { get; }
        TLTableQuery<ShipOwner> ShipOwners { get; }
        TLTableQuery<Inspector> Inspectors { get; }
        TLTableQuery<NDistrict> NDistricts { get; }
        TLTableQuery<NFileType> NFileTypes { get; }
        TLTableQuery<NFleetType> NFleetTypes { get; }
        TLTableQuery<NCatchZone> NCatchZones { get; }
        TLTableQuery<Inspection> Inspections { get; }
        TLTableQuery<FishingGear> FishingGears { get; }
        TLTableQuery<NVesselType> NVesselTypes { get; }
        TLTableQuery<NPermission> NPermissions { get; }
        TLTableQuery<Aquaculture> Aquacultures { get; }
        TLTableQuery<NFishingGear> NFishingGears { get; }
        TLTableQuery<NInstitution> NInstitutions { get; }
        TLTableQuery<NDocumentType> NDocumentTypes { get; }
        TLTableQuery<PatrolVehicle> PatrolVehicles { get; }
        TLTableQuery<PermitLicense> PermitLicenses { get; }
        TLTableQuery<NMunicipality> NMunicipalities { get; }
        TLTableQuery<NWaterBodyType> NWaterBodyTypes { get; }
        TLTableQuery<NPopulatedArea> NPopulatedAreas { get; }
        TLTableQuery<NVesselActivity> NVesselActivitys { get; }
        TLTableQuery<NInspectionType> NInspectionTypes { get; }
        TLTableQuery<FishingGearMark> FishingGearMarks { get; }
        TLTableQuery<RecentInspector> RecentInspectors { get; }
        TLTableQuery<NInspectionState> NInspectionStates { get; }
        TLTableQuery<NObservationTool> NObservationTools { get; }
        TLTableQuery<NShipAssociation> NShipAssociations { get; }
        TLTableQuery<FishingGearPinger> FishingGearPingers { get; }
        TLTableQuery<NFishPresentation> NFishPresentations { get; }
        TLTableQuery<NTranslationGroup> NTranslationGroups { get; }
        TLTableQuery<NRequiredFileType> NRequiredFileTypes { get; }
        TLTableQuery<InspectorsHistory> InspectorsHistories { get; }
        TLTableQuery<NPatrolVehicleType> NPatrolVehicleTypes { get; }
        TLTableQuery<NPermitLicenseType> NPermitLicenseTypes { get; }
        TLTableQuery<PoundNetFishingGear> PoundNetFishingGears { get; }
        TLTableQuery<NInspectionCheckType> NInspectionCheckTypes { get; }
        TLTableQuery<NTranslationResource> NTranslationResources { get; }
        TLTableQuery<NCatchInspectionType> NCatchInspectionTypes { get; }
        TLTableQuery<NInspectedPersonType> NInspectedPersonTypes { get; }
        TLTableQuery<NTransportVehicleType> NTransportVehicleTypes { get; }
        TLTableQuery<PoundNetPermitLicense> PoundNetPermitLicenses { get; }
        TLTableQuery<NFishingGearMarkStatus> NFishingGearMarkStatuses { get; }
        TLTableQuery<PoundNetFishingGearMark> PoundNetFishingGearMarks { get; }
        TLTableQuery<NFishingGearCheckReason> NFishingGearCheckReasons { get; }
        TLTableQuery<NFishingGearPingerStatus> NFishingGearPingerStatuses { get; }
        TLTableQuery<PoundNetFishingGearPinger> PoundNetFishingGearPingers { get; }
        TLTableQuery<NFishingGearRecheckReason> NFishingGearRecheckReasons { get; }
    }
}
