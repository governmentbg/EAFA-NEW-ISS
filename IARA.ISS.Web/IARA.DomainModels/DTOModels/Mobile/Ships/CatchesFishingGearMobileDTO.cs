using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IARA.DomainModels.DTOModels.CommercialFishingRegister;

namespace IARA.DomainModels.DTOModels.Mobile.Ships
{
    public class CatchesFishingGearMobileDTO
    {
        public List<CatchMobileDTO> Cathes { get; set; }
        public FishingGearDTO FishingGear { get; set; }
    }
}
