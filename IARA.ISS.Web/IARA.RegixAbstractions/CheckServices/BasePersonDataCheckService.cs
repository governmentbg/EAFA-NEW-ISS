using System.Threading.Tasks;
using IARA.Common;
using IARA.RegixAbstractions.Interfaces;
using IARA.RegixAbstractions.Models;
using IARA.RegixIntegration.Utils;
using TL.RegiXClient.Base;
using TL.RegiXClient.Extended;
using TL.RegiXClient.Extended.Models.PersonDataSearch;
using TLTTS.Common.ConfigModels;

namespace IARA.RegixIntegration.CheckServices
{
    public abstract class BasePersonDataCheckService : BaseRegiXCheckService<PersonDataRequestType, PersonDataResponseType, RegixPersonContext>
    {
        protected BasePersonDataCheckService(ConnectionStrings connectionString,
                                             IRegiXClientService regixService,
                                             IRegixConclusionsService regixConclusionsService,
                                             ScopedServiceProviderFactory scopedServiceProviderFactory)
            : base(connectionString,
                   regixService,
                   regixConclusionsService,
                   scopedServiceProviderFactory,
                   nameof(RegiXClientServiceExtensions.GetPersonDataSearch),
                   Operations.PERSON_DATA_SEARCH)
        { }

        protected abstract override Task<ResponseType<PersonDataResponseType>> HandleDataResponse(IRegiXClientService regixClientService, RegixContextData<PersonDataRequestType, RegixPersonContext> data);

        protected override RegixCheckStatus CompareApplicationData(RegixPersonContext response, RegixPersonContext compare, BaseContextData context)
        {
            return RegixCheckUtils.GetPersonChecksStatus(response, compare);
        }

        protected override RegixPersonContext MapToLocalData(PersonDataResponseType response, RegixContextData<PersonDataRequestType, RegixPersonContext> request)
        {
            return RegixDataMappers.MapPersonDataSearchResponse(response);
        }
    }
}
