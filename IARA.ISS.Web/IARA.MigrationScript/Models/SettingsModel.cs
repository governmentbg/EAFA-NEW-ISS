namespace IARA.MigrationScript.Models
{
    public class SettingsModel
    {
        public ConnectionStringsModel ConnectionStrings { get; set; }
        public int ThreadCount { get; set; }
        public int TimeToNextThread { get; set; }
        public int ChunkSize { get; set; }
        public int StartId { get; set; }
        public int? EndId { get; set; }
    }

    public class ConnectionStringsModel
    {
        public string Source { get; set; }
        public string Destination { get; set; }
    }
}
