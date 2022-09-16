using TechnoLogica.Xamarin.Converters.Base;
using TechnoLogica.Xamarin.Helpers;

namespace IARA.Mobile.Insp.Converters
{
    public class MimeTypeToGlyphConverter : BaseValueConverter<string, string>
    {
        public MimeTypeToGlyphConverter()
        {
            ConvertDefaultReturn = IconFont.File;
        }

        public override string ConvertTo(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return IconFont.File;
            }

            string[] split = value.Split('/');

            switch (split[0])
            {
                case "text":
                    return GetTextIcon(split[1]);
                case "image":
                    return IconFont.Image;
                case "audio":
                    return IconFont.FileAudio;
                case "video":
                    return IconFont.Video;
                case "font":
                    return IconFont.Font;
                case "application":
                    return GetApplicationIcon(split[1]);
                default:
                    return IconFont.File;
            }
        }

        private static string GetApplicationIcon(string mimeType)
        {
            switch (mimeType)
            {
                case "vnd.openxmlformats-officedocument.wordprocessingml.document":
                case "msword":
                    return IconFont.FileWord;
                case "pdf":
                    return IconFont.FilePdf;
                case "vnd.amazon.ebook":
                case "epub+zip":
                    return IconFont.BookOpen;
                case "octet-stream":
                    return IconFont.File; // Binary file
                case "x-cdf":
                    return IconFont.CompactDisc;
                case "vnd.ms-fontobject":
                    return IconFont.Font;
                case "x-bzip":
                case "x-bzip2":
                case "gzip":
                case "vnd.rar":
                case "x-tar":
                case "zip":
                case "x-7z-compressed":
                case "x-zoo":
                case "x-ms-wim":
                case "x-xar":
                case "x-gtar":
                case "x-rar-compressed":
                case "zstd":
                case "x-xz":
                case "x-compress":
                case "x-snappy-framed":
                case "x-lzip":
                case "x-archive":
                case "x-freearc":
                    return IconFont.FileZipper;
                case "json":
                case "ld+json":
                case "x-httpd-php":
                case "xhtml+xml":
                case "xml":
                case "atom+xml":
                case "vnd.mozilla.xul+xml":
                case "xaml+xml":
                    return IconFont.FileCode;
                case "vnd.oasis.opendocument.spreadsheet":
                case "vnd.ms-excel":
                case "vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                    return IconFont.FileExcel;
                case "vnd.oasis.opendocument.presentation":
                case "vnd.ms-powerpoint":
                case "vnd.openxmlformats-officedocument.presentationml.presentation":
                    return IconFont.FilePowerpoint;
                case "vnd.oasis.opendocument.text":
                case "rtf":
                    return IconFont.FileLines;
                case "vnd.visio":
                    return IconFont.ChartArea;
                case "java-archive":
                case "vnd.apple.installer+xml":
                case "x-sh":
                case "x-shockwave-flash":
                    return IconFont.File; // Executable file
                case "pkcs8":
                case "pkcs10":
                case "pkix-cert":
                case "pkix-crl":
                case "pkcs7-mime":
                case "x-x509-ca-cert":
                case "x-x509-user-cert":
                case "x-pkcs7-crl":
                case "x-pem-file":
                case "x-pkcs12":
                case "x-pkcs7-certificates":
                case "x-pkcs7-certreqresp":
                    return IconFont.FileSignature;
                case "x-itunes-ipa":
                case "vnd.android.package-archive":
                    return IconFont.Mobile;
            }

            return IconFont.File;
        }

        private static string GetTextIcon(string mimeType)
        {
            switch (mimeType)
            {
                case "javascript":
                case "html":
                case "x-csh":
                case "css":
                case "xml":
                case "sql":
                case "x-sql":
                    return IconFont.FileCode;
                case "csv":
                    return IconFont.FileCsv;
            }

            return IconFont.FileLines;
        }
    }
}
