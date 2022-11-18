using System;
using System.Collections.Generic;
using System.Linq;

namespace IARA.Common.Exceptions
{
    public class DuplicatedMarksException : Exception
    {
        public DuplicatedMarksException(IEnumerable<string> errors)
        {
            this.Errors = errors.ToList();
        }

        public List<string> Errors { get; set; }
    }
}
