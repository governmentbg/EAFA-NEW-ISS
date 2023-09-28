using IARA.Infrastructure.FluxIntegrations.Interfaces;

namespace IARA.Infrastructure.FluxIntegrations
{
    public class InspectionsDomainService : BaseService, IInspectionsDomainService
    {
        public InspectionsDomainService(IARADbContext dbContext)
            : base(dbContext)
        { }
    }
}
