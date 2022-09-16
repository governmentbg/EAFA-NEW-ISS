using IARA.EntityModels.Entities;

namespace IARA.EntityModels.Interfaces
{
    public interface ICancellableEntity
    {
        int? CancellationDetailsId { get; set; }

        CancellationDetail CancellationDetails { get; set; }
    }
}
