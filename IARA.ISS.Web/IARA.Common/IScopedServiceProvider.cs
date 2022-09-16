using System;

namespace IARA.Common
{
    public interface IScopedServiceProvider : IDisposable
    {
        T GetService<T>()
        where T : class;

        T GetService<T>(Type t)
        where T : class;

        T GetRequiredService<T>()
                 where T : class;
    }
}
