using System.Collections.Generic;
using IARA.DomainModels.DTOModels.ShipsRegister;

namespace IARA.RegixAbstractions.Models
{
    public class VesselContext
    {
        public ShipRegisterBaseRegixDataDTO VesselData { get; set; }
        public List<ShipOwnerRegixDataDTO> Owners { get; set; }
    }
}
