using System.Collections.Generic;

namespace IARA.Mobile.Domain.Models
{
    public class GridRequest
    {
        public GridRequest()
        {
            SortColumns = new List<ColumnSorting>();
            PageNumber = 1;
            PageSize = 20;
        }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<ColumnSorting> SortColumns { get; set; }
    }

    public class GridRequest<T> : GridRequest
    {
        public GridRequest(T filters)
        {
            Filters = filters;
        }

        public T Filters { get; set; }
    }

    public class ColumnSorting
    {
        public string PropertyName { get; set; }
        public string SortOrder { get; set; }
    }
}
