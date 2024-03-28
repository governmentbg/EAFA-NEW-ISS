using IARA.FluxModels.Enums;
using IARA.Infrastructure.FluxIntegrations.FishingActivitesServices.Models;

namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices
{
    internal class JointFishingOperationService : BaseFishingActivityService
    {
        public JointFishingOperationService(IARADbContext dbContext)
            : base(dbContext)
        { }

        public void MapDeclarationOriginal(FaProcessableMessage message)
        {
            if (message.FishingActivity == null)
            {
                Throw(message, "В съобщението няма FishingActivity за JOINT_FISHING_OPERATION.");
            }

            // добавяме записи в FVMSFishingActivityReportFishes
            AddFvmsFishingActivityReportFishes(message);
            Db.SaveChanges();
        }

        public void MapDeclarationReplace(FaProcessableMessage message)
        {
            if (message.FishingActivity == null)
            {
                Throw(message, "В съобщението няма FishingActivity за корекция на JOINT_FISHING_OPERATION.");
            }

            if (string.IsNullOrEmpty(message.FvmsfishingActivityReport.ReferencedResponseUuid))
            {
                Throw(message, "Не е подадено ReferencedID при корекция на JOINT_FISHING_OPERATION.");
            }

            List<FvmsfishingActivityReport> jfos = GetActiveJointFishingOperations(message.FvmsfishingActivityReport.TripIdentifier);
            FvmsfishingActivityReport replacedJfo = jfos
                .Where(x => x.ResponseUuid == message.FvmsfishingActivityReport.ReferencedResponseUuid)
                .FirstOrDefault();

            if (replacedJfo == null)
            {
                Throw(message, $"Не е открит JOINT_FISHING_OPERATION за корекция с ReferencedID: {message.FvmsfishingActivityReport.ReferencedResponseUuid}");
            }
            else
            {
                if (replacedJfo.IsCurrent)
                {
                    replacedJfo.IsCurrent = false;
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
                Throw(message, "Не е подадено ReferencedID при изтриване на JOINT_FISHING_OPERATION.");
            }

            List<FvmsfishingActivityReport> jfos = GetActiveJointFishingOperations(message.FvmsfishingActivityReport.TripIdentifier);
            FvmsfishingActivityReport deletedJfo = jfos
                .Where(x => x.ResponseUuid == message.FvmsfishingActivityReport.ReferencedResponseUuid)
                .FirstOrDefault();

            if (deletedJfo == null)
            {
                Throw(message, $"Не е открит JOINT_FISHING_OPERATION за изтриване с ReferencedID: {message.FvmsfishingActivityReport.ReferencedResponseUuid}");
            }
            else
            {
                deletedJfo.IsDeleted = true;
            }

            Db.SaveChanges();
        }

        private List<FvmsfishingActivityReport> GetActiveJointFishingOperations(string tripIdentifier)
        {
            List<FvmsfishingActivityReport> result
                = (from report in Db.FvmsfishingActivityReports
                   join faReportType in Db.MdrFluxFaReportTypes on report.MdrFluxFaReportTypeId equals faReportType.Id
                   join faType in Db.MdrFluxFaTypes on report.MdrFluxFaTypeId equals faType.Id
                   join faPurpose in Db.MdrFluxGpPurposes on report.MdrFluxGpPurposeId equals faPurpose.Id
                   where report.Status == nameof(FvmsFishingActivityReportStatus.Processed)
                       && report.TripIdentifier == tripIdentifier
                       && faReportType.Code == nameof(FaReportTypes.DECLARATION)
                       && faType.Code == nameof(FaTypes.JOINT_FISHING_OPERATION)
                       && (faPurpose.Code == ((int)FluxPurposes.Original).ToString()
                            || faPurpose.Code == ((int)FluxPurposes.Replace).ToString())
                       && !report.IsDeleted
                   orderby report.CreatedOn
                   select report).ToList();

            return result;
        }
    }
}
