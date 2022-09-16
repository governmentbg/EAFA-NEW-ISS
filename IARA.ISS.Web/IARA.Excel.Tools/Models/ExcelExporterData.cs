using System.Collections.Generic;
using System.Linq;

namespace IARA.Excel.Tools.Models
{
    public abstract class ExcelExporterData
    {
        public string PrimaryKey { get; set; }

        public string ForeignKey { get; set; }

        public IDictionary<string, string> HeaderNames { get; set; }

        public IEnumerable<ExcelExporterData> ChildData { get; set; }
    }

    public class ExcelExporterData<TModel> : ExcelExporterData
        where TModel : class
    {
        private IList<TModel> data;

        public IQueryable<TModel> Query { get; set; }

        public IList<TModel> Data
        {
            get
            {
                if (data == null)
                {
                    data = Query.ToList();
                }

                return data;
            }
        }
    }
}
