using IARA.Mobile.Application.DTObjects.Nomenclatures;

namespace IARA.Mobile.Insp.Application.DTObjects.Nomenclatures
{
    public class ShipSelectNomenclatureDto : SelectNomenclatureDto
    {
        public int Uid { get; set; }

        public string ExtMarkings { get; set; }
        public string AssociationName { get; set; }

        public string ShipDisplayValue => $"{Name} ({ExtMarkings}) ({Code})"
            + (string.IsNullOrEmpty(AssociationName) ? string.Empty : $" ({AssociationName})");
    }
}
