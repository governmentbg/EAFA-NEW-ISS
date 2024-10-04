namespace IARA.Infrastructure.FluxIntegrations.Mappings.InspectionsDomain
{
    public class InspectedEntityData
    {
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public int? CountryId { get; set; }

        public int? PopulatedAreaId { get; set; }

        public string PostCode { get; set; }

        public string Street { get; set; }

        public bool IsLegal { get; set; }

        public InspectedPersonTypeEnum? Role { get; set; }

        public static List<InspectedEntityData> GetInspectedEntities(int inspectionId, IARADbContext db)
        {
            List<InspectedEntityData> result = (from inspectedPerson in db.InspectedPersons
                                                join inspectedPersonType in db.NinspectedPersonTypes on inspectedPerson.InspectedPersonTypeId equals inspectedPersonType.Id
                                                join address in db.Addresses on inspectedPerson.AddressId equals address.Id into addressMatchTable
                                                from address in addressMatchTable.DefaultIfEmpty()
                                                join person in db.Persons on inspectedPerson.PersonId equals person.Id into per
                                                from person in per.DefaultIfEmpty()
                                                join legal in db.Legals on inspectedPerson.LegalId equals legal.Id into leg
                                                from legal in leg.DefaultIfEmpty()
                                                join buyer in db.BuyerRegisters on inspectedPerson.BuyerId equals buyer.Id into buy
                                                from buyer in buy.DefaultIfEmpty()
                                                join unregPerson in db.UnregisteredPersons on inspectedPerson.UnregisteredPersonId equals unregPerson.Id into unr
                                                from unregPerson in unr.DefaultIfEmpty()
                                                where inspectedPerson.IsActive
                                                    && inspectedPerson.InspectionId == inspectionId
                                                orderby inspectedPerson.Id descending
                                                select new InspectedEntityData
                                                {
                                                    IsLegal = legal != null,
                                                    FirstName = person != null ? person.FirstName
                                                                        : legal != null ? legal.Name
                                                                             : unregPerson != null ? unregPerson.FirstName
                                                                                 : String.Empty,
                                                    MiddleName = person != null ? person.MiddleName : unregPerson.MiddleName,
                                                    LastName = person != null ? person.LastName : unregPerson.LastName,
                                                    CountryId = address.CountryId,
                                                    PopulatedAreaId = address.PopulatedAreaId,
                                                    PostCode = address.PostCode,
                                                    Street = address.Street,
                                                    Role = Enum.Parse<InspectedPersonTypeEnum>(inspectedPersonType.Code)
                                                }).ToList();

            return result;
        }
    }
}
