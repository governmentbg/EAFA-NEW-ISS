using System.Threading.Tasks;
using IARA.Common;
using IARA.RegixAbstractions.Interfaces;
using IARA.RegixAbstractions.Models;
using TL.RegiXClient.Base;
using TL.RegiXClient.Extended;
using TL.RegiXClient.Extended.Models.IAMA;
using TLTTS.Common.ConfigModels;

namespace IARA.RegixIntegration.CheckServices
{
    public abstract class BaseVesselCheckService : BaseRegiXCheckService<VesselRequest, ShipsResponse, VesselContext>
    {
        protected BaseVesselCheckService(ConnectionStrings connectionString,
                                         IRegiXClientService regixService,
                                         IRegixConclusionsService regixConclusionsService,
                                         ScopedServiceProviderFactory scopedServiceProviderFactory)
            : base(connectionString,
                   regixService,
                   regixConclusionsService,
                   scopedServiceProviderFactory,
                   nameof(RegiXClientServiceExtensions.SearchVesselByCharacteristics),
                   Operations.VESSEL_REGISTRATION_SEARCH,
                   shouldCheckCache: false,
                   saveMappedResponse: true)
        { }

        protected abstract override RegixCheckStatus CompareApplicationData(VesselContext response, VesselContext compare, BaseContextData context);
        protected abstract override Task<ResponseType<ShipsResponse>> HandleDataResponse(IRegiXClientService regixClientService, RegixContextData<VesselRequest, VesselContext> request);
        protected abstract override VesselContext MapToLocalData(ShipsResponse response, RegixContextData<VesselRequest, VesselContext> requestContext);
    }
}
