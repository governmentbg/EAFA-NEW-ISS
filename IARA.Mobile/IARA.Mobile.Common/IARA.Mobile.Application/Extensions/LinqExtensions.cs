using System.Collections.Generic;

namespace IARA.Mobile.Application.Extensions
{
    public static class LinqExtensions
    {
        /// <summary>
        /// Bypasses a specified number of elements in a sequence and then returns a specified number of contiguous elements
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to return elements from</param>
        /// <param name="skip">The number of elements to skip</param>
        /// <param name="take">The number of elements to return after the specified <paramref name="skip"/> number</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains the elements that occur after the specified index in the input sequence</returns>
        public static IEnumerable<TSource> SkipAndTake<TSource>(this IEnumerable<TSource> source, int skip, int take)
        {
            take = skip + take;
            int index = -1;

            foreach (TSource item in source)
            {
                index++;

                if (index < skip)
                {
                    continue;
                }

                if (index > take)
                {
                    yield break;
                }

                yield return item;
            }
        }
    }
}
