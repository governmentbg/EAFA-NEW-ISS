using System.Collections.Concurrent;
using IARA.Mobile.Application.Interfaces.Factories;
using IARA.Mobile.Infrastructure.Factories;
using Microsoft.Extensions.DependencyInjection;

namespace IARA.Mobile.Infrastructure.Builders
{
    public class ServerUrlFactoryBuilder
    {
        private readonly ConcurrentDictionary<string, string> urls;
        private readonly ConcurrentDictionary<string, string> extensions;
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> environmentUrl;

        public ServerUrlFactoryBuilder()
        {
            urls = new ConcurrentDictionary<string, string>();
            extensions = new ConcurrentDictionary<string, string>();
            environmentUrl = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();
        }

        public void AddUrl(string environment, string url)
        {
            urls[environment] = url;
        }

        public void AddExtension(string environment, string url)
        {
            extensions[environment] = url;
        }

        public void AddExternalUrl(string environment, string name, string url)
        {
            if (!environmentUrl.ContainsKey(environment))
            {
                environmentUrl[environment] = new ConcurrentDictionary<string, string>();
            }

            environmentUrl[environment][name] = url;
        }

        public IServiceCollection Build(IServiceCollection services)
        {
            services.AddTransient<IServerUrlFactory, ServerUrlFactory>((_) => new ServerUrlFactory(urls, extensions, environmentUrl));
            return services;
        }
    }
}
