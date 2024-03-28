using IARA.FluxModels.Enums;
using IARA.Infrastructure.FluxIntegrations.FishingActivitesServices.Models;

namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices
{
    internal class AreaExitService : BaseFishingActivityService
    {
        public AreaExitService(IARADbContext dbContext)
            : base(dbContext)
        { }

        // Notification
        public void MapNotificationOriginal(FaProcessableMessage message)
        {
            if (message.FishingActivity == null)
            {
                Throw(message, "Не е подадено FishingActivity за AREA_EXIT NOTIFICATION");
            }

            // добавяме записи в FVMSFishingActivityReportFishes
            AddFvmsFishingActivityReportFishes(message);
            Db.SaveChanges();
        }

        public void MapNotificationReplace(FaProcessableMessage message)
        {
            if (message.FishingActivity == null)
            {
                Throw(message, "Не е подадено FishingActivity за корекция на AREA_EXIT NOTIFICATION");
            }

            if (string.IsNullOrEmpty(message.FvmsfishingActivityReport.ReferencedResponseUuid))
            {
                Throw(message, "Няма подадено ReferencedID за корекция на AREA_EXIT NOTIFICATION");
            }

            List<FvmsfishingActivityReport> areaExits = GetActiveAreaExitNotifications(message.FvmsfishingActivityReport.TripIdentifier);
            FvmsfishingActivityReport replacedAreaExit = areaExits
                .Where(x => x.ResponseUuid == message.FvmsfishingActivityReport.ReferencedResponseUuid)
                .FirstOrDefault();

            if (replacedAreaExit == null)
            {
                Throw(message, $"Не е открито AREA_EXIT NOTIFICATION за корекция с ReferencedID: {message.FvmsfishingActivityReport.ReferencedResponseUuid}");
            }
            else
            {
                if (replacedAreaExit.IsCurrent)
                {
                    replacedAreaExit.IsCurrent = false;
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
                Throw(message, "Няма подадено ReferencedID за отмяна на AREA_EXIT NOTIFICATION");
            }

            List<FvmsfishingActivityReport> areaExits = GetActiveAreaExitNotifications(message.FvmsfishingActivityReport.TripIdentifier);
            FvmsfishingActivityReport canceledAreaExit = areaExits
                .Where(x => x.ResponseUuid == message.FvmsfishingActivityReport.ReferencedResponseUuid)
                .FirstOrDefault();

            if (canceledAreaExit == null)
            {
                Throw(message, $"Не е открито AREA_EXIT NOTIFICATION за отмяна с ReferencedID: {message.FvmsfishingActivityReport.ReferencedResponseUuid}");
            }
            else
            {
                canceledAreaExit.IsCanceled = true;
            }

            Db.SaveChanges();
        }

        // Declaration
        public void MapDeclarationOriginal(FaProcessableMessage message)
        {
            if (message.FishingActivity == null)
            {
                Throw(message, "Не е подадено FishingActivity за корекция на AREA_EXIT DECLARATION");
            }

            // добавяме записи в FVMSFishingActivityReportFishes
            AddFvmsFishingActivityReportFishes(message);
            Db.SaveChanges();
        }

        public void MapDeclarationReplace(FaProcessableMessage message)
        {
            if (message.FishingActivity == null)
            {
                Throw(message, "Не е подадено FishingActivity за корекция на AREA_EXIT DECLARATION");
            }

            if (string.IsNullOrEmpty(message.FvmsfishingActivityReport.ReferencedResponseUuid))
            {
                Throw(message, "Няма подадено ReferencedID за корекция на AREA_EXIT DECLARATION");
            }

            List<FvmsfishingActivityReport> areaExits = GetActiveAreaExitDeclarations(message.FvmsfishingActivityReport.TripIdentifier);
            FvmsfishingActivityReport replacedAreaExit = areaExits
                .Where(x => x.ResponseUuid == message.FvmsfishingActivityReport.ReferencedResponseUuid)
                .FirstOrDefault();

            if (replacedAreaExit == null)
            {
                Throw(message, $"Не е открито AREA_EXIT DECLARATION за корекция с ReferencedID: {message.FvmsfishingActivityReport.ReferencedResponseUuid}");
            }
            else
            {
                if (replacedAreaExit.IsCurrent)
                {
                    replacedAreaExit.IsCurrent = false;
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
                Throw(message, "Няма подадено ReferencedID за изтриване на AREA_EXIT DECLARATION");
            }

            List<FvmsfishingActivityReport> areaExits = GetActiveAreaExitDeclarations(message.FvmsfishingActivityReport.TripIdentifier);
            FvmsfishingActivityReport deletedAreaExit = areaExits
                .Where(x => x.ResponseUuid == message.FvmsfishingActivityReport.ReferencedResponseUuid)
                .FirstOrDefault();

            if (deletedAreaExit == null)
            {
                Throw(message, $"Не е открито AREA_EXIT DECLARATION за изтриване с ReferencedID: {message.FvmsfishingActivityReport.ReferencedResponseUuid}");
            }
            else
            {
                deletedAreaExit.IsDeleted = true;
            }

            Db.SaveChanges();
        }

        private List<FvmsfishingActivityReport> GetActiveAreaExitNotifications(string tripIdentifier)
        {
            List<FvmsfishingActivityReport> result
                = (from report in Db.FvmsfishingActivityReports
                   join faReportType in Db.MdrFluxFaReportTypes on report.MdrFluxFaReportTypeId equals faReportType.Id
                   join faType in Db.MdrFluxFaTypes on report.MdrFluxFaTypeId equals faType.Id
                   join faPurpose in Db.MdrFluxGpPurposes on report.MdrFluxGpPurposeId equals faPurpose.Id
                   where report.Status == nameof(FvmsFishingActivityReportStatus.Processed)
                       && report.TripIdentifier == tripIdentifier
                       && faReportType.Code == nameof(FaReportTypes.NOTIFICATION)
                       && faType.Code == nameof(FaTypes.AREA_EXIT)
                       && (faPurpose.Code == ((int)FluxPurposes.Original).ToString()
                            || faPurpose.Code == ((int)FluxPurposes.Replace).ToString())
                       && !report.IsCanceled
                   orderby report.CreatedOn
                   select report).ToList();

            return result;
        }

        private List<FvmsfishingActivityReport> GetActiveAreaExitDeclarations(string tripIdentifier)
        {
            List<FvmsfishingActivityReport> result
                = (from report in Db.FvmsfishingActivityReports
                   join faReportType in Db.MdrFluxFaReportTypes on report.MdrFluxFaReportTypeId equals faReportType.Id
                   join faType in Db.MdrFluxFaTypes on report.MdrFluxFaTypeId equals faType.Id
                   join faPurpose in Db.MdrFluxGpPurposes on report.MdrFluxGpPurposeId equals faPurpose.Id
                   where report.Status == nameof(FvmsFishingActivityReportStatus.Processed)
                       && report.TripIdentifier == tripIdentifier
                       && faReportType.Code == nameof(FaReportTypes.DECLARATION)
                       && faType.Code == nameof(FaTypes.AREA_EXIT)
                       && (faPurpose.Code == ((int)FluxPurposes.Original).ToString()
                            || faPurpose.Code == ((int)FluxPurposes.Replace).ToString())
                       && !report.IsDeleted
                   orderby report.CreatedOn
                   select report).ToList();

            return result;
        }
    }
}
