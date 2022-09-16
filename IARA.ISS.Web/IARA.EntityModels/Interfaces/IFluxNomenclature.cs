using System.ComponentModel.DataAnnotations.Schema;

namespace IARA.EntityModels.Interfaces
{
    public interface IFluxNomenclature : INomenclature, ICodeEntity
    {
        int Id { get; set; }
        string Code { get; set; }

        [NotMapped]
        public new string Name { get { return this.Code; } set { this.Code = value; } }
    }
}
