using System.Threading.Tasks;
using IARA.Common;
using IARA.RegixAbstractions.Interfaces;
using IARA.RegixAbstractions.Models;
using TL.RegiXClient.Base;
using TL.RegiXClient.Extended;
using TL.RegiXClient.Extended.Models.PermanentAddressSearch;
using TLTTS.Common.ConfigModels;

namespace IARA.RegixIntegration.CheckServices
{
    public class PermanentAddressCheckService : BasePermanentAddressCheckService
    {
        public PermanentAddressCheckService(ConnectionStrings connectionString,
                                            IRegiXClientService regixService,
                                            IRegixConclusionsService conclusionsService,
                                            ScopedServiceProviderFactory scopedServiceProviderFactory)
            : base(connectionString,
                   regixService,
                   conclusionsService,
                   scopedServiceProviderFactory)
        { }

        protected override Task<ResponseType<PermanentAddressResponseType>> HandleDataResponse(IRegiXClientService regixClientService,
                                                                                 RegixContextData<PermanentAddressRequestType, RegixPersonContext> data)
        {
            return GetResponse(data, async (request) =>
            {
                return await regixClientService.GetPermanentAddress(data.Context, GetRequestParameters(data), GetEmployeeInfo(data));
            });
        }


    }
}
