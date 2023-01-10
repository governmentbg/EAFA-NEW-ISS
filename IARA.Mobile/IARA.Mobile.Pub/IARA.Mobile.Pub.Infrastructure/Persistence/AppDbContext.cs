using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using IARA.Mobile.Domain.Entities.Exceptions;
using IARA.Mobile.Domain.Entities.Nomenclatures;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Pub.Application.Interfaces.Database;
using IARA.Mobile.Pub.Domain.Entities.CatchRecords;
using IARA.Mobile.Pub.Domain.Entities.Common;
using IARA.Mobile.Pub.Domain.Entities.FishingTicket;
using IARA.Mobile.Pub.Domain.Entities.Nomenclatures;
using IARA.Mobile.Pub.Domain.Entities.ScientificFishing;
using SQLite;

namespace IARA.Mobile.Pub.Infrastructure.Persistence
{
    public class AppDbContext : SQLiteConnection, IAppDbContext
    {
        private readonly IDictionary<string, BaseTableQuery> _resolvedTables;

        #region Common

        public TLTableQuery<ErrorLog> ErrorLogs => TLTable<ErrorLog>();

        public TLTableQuery<FileInfo> FileInfos => TLTable<FileInfo>();

        #endregion Common

        #region Nomenclatures

        public TLTableQuery<NFish> NFishes => TLTable<NFish>();
        public TLTableQuery<NVersion> NVersions => TLTable<NVersion>();
        public TLTableQuery<NCountry> NCountries => TLTable<NCountry>();
        public TLTableQuery<NDistrict> NDistricts => TLTable<NDistrict>();
        public TLTableQuery<NFileType> NFileTypes => TLTable<NFileType>();
        public TLTableQuery<NTicketType> NTicketTypes => TLTable<NTicketType>();
        public TLTableQuery<NPermitReason> NPermitReasons => TLTable<NPermitReason>();
        public TLTableQuery<NDocumentType> NDocumentTypes => TLTable<NDocumentType>();
        public TLTableQuery<NMunicipality> NMunicipalities => TLTable<NMunicipality>();
        public TLTableQuery<NPopulatedArea> NPopulatedAreas => TLTable<NPopulatedArea>();
        public TLTableQuery<NTranslationGroup> NTranslationGroups => TLTable<NTranslationGroup>();
        public TLTableQuery<NTicketPeriod> NTicketPeriods => TLTable<NTicketPeriod>();
        public TLTableQuery<NTicketTariff> NTicketTariffs => TLTable<NTicketTariff>();
        public TLTableQuery<NTranslationResource> NTranslationResources => TLTable<NTranslationResource>();
        public TLTableQuery<NViolationSignalType> NViolationSignalTypes => TLTable<NViolationSignalType>();
        public TLTableQuery<NPermission> NPermissions => TLTable<NPermission>();
        public TLTableQuery<NGender> Genders => TLTable<NGender>();
        public TLTableQuery<NSystemParameter> SystemParameters => TLTable<NSystemParameter>();
        public TLTableQuery<NPaymentType> PaymentTypes => TLTable<NPaymentType>();

        #endregion Nomenclatures

        #region Scientific Fishing

        public TLTableQuery<SFCatch> SFCatches => TLTable<SFCatch>();
        public TLTableQuery<SFPermit> SFPermits => TLTable<SFPermit>();
        public TLTableQuery<SFOuting> SFOutings => TLTable<SFOuting>();
        public TLTableQuery<SFHolder> SFHolders => TLTable<SFHolder>();
        public TLTableQuery<SFPermitReason> SFPermitReasons => TLTable<SFPermitReason>();

        #endregion Scientific Fishing

        #region Catch Records

        public TLTableQuery<CatchRecord> CatchRecords => TLTable<CatchRecord>();
        public TLTableQuery<CatchRecordFile> CatchRecordFiles => TLTable<CatchRecordFile>();
        public TLTableQuery<CatchRecordFish> CatchRecordFishes => TLTable<CatchRecordFish>();
        public TLTableQuery<CatchRecordTicket> CatchRecordTickets => TLTable<CatchRecordTicket>();

        #endregion Catch Records

        #region Fishing tickets

        public TLTableQuery<FishingTicket> FishingTickets => TLTable<FishingTicket>();

        #endregion

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
