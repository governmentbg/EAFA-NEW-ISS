using System.Threading.Tasks;
using IARA.Common;
using IARA.RegixAbstractions.Interfaces;
using IARA.RegixAbstractions.Models;
using TL.RegiXClient.Base;
using TL.RegiXClient.Extended;
using TL.RegiXClient.Extended.Models.ActualState;
using TLTTS.Common.ConfigModels;

namespace IARA.RegixIntegration.CheckServices
{
    public class ActualStateCheckService : BaseActualStateCheckService
    {
        public ActualStateCheckService(ConnectionStrings connectionString,
                                       IRegiXClientService regixService,
                                       IRegixConclusionsService conclusionsService,
                                       ScopedServiceProviderFactory scopedServiceProviderFactory)
            : base(connectionString,
                   regixService,
                   conclusionsService,
                   scopedServiceProviderFactory)
        { }

        protected override Task<ResponseType<ActualStateResponseType>> HandleDataResponse(IRegiXClientService regixClientService,
                                                                                  RegixContextData<ActualStateRequestType, RegixLegalContext> data)
        {
            return GetResponse(data, async (request) =>
            {
                return await regixClientService.GetActualState(data.Context, GetRequestParameters(data), GetEmployeeInfo(data));
            });
        }


    }
}
