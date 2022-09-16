using IARA.Mobile.Application.Interfaces.Utilities;

namespace IARA.Mobile.Shared.Utilities
{
    public class IdentityServerConfiguration : IIdentityServerConfiguration
    {
        public IdentityServerConfiguration(string client, string callback)
        {
            Client = client;
            Callback = callback;
        }

        public string Client { get; set; }

        public string Callback { get; set; }
    }
}
