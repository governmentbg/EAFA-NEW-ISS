using System.ComponentModel.DataAnnotations.Schema;

namespace IARA.EntityModels.Interfaces
{
    public abstract class FluxNomenclature : IFluxNomenclature
    {
        [NotMapped]
        public string Name
        {
            get
            {
                return this.Code;
            }
            set
            {
                this.Code = value;
            }
        }

        public abstract int Id { get; set; }
        public abstract string Code { get; set; }
    }
}
