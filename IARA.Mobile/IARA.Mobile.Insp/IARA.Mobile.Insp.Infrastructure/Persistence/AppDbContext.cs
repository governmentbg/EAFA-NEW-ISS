using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using IARA.Mobile.Domain.Entities.Exceptions;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Insp.Application.Interfaces.Database;
using IARA.Mobile.Insp.Domain.Entities.Inspections;
using IARA.Mobile.Insp.Domain.Entities.Nomenclatures;
using SQLite;

namespace IARA.Mobile.Insp.Infrastructure.Persistence
{
    public class AppDbContext : SQLiteConnection, IAppDbContext
    {
        private readonly IDictionary<string, BaseTableQuery> _resolvedTables;

        #region Common

        public TLTableQuery<ErrorLog> ErrorLogs => TLTable<ErrorLog>();

        #endregion Common

        #region Nomenclatures

        public TLTableQuery<NPort> NPorts => TLTable<NPort>();
        public TLTableQuery<NFish> NFishes => TLTable<NFish>();
        public TLTableQuery<NGender> NGenders => TLTable<NGender>();
        public TLTableQuery<NFishSex> NFishSex => TLTable<NFishSex>();
        public TLTableQuery<NCountry> NCountries => TLTable<NCountry>();
        public TLTableQuery<NDistrict> NDistricts => TLTable<NDistrict>();
        public TLTableQuery<NFileType> NFileTypes => TLTable<NFileType>();
        public TLTableQuery<NFleetType> NFleetTypes => TLTable<NFleetType>();
        public TLTableQuery<NCatchZone> NCatchZones => TLTable<NCatchZone>();
        public TLTableQuery<NVesselType> NVesselTypes => TLTable<NVesselType>();
        public TLTableQuery<NPermission> NPermissions => TLTable<NPermission>();
        public TLTableQuery<NPermitType> NPermitTypes => TLTable<NPermitType>();
        public TLTableQuery<NFishingGear> NFishingGears => TLTable<NFishingGear>();
        public TLTableQuery<NInstitution> NInstitutions => TLTable<NInstitution>();
        public TLTableQuery<NDocumentType> NDocumentTypes => TLTable<NDocumentType>();
        public TLTableQuery<NMunicipality> NMunicipalities => TLTable<NMunicipality>();
        public TLTableQuery<NPopulatedArea> NPopulatedAreas => TLTable<NPopulatedArea>();
        public TLTableQuery<NWaterBodyType> NWaterBodyTypes => TLTable<NWaterBodyType>();
        public TLTableQuery<NVesselActivity> NVesselActivitys => TLTable<NVesselActivity>();
        public TLTableQuery<NInspectionType> NInspectionTypes => TLTable<NInspectionType>();
        public TLTableQuery<NInspectionState> NInspectionStates => TLTable<NInspectionState>();
        public TLTableQuery<NObservationTool> NObservationTools => TLTable<NObservationTool>();
        public TLTableQuery<NShipAssociation> NShipAssociations => TLTable<NShipAssociation>();
        public TLTableQuery<NTurbotSizeGroup> NTurbotSizeGroups => TLTable<NTurbotSizeGroup>();
        public TLTableQuery<NFishPresentation> NFishPresentations => TLTable<NFishPresentation>();
        public TLTableQuery<NTranslationGroup> NTranslationGroups => TLTable<NTranslationGroup>();
        public TLTableQuery<NRequiredFileType> NRequiredFileTypes => TLTable<NRequiredFileType>();
        public TLTableQuery<NPatrolVehicleType> NPatrolVehicleTypes => TLTable<NPatrolVehicleType>();
        public TLTableQuery<NPermitLicenseType> NPermitLicenseTypes => TLTable<NPermitLicenseType>();
        public TLTableQuery<NInspectionCheckType> NInspectionCheckTypes => TLTable<NInspectionCheckType>();
        public TLTableQuery<NTranslationResource> NTranslationResources => TLTable<NTranslationResource>();
        public TLTableQuery<NCatchInspectionType> NCatchInspectionTypes => TLTable<NCatchInspectionType>();
        public TLTableQuery<NInspectedPersonType> NInspectedPersonTypes => TLTable<NInspectedPersonType>();
        public TLTableQuery<NTransportVehicleType> NTransportVehicleTypes => TLTable<NTransportVehicleType>();
        public TLTableQuery<NFishingGearMarkStatus> NFishingGearMarkStatuses => TLTable<NFishingGearMarkStatus>();
        public TLTableQuery<NFishingGearCheckReason> NFishingGearCheckReasons => TLTable<NFishingGearCheckReason>();
        public TLTableQuery<NFishingGearPingerStatus> NFishingGearPingerStatuses => TLTable<NFishingGearPingerStatus>();
        public TLTableQuery<NFishingGearRecheckReason> NFishingGearRecheckReasons => TLTable<NFishingGearRecheckReason>();

