namespace IARA.MigrationScript.Models
{
    public class ExceptionModel
    {
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string Source { get; set; }
        public string ExceptionTypeName { get; set; }
        public ChunkModel Chunk { get; set; }
        public long CurrentFileId { get; set; }
    }
}
