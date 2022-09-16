using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using IARA.Common.GridModels;

namespace IARA.WebHelpers.Models
{
    public class GridResultModel<T> : BaseGridResultModel<T>
    {
        private static string[] OrderByMethodPair = new string[]
        {
                             "OrderBy",
                             "OrderByDescending"
        };

        private static string[] ThenByMethodPair = new string[]
        {
                             "ThenBy",
                             "ThenByDescending"
        };

        private IQueryable<T> query;

        public GridResultModel(IQueryable<T> query, BaseGridRequestModel request, bool applyDefaultSorting = true)
        {
            this.query = query;
            this.ApplyDefaultSorting = applyDefaultSorting;
            Process(request);
        }

        public bool ApplyDefaultSorting { get; set; }

        private void Process(BaseGridRequestModel request)
        {
            List<string> propertyNames = typeof(T).GetProperties().Select(x => x.Name).ToList();

            if (request != null)
            {
                this.TotalRecordsCount = query.Count();

                if (request.SortColumns != null && request.SortColumns.Any())
                {
                    var sortingColumn = request.SortColumns.First();
                    int skip = 1;
                    if (PropertyExists(propertyNames, sortingColumn))
                    {
                        this.query = OrderByField(this.query, sortingColumn.PropertyName, OrderByMethodPair, sortingColumn.SortOrder);

                        foreach (var item in request.SortColumns.Skip(skip))
                        {
                            if (PropertyExists(propertyNames, item))
                            {
                                this.query = OrderByField(this.query, sortingColumn.PropertyName, ThenByMethodPair, sortingColumn.SortOrder);
                            }
                        }
                    }
                    else
                    {
                        sortingColumn = request.SortColumns.Skip(skip).Take(1).FirstOrDefault();

                        while (sortingColumn != null && !PropertyExists(propertyNames, sortingColumn))
                        {
                            skip++;
                            sortingColumn = request.SortColumns.Skip(skip).Take(1).FirstOrDefault();
                        }

                        if (sortingColumn != null)
                        {
                            this.query = OrderByField(this.query, sortingColumn.PropertyName, OrderByMethodPair, sortingColumn.SortOrder);

                            foreach (var item in request.SortColumns.Skip(skip))
                            {
                                if (PropertyExists(propertyNames, item))
                                {
                                    this.query = OrderByField(this.query, sortingColumn.PropertyName, ThenByMethodPair, sortingColumn.SortOrder);
                                }
                            }
                        }
                    }
                }
                else if (this.ApplyDefaultSorting)
                {
                    //Default sorting by first property in the class in ascending order
                    this.query = OrderByField(this.query, propertyNames.First(), OrderByMethodPair, ColumnOrderTypes[0]);
                }

                this.Records = query.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
            }
            else
            {
                throw new ArgumentNullException(nameof(request));
            }
        }

        private bool PropertyExists(List<string> propertyNames, ColumnSorting sortingColumn)
        {
            sortingColumn.PropertyName = propertyNames.Where(x => x.ToLowerInvariant() == sortingColumn.PropertyName.ToLowerInvariant()).FirstOrDefault();
            return !string.IsNullOrEmpty(sortingColumn.PropertyName);
        }

        private static IQueryable<T> OrderByField(IQueryable<T> query, string propertyName, string[] methodsPair, string sortOrder = "asc")
        {
            if (!ColumnOrderTypes.Contains(sortOrder))
            {
                return query;
            }

            var param = Expression.Parameter(typeof(T), "p");

            var prop = Expression.Property(param, propertyName);

            var exp = Expression.Lambda(prop, param);

            string method = sortOrder == "asc" ? methodsPair[0] : methodsPair[1];

            Type[] types = new[] { query.ElementType, exp.Body.Type };

            var mce = Expression.Call(typeof(Queryable), method, types, query.Expression, exp);

            return query.Provider.CreateQuery<T>(mce);
        }
    }
}
