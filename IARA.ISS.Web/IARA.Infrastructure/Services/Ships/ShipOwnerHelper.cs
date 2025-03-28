﻿using IARA.DomainModels.DTOModels.ShipsRegister;

namespace IARA.Infrastructure.Services
{
    internal class ShipOwnerHelper : ShipOwnerDTO, IShipOwnerHelper
    {
        public int? PersonId { get; set; }
        public int? LegalId { get; set; }
    }
}
