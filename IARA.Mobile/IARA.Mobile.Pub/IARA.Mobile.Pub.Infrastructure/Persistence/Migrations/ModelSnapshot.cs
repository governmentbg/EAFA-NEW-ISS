using IARA.Mobile.Domain.Entities.Exceptions;
using IARA.Mobile.Domain.Entities.Nomenclatures;
using IARA.Mobile.Pub.Domain.Entities.CatchRecords;
using IARA.Mobile.Pub.Domain.Entities.Common;
using IARA.Mobile.Pub.Domain.Entities.FishingTicket;
using IARA.Mobile.Pub.Domain.Entities.Nomenclatures;
using IARA.Mobile.Pub.Domain.Entities.ScientificFishing;

namespace IARA.Mobile.Pub.Infrastructure.Persistence.Migrations
{
    public class ModelSnapshot
    {
        private readonly AppDbContext context;

        public ModelSnapshot(AppDbContext context)
        {
            this.context = context;
        }

        public void CreateDatabase()
        {
            context.CreateTable<NFish>();
            context.CreateTable<NCountry>();
            context.CreateTable<NDistrict>();
            context.CreateTable<NTicketType>();
            context.CreateTable<NDocumentType>();
            context.CreateTable<NPermitReason>();
            context.CreateTable<NMunicipality>();
            context.CreateTable<NPopulatedArea>();
            context.CreateTable<NTranslationGroup>();
            context.CreateTable<NTicketPeriod>();
            context.CreateTable<NTicketTariff>();
            context.CreateTable<NTranslationResource>();
            context.CreateTable<NViolationSignalType>();
            context.CreateTable<NPermission>();
            context.CreateTable<NFileType>();
            context.CreateTable<NPermitReason>();
            context.CreateTable<SFCatch>();
            context.CreateTable<SFPermit>();
            context.CreateTable<SFOuting>();
            context.CreateTable<SFHolder>();
            context.CreateTable<SFPermitReason>();
            context.CreateTable<FileInfo>();
            context.CreateTable<CatchRecord>();
            context.CreateTable<CatchRecordFish>();
            context.CreateTable<CatchRecordFile>();
            context.CreateTable<CatchRecordTicket>();
            context.CreateTable<NVersion>();
            context.CreateTable<FishingTicket>();
            context.CreateTable<NGender>();
            context.CreateTable<NSystemParameter>();
            context.CreateTable<ErrorLog>();
            context.CreateTable<NPaymentType>();
            context.CreateTable<NTerritorialUnit>();
        }
    }
}
