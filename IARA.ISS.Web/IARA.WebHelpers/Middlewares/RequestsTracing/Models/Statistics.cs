using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace IARA.WebMiddlewares.RequestsTracing
{
    public sealed class Statistics : IEnumerable<KeyValuePair<string, StatisticValues>>
    {
        private ConcurrentDictionary<string, StatisticValues> aggregatedStatistics;

        public Statistics(string key, DateTime exactTime)
        {
            var list = new List<KeyValuePair<string, StatisticValues>>
            {
                new KeyValuePair<string, StatisticValues>(key, new StatisticValues(exactTime))
            };

            aggregatedStatistics = new ConcurrentDictionary<string, StatisticValues>(list);
        }

        public void AddOrUpdate(string key, DateTime exactTime)
        {
            aggregatedStatistics.AddOrUpdate(key, new StatisticValues(exactTime), (innerKey, innerValue) =>
            {
                innerValue.Aggregate(exactTime);
                return innerValue;
            });
        }

        public void RemoveAll(DateTime olderThan)
        {
            if (aggregatedStatistics.Any())
            {
                foreach (var item in aggregatedStatistics.ToList())
                {
                    if (item.Value.RemoveAll(olderThan) == 0)
                    {
                        aggregatedStatistics.TryRemove(item.Key, out _);
                    }
                }
            }
        }

        public IEnumerator<KeyValuePair<string, StatisticValues>> GetEnumerator()
        {
            return aggregatedStatistics.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
