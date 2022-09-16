using System;
using System.Linq;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.CatchesAndSales;
using IARA.EntityModels.Entities;
using IARA.Flux.Models;
using IARA.FluxModels.Enums;
using IARA.Logging.Abstractions.Interfaces;
using TL.SysToSysSecCom;

namespace IARA.Infrastructure.FluxIntegrations.Helpers
{
    internal class FishingActivityCreationHelper
    {
        private IARADbContext Db;
        private IExtendedLogger Logger { get; set; }
        private readonly FishingActivityHelper helper;
        private const string LOGGER_MSG_TYPE = "FLUX FA DOMAIN:";

        public FishingActivityCreationHelper(IARADbContext dbContext, IExtendedLogger logger, FishingActivityHelper helper)
        {
            Db = dbContext;
            Logger = logger;
            this.helper = helper;
        }

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
                FvmsfishingActivityReport referencedReport = helper.GetDocumentByUUID((Guid)departureGrouping.Key.FluxMessageId);
                FLUXFAReportMessageType referencedMessage = CommonUtils.JsonDeserialize<FLUXFAReportMessageType>(referencedReport.ResponseMessage);
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
