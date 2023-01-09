using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class MaxNumberMissingPagesWithoutPermissionExceededException : ArgumentException
    {
        public MaxNumberMissingPagesWithoutPermissionExceededException(long lastUsedPageNumber, long difference, long? currStartPage = null)
        {
            LastUsedPageNumber= lastUsedPageNumber;
            Difference = difference;
            CurrentStartPage= currStartPage;
        }

        public long LastUsedPageNumber { get; private set; }

        public long Difference { get; private set; }

        public long? CurrentStartPage { get; private set; }

    }
}
