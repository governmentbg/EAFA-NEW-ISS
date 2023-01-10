using IARA.Mobile.Domain.Interfaces;

namespace IARA.Mobile.Application.DTObjects.Nomenclatures
{
    public class SelectNomenclatureDto : ISelectProperty
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public string DisplayValue => $"{Name} ({Code})";

        public override string ToString()
        {
            return DisplayValue;
        }

        public static implicit operator int(SelectNomenclatureDto nom)
        {
            return nom.Id;
        }

        public static implicit operator int?(SelectNomenclatureDto nom)
        {
            return nom?.Id;
        }
    }
}
