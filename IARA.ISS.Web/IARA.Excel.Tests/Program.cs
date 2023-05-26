using IARA.Excel.Tools;

namespace IARA.Excel.Tests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string fileName = "excel.xlsx";
            FileStream fileStream = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), fileName), FileMode.Create, FileAccess.ReadWrite, FileShare.None);

            List<Dictionary<string, object>> data;

            data = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object> { { "Id", 34 }, { "Name", "proba" } },
                new Dictionary<string, object> { { "Id", 35 }, { "Name", "proba1" } },
                new Dictionary<string, object> { { "Id", 36 }, { "Name", "proba2" } },
                new Dictionary<string, object> { { "Id", 37 }, { "Name", "proba3" } },
                new Dictionary<string, object> { { "Id", 38 }, { "Name", "proba4" } }
            };

            new ExcelExporter(fileStream, fileName).ExportXlsx(data);

            Console.WriteLine("File name: ");
            Console.WriteLine($"{fileStream.Name}");
        }
    }
}
