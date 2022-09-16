using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.LegalEntities;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Fakes.MockupData;
using IARA.Interfaces;
using IARA.Interfaces.Legals;
using Newtonsoft.Json;
using Xunit;

namespace IARA.Infrastructure.UnitTests.ServiceTests
{
    public class LegalTests
    {
        private IARADbContext db;
        private readonly ILegalEntitiesService service;

        public LegalTests(IARADbContext db, ILegalEntitiesService service)
        {
            Thread.CurrentPrincipal = new ClaimsPrincipal(new ClaimsIdentity("Bearer", "TEST_USER", "TEST_ROLE"));
            this.db = db;
            this.service = service;

            this.db.Persons.AddRange(PersonsData.Persons);
            this.db.Legals.AddRange(LegalsData.Legals);
            this.db.UserLegals.AddRange(UserManagementData.UserLegals);

            this.db.SaveChanges();
        }

        [Fact(DisplayName = "Извличане на всички юридически лица без филтри")]
        public void TestGetAllLegalsEmptyFilters()
        {
            LegalEntitiesFilters filters = new LegalEntitiesFilters();
            IQueryable<LegalEntityDTO> result = service.GetAllLegalEntities(filters);

            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        [Fact(DisplayName = "Извличане на юридически лица по зададен текст за търсене")]
        public void TestGetAllLegalsFreeTextFilter()
        {
            LegalEntitiesFilters filters = new LegalEntitiesFilters { FreeTextSearch = "1111", ShowInactiveRecords = false };

            IQueryable<LegalEntityDTO> legals = service.GetAllLegalEntities(filters);

            Assert.NotNull(legals);
            Assert.Equal(1, legals.Count());
        }

        [Fact(DisplayName = "Извличане на юридически лица по зададени комплексни филтри")]
        public void TestGetParametersFilteredLegals()
        {
            LegalEntitiesFilters filters = new LegalEntitiesFilters { Eik = "2222", LegalName = "Юридическо лице 2" };
            IQueryable<LegalEntityDTO> legals = service.GetAllLegalEntities(filters);

            Assert.NotNull(legals);
            Assert.Equal(1, legals.Count());
        }

        [Fact(DisplayName = "Извличане на юридическо лице по id")]
        public void TestGetLegalEntity()
        {
            LegalEntityEditDTO legalEntity = service.GetLegalEntity(1);
            Assert.NotNull(legalEntity);
        }

        [Fact(DisplayName = "Редактиране на юридическо лице")]
        public void TestEditLegalEntity()
        {
            LegalEntityEditDTO legalEntity = new()
            {
                Id = LegalsData.Legals[2].Id,
                //Eik = "245892А3",
                //LegalName = "Сахара ЕООД",
                AuthorizedPeople = new List<AuthorizedPersonDTO>(),
                Files = new List<FileInfoDTO>()
            };

            service.EditLegalEntity(legalEntity);

            var expected = JsonConvert.SerializeObject(legalEntity);
            var actual = JsonConvert.SerializeObject(service.GetLegalEntity(3));

            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }

        [Fact(DisplayName = "Добавяне на юридическо лице")]
        public void TestAddLegalEntity()
        {
            LegalEntityEditDTO legalEntity = new()
            {
                //Eik = "48792156",
                //LegalName = "Морски бриз ЕООД",
                AuthorizedPeople = new List<AuthorizedPersonDTO>(),
                Files = new List<FileInfoDTO>()
            };

            service.AddLegalEntity(legalEntity);
            Assert.Equal(5, db.Legals.Count());
        }
    }
}
