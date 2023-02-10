using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.FLUXVMSRequests;
using IARA.DomainModels.Nomenclatures;
using IARA.Flux.Models;
using IARA.Infrastructure.FluxIntegration;
using IARA.Infrastructure.FluxIntegrations.QueueServices;
using IARA.Interfaces.Flux;
using Microsoft.Extensions.DependencyInjection;
using TL.SysToSysSecCom.Interfaces;

namespace IARA.Infrastructure.FluxIntegrations
{
    public class FluxReplayService : BaseService, IFluxReplayService
    {
        private Dictionary<FluxFvmsDomainsEnum, BaseFluxService> fluxQueueServices;
        private ICryptoHelper cryptoHelper;

        public FluxReplayService(IARADbContext dbContext, IServiceProvider service, ICryptoHelper cryptoHelper)
            : base(dbContext)
        {
            this.cryptoHelper = cryptoHelper;
            this.fluxQueueServices = new Dictionary<FluxFvmsDomainsEnum, BaseFluxService>
            {
                { FluxFvmsDomainsEnum.ACDRDomain, service.GetService<ACDRQueueService>() },
                { FluxFvmsDomainsEnum.VesselDomain, service.GetService<FluxVesselQueueService>() },
                { FluxFvmsDomainsEnum.SalesDomain, service.GetService<FluxSalesQueueService>() },
                { FluxFvmsDomainsEnum.FLAPDomain, service.GetService<FluxPermitsQueueService>() },
                { FluxFvmsDomainsEnum.FADomain, service.GetService<FluxFAQueueService>() }
            };
        }

        public Task<bool> ReplayOutgoing(Guid requestGuid, FluxFvmsDomainsEnum serviceType, string messageType = null)
        {
            var dbRequest = this.Db.Fluxfvmsrequests.Where(x => x.RequestUuid == requestGuid
                                                      && x.IsActive
                                                      && x.DomainName == serviceType.ToString()
                                                      && x.RequestContent != null
                                                      && (messageType == null || x.WebServiceName == messageType))
                                             .Select(x => new
                                             {
                                                 x.WebServiceName,
                                                 x.RequestContent
                                             }).First();

            return ReplayOutgoing(serviceType, dbRequest.RequestContent, dbRequest.WebServiceName);
        }

        public Task<bool> ReplayOutgoing(int id, FluxFvmsDomainsEnum serviceType, string messageType = null)
        {
            var dbRequest = this.Db.Fluxfvmsrequests.Where(x => x.Id == id
                                                           && x.IsActive
                                                           && x.DomainName == serviceType.ToString()
                                                           && x.RequestContent != null
                                                           && (messageType == null || x.WebServiceName == messageType))
                                                  .Select(x => new
                                                  {
                                                      x.WebServiceName,
                                                      x.RequestContent
                                                  }).First();

            return ReplayOutgoing(serviceType, dbRequest.RequestContent, dbRequest.WebServiceName);
        }

        private Task<bool> ReplayOutgoing(FluxFvmsDomainsEnum serviceType, string requestContent, string webServiceName)
        {
            var service = fluxQueueServices[serviceType];

            switch (serviceType)
            {
                case FluxFvmsDomainsEnum.VesselDomain:
                    {
                        var request = TL.SysToSysSecCom.CommonUtils.JsonDeserialize<FLUXReportVesselInformationType>(requestContent);
                        return (service as FluxVesselQueueService).ReportVesselChange(request);
                    }
                case FluxFvmsDomainsEnum.FADomain:
                    {
                        var request = TL.SysToSysSecCom.CommonUtils.JsonDeserialize<FLUXFAReportMessageType>(requestContent);
                        return (service as FluxFAQueueService).ReportFishingActivities(request);
                    }
                case FluxFvmsDomainsEnum.SalesDomain:
                    {
                        if (webServiceName == "SubSalesDoc")
                        {
                            var request = TL.SysToSysSecCom.CommonUtils.JsonDeserialize<FLUXSalesReportMessageType>(requestContent);
                            return (service as FluxSalesQueueService).ReportSalesDocument(request);
                        }
                        else
                        {
                            var request = TL.SysToSysSecCom.CommonUtils.JsonDeserialize<FLUXSalesQueryMessageType>(requestContent);
                            return (service as FluxSalesQueueService).CreateFluxSalesQuery(request);
                        }
                    }
                case FluxFvmsDomainsEnum.FLAPDomain:
                    {
                        var requestPayload = TL.SysToSysSecCom.CommonUtils.JsonDeserialize<FluxFLAPRequestQueuePayload>(requestContent);
                        var request = new Tuple<FLUXFLAPRequestMessageType, FluxFlapRequestEditDTO>(requestPayload.Message, requestPayload.Request);
                        return (service as FluxPermitsQueueService).SendFlapRequest(request);
                    }
                case FluxFvmsDomainsEnum.ACDRDomain:
                    {
                        var request = TL.SysToSysSecCom.CommonUtils.JsonDeserialize<FLUXACDRMessageType>(requestContent);
                        return (service as ACDRQueueService).ReportAggregatedCatches(request);
                    }
                default:
                    throw new NotImplementedException();
            }
        }


        public SimpleAuditDTO GetSimpleAudit(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}
