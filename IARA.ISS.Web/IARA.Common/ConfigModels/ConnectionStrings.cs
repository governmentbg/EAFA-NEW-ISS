using Microsoft.Extensions.Configuration;

namespace TLTTS.Common.ConfigModels
{
    public class ConnectionStrings
    {
        public static ConnectionStrings Default { get; private set; }
        public string Connection { get; set; }
        public string Failover { get; set; }


        public static ConnectionStrings ReadSettings(IConfiguration configuration)
        {
            Default = new ConnectionStrings
            {
                Connection = configuration.GetConnectionString("IARADatabase"),
                Failover = configuration.GetConnectionString(nameof(Failover))
            };

            return Default;
        }
    }
}
