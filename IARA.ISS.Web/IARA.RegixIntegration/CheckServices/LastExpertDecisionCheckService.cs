using System.Threading.Tasks;
using IARA.Common;
using IARA.DomainModels.DTOModels.RecreationalFishing;
using IARA.RegixAbstractions.Interfaces;
using IARA.RegixAbstractions.Models;
using TL.RegiXClient.Base;
using TL.RegiXClient.Extended;
using TL.RegiXClient.Extended.Models.NelkEisme;
using TLTTS.Common.ConfigModels;

namespace IARA.RegixIntegration.CheckServices
{
    public class LastExpertDecisionCheckService : BaseLastExpertDecisionCheckService
    {
        public LastExpertDecisionCheckService(ConnectionStrings connectionString,
                                              IRegiXClientService regixService,
                                              IRegixConclusionsService conclusionsService,
                                              ScopedServiceProviderFactory scopedServiceProviderFactory)
            : base(connectionString,
                   regixService,
                   conclusionsService,
                   scopedServiceProviderFactory)
        { }


        protected override Task<ResponseType<ExpertDecisionsResponse>> HandleDataResponse(IRegiXClientService regixClientService, RegixContextData<GetLastExpertDecisionByIdentifierRequest, RecreationalFishingTelkDTO> data)
        {
            return GetResponse(data, async (request) =>
              {
                  return await regixClientService.GetLastExpertDecision(request, GetRequestParameters(data), GetEmployeeInfo(data));
              });
        }
    }
}
