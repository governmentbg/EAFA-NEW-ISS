using System;
using Microsoft.Extensions.DependencyInjection;

namespace IARA.Common
{
    public class ScopedServiceProviderFactory
    {
        public ScopedServiceProviderFactory(IServiceProvider serviceProvider)
        {
            this.RootServiceProvider = serviceProvider;
        }

        public IScopedServiceProvider GetServiceProvider()
        {
            return new ScopedServiceProvider(RootServiceProvider.CreateScope());
        }

        public IServiceProvider RootServiceProvider { get; }
    }
}
