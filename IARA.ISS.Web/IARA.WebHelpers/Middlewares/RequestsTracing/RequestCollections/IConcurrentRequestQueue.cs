using System.Collections.Generic;

namespace IARA.WebMiddlewares.RequestsTracing
{
    public interface IConcurrentRequestQueue<T> : IReadOnlyCollection<T>
    {
        void Enqueue(T item);

        bool TryDequeue(out T item);

        void Clear();
    }
}
