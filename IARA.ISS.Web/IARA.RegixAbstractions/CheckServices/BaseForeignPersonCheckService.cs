using System.Threading.Tasks;
using IARA.Common;
using IARA.RegixAbstractions.CheckServices;
using IARA.RegixAbstractions.Interfaces;
using IARA.RegixAbstractions.Models;
using IARA.RegixIntegration.Utils;
using TL.RegiXClient.Base;
using TL.RegiXClient.Extended;
using TL.RegiXClient.Extended.Models.ForeignPersonIdentity;
using TLTTS.Common.ConfigModels;

namespace IARA.RegixIntegration.CheckServices
{
    public abstract class BaseForeignPersonCheckService : BaseRegiXCheckService<ForeignIdentityInfoRequestType, ForeignIdentityInfoResponseType, RegixPersonContext>, IForeignPersonCheckService
    {
        protected BaseForeignPersonCheckService(ConnectionStrings connectionString,
                                                IRegiXClientService regixService,
                                                IRegixConclusionsService regixConclusionsService,
                                                ScopedServiceProviderFactory scopedServiceProviderFactory)
            : base(connectionString,
                   regixService,
                   regixConclusionsService,
                   scopedServiceProviderFactory,
                   nameof(RegiXClientServiceExtensions.ForeignPersonIdentity),
                   Operations.GET_FOREIGN_IDENTITY_V2)
        { }

        protected abstract override Task<ResponseType<ForeignIdentityInfoResponseType>> HandleDataResponse(IRegiXClientService regixClientService, RegixContextData<ForeignIdentityInfoRequestType, RegixPersonContext> data);

        protected override RegixCheckStatus CompareApplicationData(RegixPersonContext response, RegixPersonContext compare, BaseContextData context)
        {
            return RegixCheckUtils.GetPersonFullChecks(response, compare);
        }

        protected override RegixPersonContext MapToLocalData(ForeignIdentityInfoResponseType response, RegixContextData<ForeignIdentityInfoRequestType, RegixPersonContext> request)
        {
            return RegixDataMappers.MapForeignPersonIdentityResponse(response);
        }
    }
}
