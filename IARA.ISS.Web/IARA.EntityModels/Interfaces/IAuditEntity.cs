using System;

namespace IARA.EntityModels.Interfaces
{
    public interface IAuditEntity
    {
        string CreatedBy { get; set; }
        DateTime CreatedOn { get; set; }
        string UpdatedBy { get; set; }
        DateTime? UpdatedOn { get; set; }
    }
}
