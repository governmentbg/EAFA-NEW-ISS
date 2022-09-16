using System;

namespace IARA.DomainModels.DTOModels.UserManagement
{
    public class PublicMobileDeviceDTO
    {
        public string Imei { get; set; }
        public DateTime LastLoginDate { get; set; }
        public string DeviceType { get; set; }
        public string Osversion { get; set; }
        public string DeviceModel { get; set; }
        public string AppVersion { get; set; }
        public string FirebaseTokenKey { get; set; }
    }
}
