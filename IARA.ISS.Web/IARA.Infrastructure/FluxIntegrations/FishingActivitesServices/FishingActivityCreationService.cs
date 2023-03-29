using IARA.DomainModels.DTOModels.CatchesAndSales;
using IARA.Flux.Models;
using IARA.FluxModels.Enums;
using TL.Logging.Abstractions.Interfaces;

namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices
{
    internal class FishingActivityCreationService : BaseFishingActivityService
    {
        public FishingActivityCreationService(IARADbContext dbContext, IExtendedLogger logger)
            : base(dbContext, logger, "FishingActivityCreationService.cs")
        { }

        public FLUXFAReportMessageType CreateFluxFAReportMessage(ShipLogBookPageFLUXFieldsDTO data, ILookup<FluxFaMessageData, string> fluxMessageIdentifiers)
        {
            FAReportDocumentType[] faReportDocuments = new FAReportDocumentType[] { };
            ReportPurposeCodes purposeCode;

            if (data.IsCancelled.HasValue && data.IsCancelled.Value)
            {
                purposeCode = ReportPurposeCodes.Cancellation;

                var departureGrouping = fluxMessageIdentifiers.Where(x => x.Key.FaType == FaTypes.DEPARTURE)
                                                              .OrderByDescending(x => x.Key.CreatedOn)
                                                              .First();
                FvmsfishingActivityReport referencedReport = GetDocumentByUUID((Guid)departureGrouping.Key.FluxMessageId);
                FLUXFAReportMessageType referencedMessage = CommonUtils.Deserialize<FLUXFAReportMessageType>(referencedReport.ResponseMessage);
            }
            else if (data.IsActive.HasValue && data.IsActive.Value)
            {
                purposeCode = ReportPurposeCodes.Delete;
            }
            else
            {
                purposeCode = ReportPurposeCodes.Replace;
            }

            FLUXFAReportMessageType report = new FLUXFAReportMessageType
            {
                FLUXReportDocument = CreateFluxReportDocument(purposeCode),
                FAReportDocument = faReportDocuments
            };

            return report;
        }

        private FLUXReportDocumentType CreateFluxReportDocument(ReportPurposeCodes purposeCode)
        {
            DateTime now = DateTime.Now;

            var fluxReportDocument = new FLUXReportDocumentType
            {
                ID = new IDType[] { IDType.GenerateGuid() },
                CreationDateTime = DateTimeType.BuildDateTime(now),
                PurposeCode = CodeType.CreatePurpose(purposeCode),
                OwnerFLUXParty = new FLUXPartyType
                {
                    ID = new IDType[] { IDType.CreateParty(nameof(CountryCodes.BGR)) },
                    Name = new TextType[] { TextType.CreateText("Bulgaria") }
                }
            };

            return fluxReportDocument;
        }
    }
}
