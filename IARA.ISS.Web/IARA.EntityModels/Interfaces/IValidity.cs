using System;

namespace IARA.EntityModels.Interfaces
{
    public interface IValidity
    {
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
    }
}
