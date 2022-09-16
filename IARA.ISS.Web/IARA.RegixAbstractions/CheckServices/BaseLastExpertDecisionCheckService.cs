using System;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.RecreationalFishing;
using IARA.RegixAbstractions.Interfaces;
using IARA.RegixAbstractions.Models;
using TL.RegiXClient.Base;
using TL.RegiXClient.Extended;
using TL.RegiXClient.Extended.Models.NelkEisme;
using TLTTS.Common.ConfigModels;

namespace IARA.RegixIntegration.CheckServices
{
    public abstract class BaseLastExpertDecisionCheckService : BaseRegiXCheckService<GetLastExpertDecisionByIdentifierRequest, ExpertDecisionsResponse, RecreationalFishingTelkDTO>
    {
        protected BaseLastExpertDecisionCheckService(ConnectionStrings connectionString,
                                                     IRegiXClientService regixService,
                                                     IRegixConclusionsService regixConclusionsService,
                                                     ScopedServiceProviderFactory scopedServiceProviderFactory)
            : base(connectionString,
                   regixService,
                   regixConclusionsService,
                   scopedServiceProviderFactory,
                   nameof(RegiXClientServiceExtensions.GetLastExpertDecision),
                   Operations.GET_LAST_EXPERT_DECISION_BY_ID)
        { }

        protected override RegixCheckStatus CompareApplicationData(RecreationalFishingTelkDTO response, RecreationalFishingTelkDTO compare, BaseContextData context)
        {
            if (response != null
                && (response.IsIndefinite.HasValue && response.IsIndefinite.Value
                    || response.ValidTo.HasValue && response.ValidTo.Value > DateTime.Now))
            {
                if (response.Num != compare.Num)
                {
                    return new RegixCheckStatus(RegixCheckStatusesEnum.WARN, ErrorResources.msgTelkWarn);
                }
                else
                {
                    return new RegixCheckStatus(RegixCheckStatusesEnum.NONE);
                }
            }
            else
            {
                return new RegixCheckStatus(RegixCheckStatusesEnum.ERROR, ErrorResources.msgTelkError);
            }
        }

        protected override RecreationalFishingTelkDTO MapToLocalData(ExpertDecisionsResponse response, RegixContextData<GetLastExpertDecisionByIdentifierRequest, RecreationalFishingTelkDTO> request)
        {
            if (response == null || response.ExpertDecisionDocument == null || response.ExpertDecisionDocument.Length == 0)
            {
                return null;
            }
            else
            {
                var document = response.ExpertDecisionDocument.First();

                return new RecreationalFishingTelkDTO
                {
                    IsIndefinite = !document.DurationDisabilityDateSpecified,
                    Num = document.RegistrationNumber.ToString(),
                    ValidTo = document.DurationDisabilityDateSpecified ? document.DurationDisabilityDate : default(DateTime?)
                };
            }
        }

        protected abstract override Task<ResponseType<ExpertDecisionsResponse>> HandleDataResponse(IRegiXClientService regixClientService, RegixContextData<GetLastExpertDecisionByIdentifierRequest, RecreationalFishingTelkDTO> data);
    }
}
