using System.Threading.Tasks;
using IARA.Common;
using IARA.RegixAbstractions.Interfaces;
using IARA.RegixAbstractions.Models;
using TL.RegiXClient.Base;
using TL.RegiXClient.Extended;
using TL.RegiXClient.Extended.Models.PersonDataSearch;
using TLTTS.Common.ConfigModels;

namespace IARA.RegixIntegration.CheckServices
{
    public class PersonDataCheckService : BasePersonDataCheckService
    {
        public PersonDataCheckService(ConnectionStrings connectionString,
                                      IRegiXClientService regixService,
                                      IRegixConclusionsService conclusionsService,
                                      ScopedServiceProviderFactory scopedServiceProviderFactory)
            : base(connectionString,
                   regixService,
                   conclusionsService,
                   scopedServiceProviderFactory)
        { }

        protected override Task<ResponseType<PersonDataResponseType>> HandleDataResponse(IRegiXClientService regixClientService, RegixContextData<PersonDataRequestType, RegixPersonContext> data)
        {
            return GetResponse(data, async (request) =>
            {
                return await regixClientService.GetPersonDataSearch(data.Context, GetRequestParameters(data), GetEmployeeInfo(data));
            });
        }


    }
}
