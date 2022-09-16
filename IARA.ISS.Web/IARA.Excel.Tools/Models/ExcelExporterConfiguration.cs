namespace IARA.Excel.Tools.Models
{
    public class ExcelExporterConfiguration
    {
        public bool AddHeaders { get; set; } = true;

        public int MaxColumnWidth { get; set; } = 100;

        public bool PaginateQueries { get; set; } = false;

        public int QueryDataChunk { get; set; } = 10000;
    }
}
