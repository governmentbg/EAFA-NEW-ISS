using IARA.Mobile.Domain.Interfaces;
using SQLite;

namespace IARA.Mobile.Insp.Domain.Entities.Inspections
{
    public class Aquaculture : IEntity
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string UrorNum { get; set; }
        public string Name { get; set; }
        public int LegalId { get; set; }

        public string NormalizedName { get; set; }
    }
}
