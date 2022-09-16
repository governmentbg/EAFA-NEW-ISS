using System.IO;
using IARA.DomainModels.RequestModels;
using IARA.Excel.Tools.Models;

namespace IARA.Excel.Tools.Interfaces
{
    public interface IExcelExporterService
    {
        Stream BuildExcelFile<TRequestModel, TModel>(ExcelExporterRequestModel<TRequestModel> request,
                                                     ExcelExporterData<TModel> data,
                                                     ExcelExporterConfiguration config = null)
            where TRequestModel : BaseRequestModel
            where TModel : class;
    }
}
