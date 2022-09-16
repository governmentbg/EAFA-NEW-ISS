using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace IARA.WebMiddlewares.RequestsTracing.RequestCollections
{
    public class ConcurrentUniqueQueue<T> : IConcurrentRequestQueue<T>
    {
        private readonly ConcurrentDictionary<int, T> uniqueQueue;

        public ConcurrentUniqueQueue()
        {
            uniqueQueue = new ConcurrentDictionary<int, T>();
        }

        public int Count => uniqueQueue.Count;

        public void Clear()
        {
            uniqueQueue.Clear();
        }

        public void Enqueue(T item)
        {
            uniqueQueue.AddOrUpdate(item.GetHashCode(), item, (key, value) =>
            {
                return item;
            });
        }

        public IEnumerator<T> GetEnumerator()
        {
            return uniqueQueue.Select(x => x.Value).GetEnumerator();
        }

        public bool TryDequeue(out T item)
        {
            var element = uniqueQueue.First();
            return uniqueQueue.TryRemove(element.Key, out item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
