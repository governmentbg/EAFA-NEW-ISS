using IARA.Mobile.Application.Interfaces.Database;
using IARA.Mobile.Domain.Entities.Nomenclatures;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Pub.Domain.Entities.CatchRecords;
using IARA.Mobile.Pub.Domain.Entities.Common;
using IARA.Mobile.Pub.Domain.Entities.FishingTicket;
using IARA.Mobile.Pub.Domain.Entities.Nomenclatures;
using IARA.Mobile.Pub.Domain.Entities.ScientificFishing;

namespace IARA.Mobile.Pub.Application.Interfaces.Database
{
    public interface IAppDbContext : IDbContext
    {
        TLTableQuery<FileInfo> FileInfos { get; }

        TLTableQuery<NFish> NFishes { get; }
        TLTableQuery<NVersion> NVersions { get; }
        TLTableQuery<NCountry> NCountries { get; }
        TLTableQuery<NDistrict> NDistricts { get; }
        TLTableQuery<NFileType> NFileTypes { get; }
        TLTableQuery<NTicketType> NTicketTypes { get; }
        TLTableQuery<NPermitReason> NPermitReasons { get; }
        TLTableQuery<NDocumentType> NDocumentTypes { get; }
        TLTableQuery<NMunicipality> NMunicipalities { get; }
        TLTableQuery<NPopulatedArea> NPopulatedAreas { get; }
        TLTableQuery<NTranslationGroup> NTranslationGroups { get; }
        TLTableQuery<NTicketPeriod> NTicketPeriods { get; }
        TLTableQuery<NTicketTariff> NTicketTariffs { get; }
        TLTableQuery<NTranslationResource> NTranslationResources { get; }
        TLTableQuery<NViolationSignalType> NViolationSignalTypes { get; }
        TLTableQuery<NPermission> NPermissions { get; }
        TLTableQuery<NGender> Genders { get; }
        TLTableQuery<NSystemParameter> SystemParameters { get; }
        TLTableQuery<NPaymentType> PaymentTypes { get; }

        TLTableQuery<SFCatch> SFCatches { get; }
        TLTableQuery<SFPermit> SFPermits { get; }
        TLTableQuery<SFOuting> SFOutings { get; }
        TLTableQuery<SFHolder> SFHolders { get; }
        TLTableQuery<SFPermitReason> SFPermitReasons { get; }

        TLTableQuery<CatchRecord> CatchRecords { get; }
        TLTableQuery<CatchRecordFile> CatchRecordFiles { get; }
        TLTableQuery<CatchRecordFish> CatchRecordFishes { get; }
        TLTableQuery<CatchRecordTicket> CatchRecordTickets { get; }

        TLTableQuery<FishingTicket> FishingTickets { get; }
    }
}
