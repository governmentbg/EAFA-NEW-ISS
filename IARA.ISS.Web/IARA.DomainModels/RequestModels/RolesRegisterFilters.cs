using System;

namespace IARA.DomainModels.RequestModels
{
    public class RolesRegisterFilters : BaseRequestModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int? PermissionId { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }
}
