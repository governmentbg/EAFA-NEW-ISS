namespace IARA.Mobile.Insp.ViewModels.Models
{
    public class FileResultModel
    {
        public bool IsUploaded { get; set; }
        public string FullPath { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long FileSize { get; set; }
    }
}
