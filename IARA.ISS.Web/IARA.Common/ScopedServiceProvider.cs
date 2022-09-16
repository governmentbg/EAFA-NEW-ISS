using System;
using Microsoft.Extensions.DependencyInjection;

namespace IARA.Common
{
    internal class ScopedServiceProvider : IScopedServiceProvider, IDisposable
    {
        private IServiceScope serviceScope;
        private bool disposedValue;
        
        public ScopedServiceProvider(IServiceScope serviceScope)
        {
            this.serviceScope = serviceScope;
        }

        public T GetRequiredService<T>()
                 where T : class
        {
            return GetService<T>();
        }

        public T GetService<T>()
                 where T : class
        {
            return serviceScope.ServiceProvider.GetRequiredService<T>();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    serviceScope?.Dispose();
                    serviceScope = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public T GetService<T>(Type t)
                 where T : class
        {
            return serviceScope.ServiceProvider.GetService(t) as T;
        }
    }
}
