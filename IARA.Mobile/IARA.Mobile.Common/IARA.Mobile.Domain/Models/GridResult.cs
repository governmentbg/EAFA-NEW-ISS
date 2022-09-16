using System.Collections.Generic;

namespace IARA.Mobile.Domain.Models
{
    public class GridResult<TDto>
    {
        public List<TDto> Records { get; set; }
        public int TotalRecordsCount { get; set; }
    }
}
