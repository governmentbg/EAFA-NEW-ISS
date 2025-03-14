﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.ShipsRegister.ReduceCapacity
{
    public class ShipRegisterReduceCapacityDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? ApplicationId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<ShipRegisterEditDTO> Ships { get; set; }
    }
}
