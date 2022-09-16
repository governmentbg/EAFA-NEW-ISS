using System.Linq;
using System.Security.Claims;
using System.Threading;
using IARA.DataAccess;
using IARA.DomainModels.RequestModels;
using IARA.Fakes.MockupData;
using IARA.Interfaces;
using Xunit;

namespace IARA.Infrastructure.UnitTests.ServiceTests
{
    public class PoundnetRegisterTests
    {
        private IARADbContext Db;
        private IPoundNetRegisterService service;

        public PoundnetRegisterTests(IARADbContext dbContext, IPoundNetRegisterService service)
        {
            Thread.CurrentPrincipal = new ClaimsPrincipal(new ClaimsIdentity("Bearer", "TEST_USER", "TEST_ROLE"));
            this.Db = dbContext;
            this.service = service;
            this.Db.NpoundNetCategoryTypes.AddRange(PoundnetRegisterData.PoundnetCategoryTypes);
            this.Db.NpoundNetSeasonTypes.AddRange(PoundnetRegisterData.PoundnetSeasonTypes);
            this.Db.PoundNetRegisters.AddRange(PoundnetRegisterData.PoundnetRegister);
            this.Db.SaveChanges();
        }

        [Fact(DisplayName = "Тества извличане на всички даляни без филтри")]
        public void TestGetAllPoundNetsEmptyFilters()
        {
            var filter = new PoundNetRegisterFilters();

            var result = service.GetAll(filter);
            int allRecordsCount = Db.PoundNetRegisters.Count();

            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
            Assert.Equal(7, allRecordsCount);
        }

        [Fact(DisplayName = "Тества извличане на даляни по зададен текст за търсене")]
        public void TestGetAllPoundNetsFreeTextFilter()
        {
            var filter = new PoundNetRegisterFilters
            {
                FreeTextSearch = "Първа"
            };

            var result = service.GetAll(filter);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            filter.FreeTextSearch = "Целогодишен";

            result = service.GetAll(filter);

            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        [Fact(DisplayName = "Тества извличане на даляни по зададени комплексни филтри")]
        public void TestGetAllPoundNetsAdvancedFilters()
        {
            var filter = new PoundNetRegisterFilters();

            var result = service.GetAll(filter);

            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }
    }
}
