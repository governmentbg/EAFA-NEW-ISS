using System;
using System.Collections.Generic;

namespace IARA.Common.GridModels
{
    public class BaseGridRequestModel
    {
        public const int MAX_PAGE_SIZE = 500;
        public const int DEFAULT_PAGE_SIZE = 20;

        public int PageNumber { get; set; }


        private int pageSize;
        public int PageSize
        {
            get
            {
                return pageSize;
            }
            set
            {
                pageSize = Math.Min(value, MAX_PAGE_SIZE);

                if (pageSize <= 0)
                {
                    pageSize = DEFAULT_PAGE_SIZE;
                }
            }
        }

        public List<ColumnSorting> SortColumns { get; set; }
    }
}
