using System;

namespace IARA.DomainModels.DTOModels.LegalEntities
{
    public class LegalEntityDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Eik { get; set; }

        public DateTime RegistrationDate { get; set; }

        public int ActiveUsersCount { get; set; }
    }
}
