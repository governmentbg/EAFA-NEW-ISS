using System;
using IARA.Web;

[assembly: AssemblyKey()]

namespace IARA.Web
{

    [AttributeUsage(AttributeTargets.Assembly)]
    internal class AssemblyKeyAttribute : Attribute
    {
        public Guid Key { get; private init; }
        public AssemblyKeyAttribute()
        {
            Key = Guid.NewGuid();
        }

    }
}
