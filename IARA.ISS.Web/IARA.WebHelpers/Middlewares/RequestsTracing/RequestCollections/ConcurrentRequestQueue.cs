using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace IARA.WebMiddlewares.RequestsTracing.RequestCollections
{
    public class ConcurrentRequestQueue<T> : IConcurrentRequestQueue<T>
    {
        private readonly ConcurrentQueue<T> queue;

        public ConcurrentRequestQueue()
        {
            queue = new ConcurrentQueue<T>();
        }

        public int Count => queue.Count;

        public void Clear()
        {
            queue.Clear();
        }

        public void Enqueue(T item)
        {
            queue.Enqueue(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return queue.GetEnumerator();
        }

        public bool TryDequeue(out T item)
        {
            return queue.TryDequeue(out item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
