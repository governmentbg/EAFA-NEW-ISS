using System.Security.Claims;
using System.Threading;
using IARA.DataAccess;
using IARA.Fakes.MockupData;
using IARA.Interfaces;

namespace IARA.Infrastructure.UnitTests.ServiceTests
{
    public class BuyerTests
    {
        private IARADbContext Db;
        private IBuyersService service;

        public BuyerTests(IARADbContext Db, IBuyersService service)
        {
            Thread.CurrentPrincipal = new ClaimsPrincipal(new ClaimsIdentity("Bearer", "TEST_USER", "TEST_ROLE"));
            this.Db = Db;
            this.service = service;
            this.Db.NaddressTypes.AddRange(AddressData.AddressTypes);
            this.Db.Ndistricts.AddRange(AddressData.Districts);
            this.Db.Nmunicipalities.AddRange(AddressData.Municipalities);
            this.Db.NpopulatedAreas.AddRange(AddressData.PopulatedAreas);
            this.Db.Addresses.AddRange(AddressData.Addresses);
            this.Db.Persons.AddRange(PersonsData.Persons);
            this.Db.Nsectors.AddRange(NSectorsData.Sectors);
            this.Db.NterritoryUnits.AddRange(TerritoryData.TerritoryUnits);
            this.Db.Legals.AddRange(LegalsData.Legals);
            this.Db.SaveChanges();
        }
    }
}
