using System;

namespace IARA.DomainModels.DTOModels.Mobile.Ships
{
    public class UnloadingDeclarationMobileDTO
    {
        public int Id { get; set; }
        public int ShipId { get; set; }
        public string Number { get; set; }
        public DateTime FilledOn { get; set; }
        public bool IsActive { get; set; }
    }
}
