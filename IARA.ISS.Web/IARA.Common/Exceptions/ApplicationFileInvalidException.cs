using System;
using System.Collections.Generic;
using System.Linq;

namespace IARA.Common.Exceptions
{
    public class ApplicationFileInvalidException : Exception
    {
        public ApplicationFileInvalidException(IEnumerable<string> errors)
        {
            this.Errors = errors.ToList();
        }

        public List<string> Errors { get; set; }
    }
}
