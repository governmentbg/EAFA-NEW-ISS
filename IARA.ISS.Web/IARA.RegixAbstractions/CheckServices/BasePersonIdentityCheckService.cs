using System.Threading.Tasks;
using IARA.Common;
using IARA.RegixAbstractions.Interfaces;
using IARA.RegixAbstractions.Models;
using IARA.RegixIntegration.Utils;
using TL.RegiXClient.Base;
using TL.RegiXClient.Extended;
using TL.RegiXClient.Extended.Models.PersonalIdentity;
using TLTTS.Common.ConfigModels;

namespace IARA.RegixIntegration.CheckServices
{
    public abstract class BasePersonIdentityCheckService : BaseRegiXCheckService<PersonalIdentityInfoRequestType, PersonalIdentityInfoResponseType, RegixPersonContext>
    {
        protected BasePersonIdentityCheckService(ConnectionStrings connectionString,
                                                 IRegiXClientService regixService,
                                                 IRegixConclusionsService regixConclusionsService,
                                                 ScopedServiceProviderFactory scopedServiceProviderFactory)
            : base(connectionString,
                   regixService,
                   regixConclusionsService,
                   scopedServiceProviderFactory,
                   nameof(RegiXClientServiceExtensions.GetPersonalIdentity),
                   Operations.GET_PERSONAL_IDENTITY)
        { }

        protected abstract override Task<ResponseType<PersonalIdentityInfoResponseType>> HandleDataResponse(IRegiXClientService regixClientService, RegixContextData<PersonalIdentityInfoRequestType, RegixPersonContext> data);

        protected override RegixCheckStatus CompareApplicationData(RegixPersonContext response, RegixPersonContext compare, BaseContextData context)
        {
            return RegixCheckUtils.GetPersonFullChecks(response, compare);
        }

        protected override RegixPersonContext MapToLocalData(PersonalIdentityInfoResponseType response, RegixContextData<PersonalIdentityInfoRequestType, RegixPersonContext> request)
        {
            return RegixDataMappers.MapPersonIdentityInfoResponse(response);
        }
    }
}
