using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.UserManagement
{
    public class InternalUserDTO : UserEditDTO
    {
        public List<MobileDeviceDTO> MobileDevices { get; set; }
    }
}
