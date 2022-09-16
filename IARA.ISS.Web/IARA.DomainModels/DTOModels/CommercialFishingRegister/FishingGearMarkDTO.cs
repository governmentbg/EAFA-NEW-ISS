using System;
using System.Collections.Generic;
using System.Text;
using IARA.Common.Enums;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.CommercialFishingRegister
{
    public class FishingGearMarkDTO
    {
        public int? Id { get; set; }

        public string Number { get; set; }

        public bool IsActive { get; set; }

        public int StatusId { get; set; }

        public FishingGearMarkStatusesEnum SelectedStatus { get; set; }
    }
}