        #endregion Nomenclatures

        #region Inspections

        public TLTableQuery<PoundNetFishingGearPinger> PoundNetFishingGearPingers => TLTable<PoundNetFishingGearPinger>();
        public TLTableQuery<PoundNetFishingGearMark> PoundNetFishingGearMarks => TLTable<PoundNetFishingGearMark>();
        public TLTableQuery<PoundNetPermitLicense> PoundNetPermitLicenses => TLTable<PoundNetPermitLicense>();
        public TLTableQuery<PoundNetFishingGear> PoundNetFishingGears => TLTable<PoundNetFishingGear>();
        public TLTableQuery<InspectorsHistory> InspectorsHistories => TLTable<InspectorsHistory>();
        public TLTableQuery<FishingGearPinger> FishingGearPingers => TLTable<FishingGearPinger>();
        public TLTableQuery<RecentInspector> RecentInspectors => TLTable<RecentInspector>();
        public TLTableQuery<FishingGearMark> FishingGearMarks => TLTable<FishingGearMark>();
        public TLTableQuery<PatrolVehicle> PatrolVehicles => TLTable<PatrolVehicle>();
        public TLTableQuery<PermitLicense> PermitLicenses => TLTable<PermitLicense>();
        public TLTableQuery<FishingGear> FishingGears => TLTable<FishingGear>();
        public TLTableQuery<Aquaculture> Aquacultures => TLTable<Aquaculture>();
        public TLTableQuery<Inspection> Inspections => TLTable<Inspection>();
        public TLTableQuery<Inspector> Inspectors => TLTable<Inspector>();
        public TLTableQuery<ShipOwner> ShipOwners => TLTable<ShipOwner>();
        public TLTableQuery<PoundNet> PoundNets => TLTable<PoundNet>();
        public TLTableQuery<LogBook> LogBooks => TLTable<LogBook>();
        public TLTableQuery<Person> Persons => TLTable<Person>();
        public TLTableQuery<Permit> Permits => TLTable<Permit>();
        public TLTableQuery<Buyer> Buyers => TLTable<Buyer>();
        public TLTableQuery<Legal> Legals => TLTable<Legal>();
        public TLTableQuery<Ship> Ships => TLTable<Ship>();

        #endregion Inspections

        public AppDbContext(string dbPath)
            : base(dbPath)
        {
            _resolvedTables = new Dictionary<string, BaseTableQuery>();
        }

        public void DeleteAllTables()
        {
            IEnumerable<PropertyInfo> tables = GetType()
                .GetProperties()
                .Where(f => f.PropertyType.IsAssignableFrom(typeof(BaseTableQuery)));

            foreach (PropertyInfo table in tables)
            {
                DeleteAll(new TableMapping(table.PropertyType.GetGenericArguments()[0]));
            }
        }

        public TLTableQuery<TEntity> TLTable<TEntity>([CallerMemberName] string propertyName = "")
        {
            if (_resolvedTables.TryGetValue(propertyName, out BaseTableQuery table))
            {
                return (TLTableQuery<TEntity>)table;
            }

            TLTableQuery<TEntity> newTable = new TLTableQuery<TEntity>(this);
            _resolvedTables.Add(propertyName, newTable);
            return newTable;
        }

        public TLTableQuery<TEntity> TLTable<TEntity>()
        {
            return new TLTableQuery<TEntity>(this);
        }
    }
}
