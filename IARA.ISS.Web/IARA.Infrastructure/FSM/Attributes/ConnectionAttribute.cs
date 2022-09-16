using System;
using IARA.Infrastructure.FSM.Enums;

namespace IARA.Infrastructure.FSM.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ConnectionAttribute : Attribute
    {
        public TransitionCodesEnum TransitionCode { get; set; }
    }
}
