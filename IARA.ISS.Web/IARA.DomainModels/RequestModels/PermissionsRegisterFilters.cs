using System.Collections.Generic;

namespace IARA.DomainModels.RequestModels
{
    public class PermissionsRegisterFilters : BaseRequestModel
    {
        public string Name { get; set; }
        public int? GroupId { get; set; }
        public List<int> TypeIds { get; set; }
        public int? RoleId { get; set; }
    }
}
