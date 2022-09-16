using System;

namespace IARA.DomainModels.DTOModels.ShipsRegister
{
    public class ShipRegisterUsedCertificateDTO
    {
        public DateTime Date { get; set; }

        public int Num { get; set; }

        public decimal EnginePower { get; set; }

        public decimal GrossTonnage { get; set; }

        public bool IsActive { get; set; }
    }
}
