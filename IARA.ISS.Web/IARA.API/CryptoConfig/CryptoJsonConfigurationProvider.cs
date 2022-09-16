using System.IO;
using Microsoft.Extensions.Configuration.Json;

namespace IARA.Web
{
    public class CryptoJsonConfigurationProvider : JsonStreamConfigurationProvider
    {
        public CryptoJsonConfigurationProvider(CryptoJsonConfigurationSource source)
            : base(new JsonStreamConfigurationSource
            {
                Stream = new FileStream(source.Path, FileMode.Open, FileAccess.Read, FileShare.Read)
            })
        { }

        public override void Load(Stream stream)
        {
            // Let the base class do the heavy lifting.
            if (stream.CanRead && stream.Length > 0)
            {
                base.Load(CryptoUtils.DecryptStream(stream));
            }
        }
    }
}
