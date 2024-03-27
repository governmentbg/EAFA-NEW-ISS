using IARA.FluxModels.Enums;
using IARA.Infrastructure.FluxIntegrations.FishingActivitesServices.Models;

namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices
{
    internal class ArrivalService : BaseFishingActivityService
    {
        public ArrivalService(IARADbContext dbContext)
            : base(dbContext)
        { }

        // Notifications
        public void MapNotificationOriginal(FaProcessableMessage message)
        {
            if (message.FishingActivity == null)
            {
                Throw(message, "Няма подадено FishingActivity за ARRIVAL NOTIFICATION");
            }

            // добавяме записи в FVMSFishingActivityReportFishes, ако има улов на борда
            AddFvmsFishingActivityReportFishes(message);

            // редактираме часа и пристанището на връщане на всички страници, отворени по време на рейса
            List<ShipLogBookPage> pages = GetOpenedPagesByTripIdentifier(message.FvmsfishingActivityReport.TripIdentifier);

            int? arrivePortId = GetPortIdFromMdrLocation(message.FvmsfishingActivityReport.MdrLocationId);

            foreach (ShipLogBookPage page in pages)
            {
                page.FishTripEndDateTime = message.OccurrenceDateTime;
                page.ArrivePortId = arrivePortId;

                AddFvmsFishingActivityReportLogBookPage(message.FvmsfishingActivityReport, page);
            }

            Db.SaveChanges();
        }

        public void MapNotificationReplace(FaProcessableMessage message)
        {
            if (message.FishingActivity == null)
            {
                Throw(message, "Няма подадено FishingActivity за корекция на ARRIVAL NOTIFICATION");
            }

            if (string.IsNullOrEmpty(message.FvmsfishingActivityReport.ReferencedResponseUuid))
            {
                Throw(message, "Не е подадено ReferencedID за корекция на ARRIVAL NOTIFICATION");
            }

            (List<FvmsfishingActivityReport> notifications, List<FvmsfishingActivityReport> declarations)
                = GetActiveArrivals(message.FvmsfishingActivityReport.TripIdentifier);

            FvmsfishingActivityReport replacedArrival = notifications
                .Where(x => x.ResponseUuid == message.FvmsfishingActivityReport.ReferencedResponseUuid)
                .FirstOrDefault();

            if (replacedArrival == null)
            {
                Throw(message, $"Не е открито ARRIVAL NOTIFICATION за корекция с ReferencedID: {message.FvmsfishingActivityReport.ReferencedResponseUuid}");
            }

            // добавяме записи в FVMSFishingActivityReportFishes, ако има улов на борда
            AddFvmsFishingActivityReportFishes(message);

            // ако има декларации или редакцията се отнася до Arrival, който не е последният активен, не правим нищо
            if (replacedArrival.IsCurrent)
            {
                replacedArrival.IsCurrent = false;

                if (!declarations.Any(x => x.IsCurrent))
                {
                    // редактираме часа и пристанището на връщане на всички страници, отворени по време на рейса
                    List<ShipLogBookPage> pages = GetOpenedPagesByTripIdentifier(message.FvmsfishingActivityReport.TripIdentifier);

                    int? arrivePortId = GetPortIdFromMdrLocation(message.FvmsfishingActivityReport.MdrLocationId);

                    foreach (ShipLogBookPage page in pages)
                    {
                        page.FishTripEndDateTime = message.OccurrenceDateTime;
                        page.ArrivePortId = arrivePortId;

                        AddFvmsFishingActivityReportLogBookPage(message.FvmsfishingActivityReport, page);
                    }
                }
            }
            else
            {
                message.FvmsfishingActivityReport.IsCurrent = false;
            }

            Db.SaveChanges();
        }

        public void MapNotificationCancel(FaProcessableMessage message)
        {
            if (string.IsNullOrEmpty(message.FvmsfishingActivityReport.ReferencedResponseUuid))
            {
                Throw(message, "Не е подадено ReferencedID за отмяна на ARRIVAL NOTIFICATION");
            }

            (List<FvmsfishingActivityReport> notifications, List<FvmsfishingActivityReport> declarations)
                = GetActiveArrivals(message.FvmsfishingActivityReport.TripIdentifier);

            FvmsfishingActivityReport canceledArrival = notifications
                .Where(x => x.ResponseUuid == message.FvmsfishingActivityReport.ReferencedResponseUuid)
                .FirstOrDefault();

            notifications.Remove(canceledArrival);

            if (canceledArrival == null)
            {
                Throw(message, $"Не е открито ARRIVAL NOTIFICATION за отмяна с ReferencedID: {message.FvmsfishingActivityReport.ReferencedResponseUuid}");
            }
            else
            {
                canceledArrival.IsCanceled = true;
            }

            // последната получена нотификация, ако има такава, става текущата активна
            if (canceledArrival.IsCurrent)
            {
                canceledArrival.IsCurrent = false;

                FvmsfishingActivityReport lastNotification = notifications.LastOrDefault();
                if (lastNotification != null)
                {
                    lastNotification.IsCurrent = true;
                }

                List<ShipLogBookPage> pages = GetOpenedPagesByTripIdentifier(message.FvmsfishingActivityReport.TripIdentifier);
                DateTime? fishTripEndDateTime = null;
                int? arrivePortId = null;

                // ако няма нито активна нотификация, нито активна декларация, изтриваме данните за връщането
                if (!notifications.Any(x => x.IsCurrent) && !declarations.Any(x => x.IsCurrent))
                {
                    fishTripEndDateTime = null;
                    arrivePortId = null;
                }
                // ако има активна декларация, взимаме нея
                else if (declarations.Any(x => x.IsCurrent))
                {
                    FvmsfishingActivityReport lastDeclaration = declarations.First(x => x.IsCurrent);
                    fishTripEndDateTime = lastDeclaration.OccurenceDateTime;
                    arrivePortId = GetPortIdFromMdrLocation(lastDeclaration.MdrLocationId);
                }
                // в противен случай взимаме последната активна нотификация, ако има такава
                else if (lastNotification != null)
                {
                    fishTripEndDateTime = lastNotification.OccurenceDateTime;
                    arrivePortId = GetPortIdFromMdrLocation(lastNotification.MdrLocationId);
                }

                // редактираме часа и пристанището на връщане на всички страници, отворени по време на рейса
                foreach (ShipLogBookPage page in pages)
                {
                    page.FishTripEndDateTime = fishTripEndDateTime;
                    page.ArrivePortId = arrivePortId;

                    AddFvmsFishingActivityReportLogBookPage(message.FvmsfishingActivityReport, page);
                }
            }

            Db.SaveChanges();
        }

        // Declarations
        public void MapDeclarationOriginal(FaProcessableMessage message)
        {
            if (message.FishingActivity == null)
            {
                Throw(message, "Няма подадено FishingActivity за ARRIVAL DECLARATION");
            }

            // редактираме часа и пристанището на връщане на всички страници, отворени по време на рейса
            List<ShipLogBookPage> pages = GetOpenedPagesByTripIdentifier(message.FvmsfishingActivityReport.TripIdentifier);

            Dictionary<int, string> arrivalReasons = GetPageArrivalReasons(pages.Select(x => x.Id).ToList());
            int? arrivePortId = GetPortIdFromMdrLocation(message.FvmsfishingActivityReport.MdrLocationId);

            foreach (ShipLogBookPage page in pages)
            {
                page.FishTripEndDateTime = message.OccurrenceDateTime;
                page.ArrivePortId = arrivePortId;

                if (arrivalReasons.TryGetValue(page.Id, out string arrivalReasonCode))
                {
                    if (arrivalReasonCode != null && arrivalReasonCode != "LAN")
                    {
                        page.PageFillDate = message.OccurrenceDateTime;
                        page.Status = nameof(LogBookPageStatusesEnum.Submitted);
                    }
                }

                AddFvmsFishingActivityReportLogBookPage(message.FvmsfishingActivityReport, page);
            }

            Db.SaveChanges();
        }

        public void MapDeclarationReplace(FaProcessableMessage message)
        {
            if (message.FishingActivity == null)
            {
                Throw(message, "Няма подадено FishingActivity за корекция на ARRIVAL DECLARATION");
            }

            if (string.IsNullOrEmpty(message.FvmsfishingActivityReport.ReferencedResponseUuid))
            {
                Throw(message, "Не е подадено ReferencedID за корекция на ARRIVAL DECLARATION");
            }

            (List<FvmsfishingActivityReport> notifications, List<FvmsfishingActivityReport> declarations)
                = GetActiveArrivals(message.FvmsfishingActivityReport.TripIdentifier);

            FvmsfishingActivityReport replacedArrival = declarations
                .Where(x => x.ResponseUuid == message.FvmsfishingActivityReport.ReferencedResponseUuid)
                .FirstOrDefault();

            if (replacedArrival == null)
            {
                Throw(message, $"Не е открито ARRIVAL DECLARATION за корекция с ReferencedID: {message.FvmsfishingActivityReport.ReferencedResponseUuid}");
            }

            // ако редакцията се отнася до Arrival, който не е последният активен, не правим нищо
            if (replacedArrival.IsCurrent)
            {
                replacedArrival.IsCurrent = false;

                // редактираме часа и пристанището на връщане на всички страници, отворени по време на рейса
                List<ShipLogBookPage> pages = GetOpenedPagesByTripIdentifier(message.FvmsfishingActivityReport.TripIdentifier);

                Dictionary<int, string> arrivalReasons = GetPageArrivalReasons(pages.Select(x => x.Id).ToList());
                int? arrivePortId = GetPortIdFromMdrLocation(message.FvmsfishingActivityReport.MdrLocationId);

                foreach (ShipLogBookPage page in pages)
                {
                    page.FishTripEndDateTime = message.OccurrenceDateTime;
                    page.ArrivePortId = arrivePortId;

                    if (arrivalReasons.TryGetValue(page.Id, out string arrivalReasonCode))
                    {
                        if (arrivalReasonCode != null && arrivalReasonCode != "LAN")
                        {
                            page.PageFillDate = message.OccurrenceDateTime;
                            page.Status = nameof(LogBookPageStatusesEnum.Submitted);
                        }
                    }

                    AddFvmsFishingActivityReportLogBookPage(message.FvmsfishingActivityReport, page);
                }
            }
            else
            {
                message.FvmsfishingActivityReport.IsCurrent = false;
            }

            Db.SaveChanges();
        }

        public void MapDeclarationDelete(FaProcessableMessage message)
        {
            if (string.IsNullOrEmpty(message.FvmsfishingActivityReport.ReferencedResponseUuid))
            {
                Throw(message, "Не е подадено ReferencedID за изтриване на ARRIVAL DECLARATION");
            }

            (List<FvmsfishingActivityReport> notifications, List<FvmsfishingActivityReport> declarations)
                = GetActiveArrivals(message.FvmsfishingActivityReport.TripIdentifier);

            FvmsfishingActivityReport deletedArrival = declarations
                .Where(x => x.ResponseUuid == message.FvmsfishingActivityReport.ReferencedResponseUuid)
                .FirstOrDefault();

            declarations.Remove(deletedArrival);

            if (deletedArrival == null)
            {
                Throw(message, $"Не е открито ARRIVAL DECLARATION за отмяна с ReferencedID: {message.FvmsfishingActivityReport.ReferencedResponseUuid}");
            }
            else
            {
                deletedArrival.IsDeleted = true;
            }

            // последната получена декларация, ако има такава, става текущата активна
            if (deletedArrival.IsCurrent)
            {
                deletedArrival.IsCurrent = false;

                FvmsfishingActivityReport lastDeclaration = declarations.LastOrDefault();
                if (lastDeclaration != null)
                {
                    lastDeclaration.IsCurrent = true;
                }

                List<ShipLogBookPage> pages = GetOpenedPagesByTripIdentifier(message.FvmsfishingActivityReport.TripIdentifier);

                Dictionary<int, string> arrivalReasons = GetPageArrivalReasons(pages.Select(x => x.Id).ToList());
                DateTime? fishTripEndDateTime = null;
                int? arrivePortId = null;

                // ако няма нито активна нотификация, нито активна декларация, изтриваме данните за връщането
                if (!notifications.Any(x => x.IsCurrent) && !declarations.Any(x => x.IsCurrent))
                {
                    fishTripEndDateTime = null;
                    arrivePortId = null;
                }
                // ако има активна декларация, взимаме нея
                else if (lastDeclaration != null)
                {
                    fishTripEndDateTime = lastDeclaration.OccurenceDateTime;
                    arrivePortId = GetPortIdFromMdrLocation(lastDeclaration.MdrLocationId);
                }
                // в противен случай взимаме последната активна нотификация, ако има такава
                else if (notifications.Any(x => x.IsCurrent))
                {
                    FvmsfishingActivityReport lastNotification = notifications.First(x => x.IsCurrent);
                    fishTripEndDateTime = lastNotification.OccurenceDateTime;
                    arrivePortId = GetPortIdFromMdrLocation(lastNotification.MdrLocationId);
                }

                // редактираме часа и пристанището на връщане на всички страници, отворени по време на рейса
                foreach (ShipLogBookPage page in pages)
                {
                    page.FishTripEndDateTime = fishTripEndDateTime;
                    page.ArrivePortId = arrivePortId;

                    if (fishTripEndDateTime != null && arrivePortId != null)
                    {
                        if (arrivalReasons.TryGetValue(page.Id, out string arrivalReasonCode))
                        {
                            if (arrivalReasonCode != null && arrivalReasonCode != "LAN")
                            {
                                page.PageFillDate = message.OccurrenceDateTime;
                                page.Status = nameof(LogBookPageStatusesEnum.Submitted);
                            }
                        }
                    }

                    AddFvmsFishingActivityReportLogBookPage(message.FvmsfishingActivityReport, page);
                }
            }

            Db.SaveChanges();
        }

        private (List<FvmsfishingActivityReport> notifications, List<FvmsfishingActivityReport> declarations) GetActiveArrivals(string tripIdentifier)
        {
            var result = (from report in Db.FvmsfishingActivityReports
                          join faReportType in Db.MdrFluxFaReportTypes on report.MdrFluxFaReportTypeId equals faReportType.Id
                          join faType in Db.MdrFluxFaTypes on report.MdrFluxFaTypeId equals faType.Id
                          join faPurpose in Db.MdrFluxGpPurposes on report.MdrFluxGpPurposeId equals faPurpose.Id
                          where report.Status == nameof(FvmsFishingActivityReportStatus.Processed)
                            && report.TripIdentifier == tripIdentifier
                            && faType.Code == nameof(FaTypes.ARRIVAL)
                            && (faPurpose.Code == ((int)FluxPurposes.Original).ToString()
                                || faPurpose.Code == ((int)FluxPurposes.Replace).ToString())
                            && !report.IsCanceled
                            && !report.IsDeleted
                          orderby report.CreatedOn
                          select new
                          {
                              faReportType.Code,
                              report
                          }).ToList();

            return (result.Where(x => x.Code == nameof(FaReportTypes.NOTIFICATION)).Select(x => x.report).ToList(),
                    result.Where(x => x.Code == nameof(FaReportTypes.DECLARATION)).Select(x => x.report).ToList());
        }
    }
}
