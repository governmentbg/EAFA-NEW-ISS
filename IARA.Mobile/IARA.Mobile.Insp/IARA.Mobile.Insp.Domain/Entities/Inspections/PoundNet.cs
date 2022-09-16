using IARA.Mobile.Domain.Interfaces;
using SQLite;

namespace IARA.Mobile.Insp.Domain.Entities.Inspections
{
    public class PoundNet : IEntity
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }

        public string NormalizedName { get; set; }
    }
}
