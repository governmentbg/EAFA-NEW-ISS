using IARA.DomainModels.DTOModels.Common;

namespace IARA.Interfaces
{
    public interface ICancellationService : IService
    {
        CancellationDetailsDTO GetCancellationDetails(int? id);
    }
}
