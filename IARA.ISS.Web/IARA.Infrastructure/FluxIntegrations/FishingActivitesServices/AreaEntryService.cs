using IARA.FluxModels.Enums;
using IARA.Infrastructure.FluxIntegrations.FishingActivitesServices.Models;

namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices
{
    internal class AreaEntryService : BaseFishingActivityService
    {
        public AreaEntryService(IARADbContext dbContext)
            : base(dbContext)
        { }

        // Notification
        public void MapNotificationOriginal(FaProcessableMessage message)
        {
            if (message.FishingActivity == null)
            {
                Throw(message, "В съобщението няма FishingActivity за AREA_ENTRY NOTIFICAITON.");
            }

            // добавяме записи в FVMSFishingActivityReportFishes
            AddFvmsFishingActivityReportFishes(message);
            Db.SaveChanges();
        }

        public void MapNotificationReplace(FaProcessableMessage message)
        {
            if (message.FishingActivity == null)
            {
                Throw(message, "В съобщението няма FishingActivity за корекция на AREA_ENTRY NOTIFICAITON.");
            }

            if (string.IsNullOrEmpty(message.FvmsfishingActivityReport.ReferencedResponseUuid))
            {
                Throw(message, "Не е подадено ReferencedID при корекция на AREA_ENTRY NOTIFICAITON.");
            }

            List<FvmsfishingActivityReport> areaEntries = GetActiveAreaEntryNotifications(message.FvmsfishingActivityReport.TripIdentifier);
            FvmsfishingActivityReport replacedAreaEntry = areaEntries
                .Where(x => x.ResponseUuid == message.FvmsfishingActivityReport.ReferencedResponseUuid)
                .FirstOrDefault();

            if (replacedAreaEntry == null)
            {
                Throw(message, $"Не е открито AREA_ENTRY NOTIFICATION за корекция с ReferencedID: {message.FvmsfishingActivityReport.ReferencedResponseUuid}");
            }
            else
            {
                if (replacedAreaEntry.IsCurrent)
                {
                    replacedAreaEntry.IsCurrent = false;
                }
                else
                {
                    message.FvmsfishingActivityReport.IsCurrent = false;
                }
            }

            // добавяме записи в FVMSFishingActivityReportFishes
            AddFvmsFishingActivityReportFishes(message);
            Db.SaveChanges();
        }

        public void MapNotificationCancel(FaProcessableMessage message)
        {
            if (string.IsNullOrEmpty(message.FvmsfishingActivityReport.ReferencedResponseUuid))
            {
                Throw(message, "Не е подадено ReferencedID при отмяна на AREA_ENTRY NOTIFICAITON.");
            }

            List<FvmsfishingActivityReport> areaEntries = GetActiveAreaEntryNotifications(message.FvmsfishingActivityReport.TripIdentifier);
            FvmsfishingActivityReport canceledAreaEntry = areaEntries
                .Where(x => x.ResponseUuid == message.FvmsfishingActivityReport.ReferencedResponseUuid)
                .FirstOrDefault();

            if (canceledAreaEntry == null)
            {
                Throw(message, $"Не е открито AREA_ENTRY NOTIFICATION за отмяна с ReferencedID: {message.FvmsfishingActivityReport.ReferencedResponseUuid}");
            }
            else
            {
                canceledAreaEntry.IsCanceled = true;
            }

            Db.SaveChanges();
        }

        // Declaration
        public void MapDeclarationOriginal(FaProcessableMessage message)
        {
            if (message.FishingActivity == null)
            {
                Throw(message, "В съобщението няма FishingActivity за AREA_ENTRY DECLARATION.");
            }

            // добавяме записи в FVMSFishingActivityReportFishes
            AddFvmsFishingActivityReportFishes(message);
            Db.SaveChanges();
        }

        public void MapDeclarationReplace(FaProcessableMessage message)
        {
            if (message.FishingActivity == null)
            {
                Throw(message, "В съобщението няма FishingActivity за корекция на AREA_ENTRY DECLARATION.");
            }

            if (string.IsNullOrEmpty(message.FvmsfishingActivityReport.ReferencedResponseUuid))
            {
                Throw(message, "Не е подадено ReferencedID при корекция на AREA_ENTRY DECLARATION.");
            }

            List<FvmsfishingActivityReport> areaEntries = GetActiveAreaEntryDeclarations(message.FvmsfishingActivityReport.TripIdentifier);
            FvmsfishingActivityReport replacedAreaEntry = areaEntries
                .Where(x => x.ResponseUuid == message.FvmsfishingActivityReport.ReferencedResponseUuid)
                .FirstOrDefault();

            if (replacedAreaEntry == null)
            {
                Throw(message, $"Не е открито AREA_ENTRY DECLARATION за корекция с ReferencedID: {message.FvmsfishingActivityReport.ReferencedResponseUuid}");
            }
            else
            {
                if (replacedAreaEntry.IsCurrent)
                {
                    replacedAreaEntry.IsCurrent = false;
                }
                else
                {
                    message.FvmsfishingActivityReport.IsCurrent = false;
                }
            }

            // добавяме записи в FVMSFishingActivityReportFishes
            AddFvmsFishingActivityReportFishes(message);
            Db.SaveChanges();
        }

        public void MapDeclarationDelete(FaProcessableMessage message)
        {
            if (string.IsNullOrEmpty(message.FvmsfishingActivityReport.ReferencedResponseUuid))
            {
                Throw(message, "Не е подадено ReferencedID при изтриване на AREA_ENTRY DECLARATION.");
            }

            List<FvmsfishingActivityReport> areaEntries = GetActiveAreaEntryDeclarations(message.FvmsfishingActivityReport.TripIdentifier);
            FvmsfishingActivityReport deletedAreaEntry = areaEntries
                .Where(x => x.ResponseUuid == message.FvmsfishingActivityReport.ReferencedResponseUuid)
                .FirstOrDefault();

            if (deletedAreaEntry == null)
            {
                Throw(message, $"Не е открито AREA_ENTRY DECLARATION за изтриване с ReferencedID: {message.FvmsfishingActivityReport.ReferencedResponseUuid}");
            }
            else
            {
                deletedAreaEntry.IsDeleted = true;
            }

            Db.SaveChanges();
        }

        private List<FvmsfishingActivityReport> GetActiveAreaEntryNotifications(string tripIdentifier)
        {
            List<FvmsfishingActivityReport> result =
                (from report in Db.FvmsfishingActivityReports
                 join faReportType in Db.MdrFluxFaReportTypes on report.MdrFluxFaReportTypeId equals faReportType.Id
                 join faType in Db.MdrFluxFaTypes on report.MdrFluxFaTypeId equals faType.Id
                 join faPurpose in Db.MdrFluxGpPurposes on report.MdrFluxGpPurposeId equals faPurpose.Id
                 where report.Status == nameof(FvmsFishingActivityReportStatus.Processed)
                     && report.TripIdentifier == tripIdentifier
                     && faReportType.Code == nameof(FaReportTypes.NOTIFICATION)
                     && faType.Code == nameof(FaTypes.AREA_ENTRY)
                     && (faPurpose.Code == ((int)FluxPurposes.Original).ToString()
                            || faPurpose.Code == ((int)FluxPurposes.Replace).ToString())
                     && !report.IsCanceled
                 orderby report.CreatedOn
                 select report).ToList();

            return result;
        }

        private List<FvmsfishingActivityReport> GetActiveAreaEntryDeclarations(string tripIdentifier)
        {
            List<FvmsfishingActivityReport> result =
                (from report in Db.FvmsfishingActivityReports
                 join faReportType in Db.MdrFluxFaReportTypes on report.MdrFluxFaReportTypeId equals faReportType.Id
                 join faType in Db.MdrFluxFaTypes on report.MdrFluxFaTypeId equals faType.Id
                 join faPurpose in Db.MdrFluxGpPurposes on report.MdrFluxGpPurposeId equals faPurpose.Id
                 where report.Status == nameof(FvmsFishingActivityReportStatus.Processed)
                     && report.TripIdentifier == tripIdentifier
                     && faReportType.Code == nameof(FaReportTypes.DECLARATION)
                     && faType.Code == nameof(FaTypes.AREA_ENTRY)
                     && (faPurpose.Code == ((int)FluxPurposes.Original).ToString()
                            || faPurpose.Code == ((int)FluxPurposes.Replace).ToString())
                     && !report.IsDeleted
                 orderby report.CreatedOn
                 select report).ToList();

            return result;
        }
    }
}
