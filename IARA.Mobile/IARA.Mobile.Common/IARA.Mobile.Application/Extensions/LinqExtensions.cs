using System;
using System.Collections.ObjectModel;

namespace IARA.Mobile.Application.Extensions
{
    public static class LinqExtensions
    {
        public static int RemoveWhere<T>(this Collection<T> collection, Func<T, bool> predicate)
        {
            int count = 0;

            for (int i = 0; i < collection.Count; i++)
            {
                if (predicate(collection[i]))
                {
                    collection.RemoveAt(i);
                    count++;
                    i--;
                }
            }

            return count;
        }
    }
}
