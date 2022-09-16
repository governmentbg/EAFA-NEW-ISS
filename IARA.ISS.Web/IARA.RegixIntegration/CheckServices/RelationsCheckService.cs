using System.Collections.Generic;
using System.Threading.Tasks;
using IARA.Common;
using IARA.DomainModels.DTOModels.Common;
using IARA.RegixAbstractions.Interfaces;
using IARA.RegixAbstractions.Models;
using TL.RegiXClient.Base;
using TL.RegiXClient.Extended;
using TL.RegiXClient.Extended.Models.RelationsSearch;
using TLTTS.Common.ConfigModels;

namespace IARA.RegixIntegration.CheckServices
{
    public class RelationsCheckService : BaseRelationsCheckService
    {
        public RelationsCheckService(ConnectionStrings connectionString,
                                     IRegiXClientService regixService,
                                     IRegixConclusionsService conclusionsService,
                                     ScopedServiceProviderFactory scopedServiceProviderFactory)
            : base(connectionString,
                   regixService,
                   conclusionsService,
                   scopedServiceProviderFactory)
        { }

        protected override Task<ResponseType<RelationsResponseType>> HandleDataResponse(IRegiXClientService regixClientService, RegixContextData<RelationsRequestType, List<RegixPersonDataDTO>> data)
        {
            return GetResponse(data, async (request) =>
            {
                return await regixClientService.RelationsSearch(data.Context, GetRequestParameters(data), GetEmployeeInfo(data));
            });
        }


    }
}
