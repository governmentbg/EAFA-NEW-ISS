using System.Collections.Generic;
using System.Threading.Tasks;
using IARA.Common;
using IARA.DomainModels.DTOModels.Common;
using IARA.RegixAbstractions.Interfaces;
using IARA.RegixAbstractions.Models;
using IARA.RegixIntegration.Utils;
using TL.RegiXClient.Base;
using TL.RegiXClient.Extended;
using TL.RegiXClient.Extended.Models.PermanentAddressSearch;
using TLTTS.Common.ConfigModels;

namespace IARA.RegixIntegration.CheckServices
{
    public abstract class BasePermanentAddressCheckService : BaseRegiXCheckService<PermanentAddressRequestType, PermanentAddressResponseType, RegixPersonContext>
    {
        protected BasePermanentAddressCheckService(ConnectionStrings connectionString,
            IRegiXClientService regixService,
            IRegixConclusionsService regixConclusionsService,
            ScopedServiceProviderFactory scopedServiceProviderFactory)
            : base(connectionString,
                   regixService,
                   regixConclusionsService,
                   scopedServiceProviderFactory,
                   nameof(RegiXClientServiceExtensions.GetPermanentAddress),
                   Operations.PERMANENT_ADDRESS_SEARCH)
        { }

        protected abstract override Task<ResponseType<PermanentAddressResponseType>> HandleDataResponse(IRegiXClientService regixClientService, RegixContextData<PermanentAddressRequestType, RegixPersonContext> data);

        protected override RegixCheckStatus CompareApplicationData(RegixPersonContext response, RegixPersonContext compare, BaseContextData context)
        {
            return RegixCheckUtils.GetPermanentAddressChecksStatus(response, compare);
        }

        protected override RegixPersonContext MapToLocalData(PermanentAddressResponseType response, RegixContextData<PermanentAddressRequestType, RegixPersonContext> request)
        {
            return new RegixPersonContext
            {
                Addresses = new List<AddressRegistrationDTO>
                {
                    RegixDataMappers.MapPermanentAddressResponse(response),
                }
            };
        }
    }
}
