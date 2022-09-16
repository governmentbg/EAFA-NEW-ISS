using System;

namespace IARA.DomainModels.DTOModels.UserManagement
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsActive { get; set; }
        public string UserRoles { get; set; }
        public string MobileDevices { get; set; }
    }
}
