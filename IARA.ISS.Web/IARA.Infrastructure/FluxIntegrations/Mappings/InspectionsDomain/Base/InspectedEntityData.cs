namespace IARA.Infrastructure.FluxIntegrations.Mappings.InspectionsDomain.Base
{
    public class InspectedEntityData
    {
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public int? CountryId { get; set; }

        public int? PopulatedAreaId { get; set; }

        public string CountryCode { get; set; }

        public string PopulatedAreaCode { get; set; }

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
                                                join country in db.Ncountries on address.CountryId equals country.Id into c
                                                from country in c.DefaultIfEmpty()
                                                join populatedArea in db.NpopulatedAreas on address.PopulatedAreaId equals populatedArea.Id into pa
                                                from populatedArea in pa.DefaultIfEmpty()
                                                join person in db.Persons on inspectedPerson.PersonId equals person.Id into per
                                                from person in per.DefaultIfEmpty()
                                                join legal in db.Legals on inspectedPerson.LegalId equals legal.Id into leg
                                                from legal in leg.DefaultIfEmpty()
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
                                                                                 : string.Empty,
                                                    MiddleName = person != null ? person.MiddleName : unregPerson.MiddleName,
                                                    LastName = person != null ? person.LastName : unregPerson.LastName,
                                                    CountryCode = country.Code,
                                                    PopulatedAreaCode = populatedArea.Code,
                                                    PostCode = address.PostCode,
                                                    Street = address.Street,
                                                    Role = Enum.Parse<InspectedPersonTypeEnum>(inspectedPersonType.Code)
                                                }).ToList();

            return result;
        }
    }
}
