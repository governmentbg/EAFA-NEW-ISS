using IARA.Common.GridModels;
using IARA.DomainModels.RequestModels;

namespace IARA.WebHelpers.Models
{
    public class GridRequestModel<T> : BaseGridRequestModel
        where T : BaseRequestModel
    {
        public T Filters { get; set; }
    }
}
