using System;

namespace IARA.DomainModels.DTOModels.UserManagement
{
    public class MobileDeviceDTO
    {
        public int Id { get; set; }
        public string IMEI { get; set; }
        public string Description { get; set; }
        public string AccessStatus { get; set; }
        public DateTime RequestAccessDate { get; set; }
    }
}
