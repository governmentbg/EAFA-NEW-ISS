using System.Collections.Generic;
using IARA.Common.GridModels;
using IARA.DomainModels.RequestModels;

namespace IARA.Excel.Tools.Models
{
    public class ExcelExporterRequestModel<T> where T : BaseRequestModel
    {
        public string Filename { get; set; }

        public T Filters { get; set; }

        public Dictionary<string, string> HeaderNames { get; set; }

        public List<Dictionary<string, string>> ChildHeaderNames { get; set; }

        public List<ColumnSorting> SortColumns { get; set; }
    }
}
