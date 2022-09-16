using System.Threading.Tasks;
using IARA.Common;
using IARA.RegixAbstractions.Interfaces;
using IARA.RegixAbstractions.Models;
using TL.RegiXClient.Base;
using TL.RegiXClient.Extended;
using TL.RegiXClient.Extended.Models.PersonalIdentity;
using TLTTS.Common.ConfigModels;

namespace IARA.RegixIntegration.CheckServices
{
    public class PersonIdentityCheckService : BasePersonIdentityCheckService
    {
        public PersonIdentityCheckService(ConnectionStrings connectionString,
                                          IRegiXClientService regixService,
                                          IRegixConclusionsService conclusionsService,
                                          ScopedServiceProviderFactory scopedServiceProviderFactory)
            : base(connectionString,
                   regixService,
                   conclusionsService,
                   scopedServiceProviderFactory)
        { }

        protected override Task<ResponseType<PersonalIdentityInfoResponseType>> HandleDataResponse(IRegiXClientService regixClientService,
                                                                                           RegixContextData<PersonalIdentityInfoRequestType, RegixPersonContext> data)
        {
            return GetResponse(data, async (request) =>
            {
                return await regixClientService.GetPersonalIdentity(data.Context, GetRequestParameters(data), GetEmployeeInfo(data));
            });
        }


    }
}
