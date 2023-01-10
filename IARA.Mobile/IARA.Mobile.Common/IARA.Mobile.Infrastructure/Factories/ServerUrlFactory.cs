using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using IARA.Mobile.Application.Interfaces.Factories;

namespace IARA.Mobile.Infrastructure.Factories
{
    public class ServerUrlFactory : IServerUrlFactory
    {
        private readonly IReadOnlyDictionary<string, string> _urls;
        private readonly IReadOnlyDictionary<string, string> _extensions;
        private readonly IReadOnlyDictionary<string, ConcurrentDictionary<string, string>> _environmentUrl;

        public ServerUrlFactory(IReadOnlyDictionary<string, string> urls, IReadOnlyDictionary<string, string> extensions, IReadOnlyDictionary<string, ConcurrentDictionary<string, string>> environmentUrl)
        {
            _urls = urls ?? throw new ArgumentNullException(nameof(urls));
            _extensions = extensions ?? throw new ArgumentNullException(nameof(extensions));
            _environmentUrl = environmentUrl ?? throw new ArgumentNullException(nameof(environmentUrl));
        }

        public string GetUrl(string environment)
        {
            return _urls[environment];
        }

        public string GetExtension(string environment)
        {
            return _extensions[environment];
        }

        public string GetEnvironmentUrl(string environment, string name)
        {
            return _environmentUrl[environment][name];
        }
    }
}
