using System;

namespace IARA.DomainModels.DTOModels.Mobile.Ships
{
    public class LogBookMobileDTO
    {
        public int Id { get; set; }
        public int? ShipUid { get; set; }
        public string Number { get; set; }
        public DateTime IssuedOn { get; set; }
        public long StartPage { get; set; }
        public long EndPage { get; set; }
        public bool IsActive { get; set; }
    }
}
