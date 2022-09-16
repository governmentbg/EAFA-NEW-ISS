using IARA.Excel.Tools.Interfaces;
using IARA.Excel.Tools.Services;
using Microsoft.Extensions.DependencyInjection;

namespace IARA.Excel.Tools
{
    public static class ExcelExporterInitilizer
    {
        public static void AddExcelExporter(this IServiceCollection services)
        {
            services.AddScoped<IExcelExporterService, ExcelExporterService>();
        }
    }
}
