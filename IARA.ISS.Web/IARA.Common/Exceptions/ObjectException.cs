using System;

namespace IARA.Common.Exceptions
{
    public class ObjectException : Exception
    {
        public object Object { get; set; }

        public ObjectException(object obj)
        {
            Object = obj;
        }
    }
}
