using IARA.Mobile.Domain.Interfaces;
using SQLite;

namespace IARA.Mobile.Insp.Domain.Entities.Inspections
{
    public class Inspector : IEntity
    {
        [PrimaryKey]
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? UnregisteredPersonId { get; set; }
        public bool IsNotRegistered { get; set; }
        public int? InstitutionId { get; set; }
        public int? CitizenshipId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string CardNum { get; set; }

        public string NormalizedName { get; set; }
        public string NormalizedCardNum { get; set; }
    }
}
