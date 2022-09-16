namespace IARA.DomainModels.DTOModels.Files
{
    public class DownloadableFileDTO
    {
        public byte[] Bytes { get; set; }
        public string MimeType { get; set; }
        public string FileName { get; set; }
    }
}
