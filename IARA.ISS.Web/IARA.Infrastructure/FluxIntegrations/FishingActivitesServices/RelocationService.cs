using IARA.FluxModels.Enums;
using IARA.Infrastructure.FluxIntegrations.FishingActivitesServices.Models;

namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices
{
    internal class RelocationService : BaseFishingActivityService
    {
        public RelocationService(IARADbContext dbContext)
            : base(dbContext)
        { }

        // Notifications
        public void MapNotificationOriginal(FaProcessableMessage message)
        {
            if (message.FishingActivity == null)
            {
                Throw(message, "В съобщението няма FishingActivity за RELOCATION NOTIFICAITON.");
            }

            // добавяме записи в FVMSFishingActivityReportFishes
            AddFvmsFishingActivityReportFishes(message);
            Db.SaveChanges();
        }

        public void MapNotificationReplace(FaProcessableMessage message)
        {
            if (message.FishingActivity == null)
            {
                Throw(message, "В съобщението няма FishingActivity за корекция на RELOCATION NOTIFICAITON.");
            }

            if (string.IsNullOrEmpty(message.FvmsfishingActivityReport.ReferencedResponseUuid))
            {
                Throw(message, "Не е подадено ReferencedID при корекция на RELOCATION NOTIFICAITON.");
            }

            List<FvmsfishingActivityReport> notifications = GetActiveRelocationNotifications(message.FvmsfishingActivityReport.TripIdentifier);
            FvmsfishingActivityReport replacedRelocation = notifications
                .Where(x => x.ResponseUuid == message.FvmsfishingActivityReport.ReferencedResponseUuid)
                .FirstOrDefault();

            if (replacedRelocation == null)
            {
                Throw(message, $"Не е открито RELOCATION NOTIFICATION за корекция с ReferencedID: {message.FvmsfishingActivityReport.ReferencedResponseUuid}");
            }
            else
            {
                if (replacedRelocation.IsCurrent)
                {
                    replacedRelocation.IsCurrent = false;
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
                Throw(message, "Не е подадено ReferencedID при отмяна на RELOCATION NOTIFICAITON.");
            }

            List<FvmsfishingActivityReport> notifications = GetActiveRelocationNotifications(message.FvmsfishingActivityReport.TripIdentifier);
            FvmsfishingActivityReport canceledRelocation = notifications
                .Where(x => x.ResponseUuid == message.FvmsfishingActivityReport.ReferencedResponseUuid)
                .FirstOrDefault();

            if (canceledRelocation == null)
            {
                Throw(message, $"Не е открито RELOCATION NOTIFICATION за отмяна с ReferencedID: {message.FvmsfishingActivityReport.ReferencedResponseUuid}");
            }
            else
            {
                canceledRelocation.IsCanceled = true;
            }

            Db.SaveChanges();
        }

        // Declarations
        public void MapDeclarationOriginal(FaProcessableMessage message)
        {
            if (message.FishingActivity == null)
            {
                Throw(message, "В съобщението няма FishingActivity за RELOCATION DECLARATION.");
            }

            // добавяме записи в FVMSFishingActivityReportFishes
            AddFvmsFishingActivityReportFishes(message);
            Db.SaveChanges();
        }

        public void MapDeclarationReplace(FaProcessableMessage message)
        {
            if (message.FishingActivity == null)
            {
                Throw(message, "В съобщението няма FishingActivity за корекция на RELOCATION DECLARATION.");
            }

            if (string.IsNullOrEmpty(message.FvmsfishingActivityReport.ReferencedResponseUuid))
            {
                Throw(message, "Не е подадено ReferencedID при корекция на RELOCATION DECLARATION.");
            }

            List<FvmsfishingActivityReport> declarations = GetActiveRelocationDeclarations(message.FvmsfishingActivityReport.TripIdentifier);
            FvmsfishingActivityReport replacedRelocation = declarations
                .Where(x => x.ResponseUuid == message.FvmsfishingActivityReport.ReferencedResponseUuid)
                .FirstOrDefault();

            if (replacedRelocation == null)
            {
                Throw(message, $"Не е открито AREA_ENTRY RELOCATION за корекция с ReferencedID: {message.FvmsfishingActivityReport.ReferencedResponseUuid}");
            }
            else
            {
                if (replacedRelocation.IsCurrent)
                {
                    replacedRelocation.IsCurrent = false;
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
                Throw(message, "Не е подадено ReferencedID при изтриване на RELOCATION DECLARATION.");
            }

            List<FvmsfishingActivityReport> declarations = GetActiveRelocationDeclarations(message.FvmsfishingActivityReport.TripIdentifier);
            FvmsfishingActivityReport deletedRelocation = declarations
                .Where(x => x.ResponseUuid == message.FvmsfishingActivityReport.ReferencedResponseUuid)
                .FirstOrDefault();

            if (deletedRelocation == null)
            {
                Throw(message, $"Не е открито AREA_ENTRY DECLARATION за изтриване с ReferencedID: {message.FvmsfishingActivityReport.ReferencedResponseUuid}");
            }
            else
            {
                deletedRelocation.IsDeleted = true;
            }

            Db.SaveChanges();
        }

        private List<FvmsfishingActivityReport> GetActiveRelocationNotifications(string tripIdentifier)
        {
            List<FvmsfishingActivityReport> result
                = (from report in Db.FvmsfishingActivityReports
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

        private List<FvmsfishingActivityReport> GetActiveRelocationDeclarations(string tripIdentifier)
        {
            List<FvmsfishingActivityReport> result
                = (from report in Db.FvmsfishingActivityReports
                   join faReportType in Db.MdrFluxFaReportTypes on report.MdrFluxFaReportTypeId equals faReportType.Id
                   join faType in Db.MdrFluxFaTypes on report.MdrFluxFaTypeId equals faType.Id
                   join faPurpose in Db.MdrFluxGpPurposes on report.MdrFluxGpPurposeId equals faPurpose.Id
                   where report.Status == nameof(FvmsFishingActivityReportStatus.Processed)
                       && report.TripIdentifier == tripIdentifier
                       && faReportType.Code == nameof(FaReportTypes.DECLARATION)
                       && faType.Code == nameof(FaTypes.RELOCATION)
                       && (faPurpose.Code == ((int)FluxPurposes.Original).ToString()
                              || faPurpose.Code == ((int)FluxPurposes.Replace).ToString())
                       && !report.IsDeleted
                   orderby report.CreatedOn
                   select report).ToList();

            return result;
        }
    }
}
