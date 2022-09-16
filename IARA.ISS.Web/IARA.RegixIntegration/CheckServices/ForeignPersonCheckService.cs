using System.Threading.Tasks;
using IARA.Common;
using IARA.RegixAbstractions.Interfaces;
using IARA.RegixAbstractions.Models;
using IARA.RegixIntegration.Utils;
using TL.RegiXClient.Base;
using TL.RegiXClient.Extended;
using TL.RegiXClient.Extended.Models.ForeignPersonIdentity;
using TLTTS.Common.ConfigModels;

namespace IARA.RegixIntegration.CheckServices
{
    public class ForeignPersonCheckService : BaseForeignPersonCheckService
    {
        public ForeignPersonCheckService(ConnectionStrings connectionString,
                                         IRegiXClientService regixService,
                                         IRegixConclusionsService conclusionsService,
                                         ScopedServiceProviderFactory scopedServiceProviderFactory)
            : base(connectionString,
                   regixService,
                   conclusionsService,
                   scopedServiceProviderFactory)
        { }

        protected override Task<ResponseType<ForeignIdentityInfoResponseType>> HandleDataResponse(IRegiXClientService regixClientService,
                                                                                          RegixContextData<ForeignIdentityInfoRequestType, RegixPersonContext> data)
        {
            return GetResponse(data, async (request) =>
            {
                return await regixClientService.ForeignPersonIdentity(data.Context, GetRequestParameters(data), GetEmployeeInfo(data));
            });
        }


    }
}
