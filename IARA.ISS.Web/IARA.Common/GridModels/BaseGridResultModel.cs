using System.Collections.Generic;

namespace IARA.Common.GridModels
{
    public class BaseGridResultModel<T>
    {
        protected static string[] ColumnOrderTypes = new string[]
        {
              "asc",
              "desc"
        };

        public List<T> Records { get; set; }
        public long TotalRecordsCount { get; set; }
    }
}
