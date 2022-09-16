using System.Linq;
using IARA.DataAccess;
using IARA.EntityModels.Entities;
using IARA.FVMSModels.Common;
using IARA.Infrastructure.FluxIntegrations.Interfaces;

namespace IARA.Infrastructure.FluxIntegrations
{
    public class FluxFvmsRequestsService : BaseService, IFluxFvmsRequestsService
    {
        public FluxFvmsRequestsService(IARADbContext db)
            : base(db)
        { }

        public void AddFluxFvmsRequest(FluxFvmsRequest request)
        {
            Fluxfvmsrequest entry = new()
            {
                DomainName = request.DomainName,
                WebServiceName = request.ServiceName,
                IsOutgoing = request.IsOutgoing.Value,
                RequestUuid = request.RequestUuid.Value,
                RequestDateTime = request.RequestDateTime.Value,
                RequestContent = request.RequestContent
            };

            Db.Fluxfvmsrequests.Add(entry);
            Db.SaveChanges();
        }

        public void AddFluxFvmsResponse(FluxFvmsResponse response)
        {
            Fluxfvmsrequest entry = (from request in Db.Fluxfvmsrequests
                                     where request.RequestUuid == response.RequestUuid.Value
                                     select request).First();

            entry.ResponseUuid = response.ResponseUuid.Value;
            entry.ResponseDateTime = response.ResponseDateTime.Value;
            entry.ResponseStatus = response.ResponseStatus;
            entry.ResponseContent = response.ResponseContent;
            entry.ErrorDescription = response.ErrorDescription;

            Db.SaveChanges();
        }
    }
}
