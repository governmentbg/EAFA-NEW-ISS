using System;
using System.Collections.Generic;
using System.Text;

namespace IARA.DomainModels.DTOModels.CatchQuotas
{
    public class ShipDTO
    {
        public int Id { get; set; }
        public string AssociationName { get; set; }
        public string ShipName { get; set; }
        public string CFR { get; set; }
        public string ExtMarking { get; set; }
    }
}
