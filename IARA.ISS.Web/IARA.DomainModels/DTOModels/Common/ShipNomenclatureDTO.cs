using System.Collections.Generic;
using IARA.Common.Enums;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.Common
{
    public class ShipNomenclatureDTO : NomenclatureDTO
    {
        public string Cfr { get; set; }

        public string Name { get; set; }

        public string ExternalMark { get; set; }

        public decimal? TotalLength { get; set; }

        public decimal? GrossTonnage { get; set; }

        public decimal? MainEnginePower { get; set; }

        public ShipNomenclatureFlags? Flags { get; set; }

        public ShipNomenclatureChangeFlags? ChangeFlags { get; set; }

        public List<int> ShipIds { get; set; }

        public Dictionary<int, ShipNomenclatureDTO> EventData { get; set; }
    }
}
