using System;

namespace IARA.DomainModels.RequestModels
{
    public class UserManagementFilters : BaseRequestModel
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int? RoleId { get; set; }
        public string Email { get; set; }
        public DateTime? RegisteredDateFrom { get; set; }
        public DateTime? RegisteredDateTo { get; set; }
        public bool? IsRequestedAccess { get; set; }
    }
}
