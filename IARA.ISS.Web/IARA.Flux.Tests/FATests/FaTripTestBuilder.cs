using System.Collections.Generic;
using System.Linq;
using IARA.Flux.Models;
using IARA.Flux.Tests.FATests.Models;

namespace IARA.Flux.Tests.FATests
{
    public class FaTripTestBuilder
    {
        private readonly FaTripBuilder builder;

        public FaTripTestBuilder()
        {
            builder = new FaTripBuilder();
        }

        public FLUXFAReportMessageType BuildDeparture(FaReportModel report, FaDepartureModel model, FLUXFAReportMessageType outerMessage = null)
        {
            FAReportDocumentType faReport = builder.GenerateDeclaration(report);
            FishingActivityType activity = builder.GenerateDeparture(model);

            faReport.SpecifiedFishingActivity = new FishingActivityType[] { activity };
            return BuildReportMessage(faReport, outerMessage);
        }

        public FLUXFAReportMessageType BuildAreaEntry(FaReportModel report, FaAreaEntryModel model, FLUXFAReportMessageType outerMessage = null)
        {
            FAReportDocumentType faReport = builder.GenerateDeclaration(report);
            FishingActivityType activity = builder.GenerateAreaEntry(model);

            faReport.SpecifiedFishingActivity = new FishingActivityType[] { activity };
            return BuildReportMessage(faReport, outerMessage);
        }

        public FLUXFAReportMessageType BuildAreaExit(FaReportModel report, FaAreaExitModel model, FLUXFAReportMessageType outerMessage = null)
        {
            FAReportDocumentType faReport = builder.GenerateDeclaration(report);
            FishingActivityType activity = builder.GenerateAreaExit(model);

            faReport.SpecifiedFishingActivity = new FishingActivityType[] { activity };
            return BuildReportMessage(faReport, outerMessage);
        }

        public FLUXFAReportMessageType BuildFishingOperation(FaReportModel report, FaFishingOperationModel model, FLUXFAReportMessageType outerMessage = null)
        {
            FAReportDocumentType faReport = builder.GenerateDeclaration(report);
            FishingActivityType activity = builder.GenerateFishingOperation(model);

            faReport.SpecifiedFishingActivity = new FishingActivityType[] { activity };
            return BuildReportMessage(faReport, outerMessage);
        }

        public FishingActivityType BuildSubFishingOperation(FaSubFishingOperationModel model)
        {
            FishingActivityType activity = builder.GenerateSubFishingOperation(model);
            return activity;
        }

        public FLUXFAReportMessageType BuildJointFishingOperation(FaReportModel report, FaJointFishingOperationModel model, FLUXFAReportMessageType outerMessage = null)
        {
            FAReportDocumentType faReport = builder.GenerateDeclaration(report);
            FishingActivityType activity = builder.GenerateJointFishingOperation(model);

            faReport.SpecifiedFishingActivity = new FishingActivityType[] { activity };
            return BuildReportMessage(faReport, outerMessage);
        }

        public FishingActivityType BuildSubJointFishingOperation(FaSubJointFishingOperationModel model)
        {
            FishingActivityType activity = builder.GenerateSubJointFishingOperation(model);
            return activity;
        }

        public FLUXFAReportMessageType BuildDiscard(FaReportModel report, FaDiscardModel model, FLUXFAReportMessageType outerMessage = null)
        {
            FAReportDocumentType faReport = builder.GenerateDeclaration(report);
            FishingActivityType activity = builder.GenerateDiscard(model);

            faReport.SpecifiedFishingActivity = new FishingActivityType[] { activity };
            return BuildReportMessage(faReport, outerMessage);
        }

        public FLUXFAReportMessageType BuildRelocationNotification(FaReportModel report, FaRelocationNotificationModel model, FLUXFAReportMessageType outerMessage = null)
        {
            FAReportDocumentType faReport = builder.GenerateNotification(report);
            FishingActivityType activity = builder.GenerateRelocationNotification(model);

            faReport.SpecifiedFishingActivity = new FishingActivityType[] { activity };
            return BuildReportMessage(faReport, outerMessage);
        }

        public FLUXFAReportMessageType BuildRelocationDeclaration(FaReportModel report, FaRelocationModel model, FLUXFAReportMessageType outerMessage = null)
        {
            FAReportDocumentType faReport = builder.GenerateDeclaration(report);
            FishingActivityType activity = builder.GenerateRelocationDeclaration(model);

            faReport.SpecifiedFishingActivity = new FishingActivityType[] { activity };
            return BuildReportMessage(faReport, outerMessage);
        }

        public FLUXFAReportMessageType BuildTranshipmentNotification(FaReportModel report, FaTranshipmentNotificationModel model, FLUXFAReportMessageType outerMessage = null)
        {
            FAReportDocumentType faReport = builder.GenerateNotification(report);
            FishingActivityType activity = builder.GenerateTranshipmentNotification(model);

            faReport.SpecifiedFishingActivity = new FishingActivityType[] { activity };
            return BuildReportMessage(faReport, outerMessage);
        }

        public FLUXFAReportMessageType BuildTranshipmentDeclaration(FaReportModel report, FaTranshipmentModel model, FLUXFAReportMessageType outerMessage = null)
        {
            FAReportDocumentType faReport = builder.GenerateDeclaration(report);
            FishingActivityType activity = builder.GenerateTranshipmentDeclaration(model);

            faReport.SpecifiedFishingActivity = new FishingActivityType[] { activity };
            return BuildReportMessage(faReport, outerMessage);
        }

        public FLUXFAReportMessageType BuildArrivalNotification(FaReportModel report, FaArrivalNotificationModel model, FLUXFAReportMessageType outerMessage = null)
        {
            FAReportDocumentType faReport = builder.GenerateNotification(report);
            FishingActivityType activity = builder.GenerateArrivalNotification(model);

            faReport.SpecifiedFishingActivity = new FishingActivityType[] { activity };
            return BuildReportMessage(faReport, outerMessage);
        }

        public FLUXFAReportMessageType BuildArrivalDeclaration(FaReportModel report, FaArrivalDeclarationModel model, FLUXFAReportMessageType outerMessage = null)
        {
            FAReportDocumentType faReport = builder.GenerateDeclaration(report);
            FishingActivityType activity = builder.GenerateArrivalDeclaration(model);

            faReport.SpecifiedFishingActivity = new FishingActivityType[] { activity };
            return BuildReportMessage(faReport, outerMessage);
        }

        public FLUXFAReportMessageType BuildLanding(FaReportModel report, FaLandingModel model, FLUXFAReportMessageType outerMessage = null)
        {
            FAReportDocumentType faReport = builder.GenerateDeclaration(report);
            FishingActivityType activity = builder.GenerateLanding(model);

            faReport.SpecifiedFishingActivity = new FishingActivityType[] { activity };
            return BuildReportMessage(faReport, outerMessage);
        }

        private FLUXFAReportMessageType BuildReportMessage(FAReportDocumentType report, FLUXFAReportMessageType outerMessage = null)
        {
            if (outerMessage != null)
            {
                List<FAReportDocumentType> reports = new();

                if (outerMessage.FAReportDocument != null && outerMessage.FAReportDocument.Any())
                {
                    reports.AddRange(outerMessage.FAReportDocument);
                }

                reports.Add(report);

                outerMessage.FAReportDocument = reports.ToArray();
                return outerMessage;
            }
            else
            {
                FLUXFAReportMessageType result = builder.GenerateReport();
                result.FAReportDocument = new FAReportDocumentType[] { report };
                return result;
            }
        }
    }
}
