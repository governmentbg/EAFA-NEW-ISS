using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Reports;
using IARA.DomainModels.RequestModels;
using IARA.Fakes.MockupData;
using IARA.Infrastructure.Services;
using IARA.Interfaces;
using IARA.Interfaces.Reports;
using Xunit;

namespace IARA.Infrastructure.UnitTests.ServiceTests
{
    public class ReportTests
    {
        private IARADbContext db;
        private readonly IPersonReportsService service;

        public ReportTests(IARADbContext db, IPersonReportsService service)
        {
            Thread.CurrentPrincipal = new ClaimsPrincipal(new ClaimsIdentity("Bearer", "TEST_USER", "TEST_ROLE"));
            this.db = db;
            this.service = service;

            this.db.Persons.AddRange(PersonsData.Persons);
            this.db.Legals.AddRange(LegalsData.Legals);
            this.db.SaveChanges();
        }

        [Fact(DisplayName = "Извличане на всички справки за юридически лица без филтри")]
        public void TestGetAllLegalEntitiesReportEmptyFilters()
        {
            LegalEntitiesReportFilters filters = new LegalEntitiesReportFilters();
            IQueryable<LegalEntityReportDTO> result = service.GetAllLegalEntitiesReport(filters);

            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        [Fact(DisplayName = "Извличане на справки за юридически лица по зададен текст за търсене")]
        public void TestGetAllLegalEntitiesReportFreeTextFilter()
        {
            LegalEntitiesReportFilters filters = new LegalEntitiesReportFilters { FreeTextSearch = "2222" };
            IQueryable<LegalEntityReportDTO> result = service.GetAllLegalEntitiesReport(filters);

            Assert.NotNull(result);
            Assert.Equal(1, result.Count());
        }

        [Fact(DisplayName = "Извличане на справки за юридически лица по зададени комплексни филтри")]
        public void TestGetParametersFilteredLegalEntitiesReport()
        {
            LegalEntitiesReportFilters filters = new LegalEntitiesReportFilters { Eik = LegalsData.Legals[2].Eik, LegalName = LegalsData.Legals[2].Name };
            IQueryable<LegalEntityReportDTO> result = service.GetAllLegalEntitiesReport(filters);

            Assert.NotNull(result);
            Assert.Equal(1, result.Count());
        }

        [Fact(DisplayName = "Извличане на справка за юридическо лице по id")]
        public void TestGetLegalEntityReport()
        {
            LegalEntityReportInfoDTO legalEntityReport = service.GetLegalEntityReport(LegalsData.Legals[0].Id);
            Assert.NotNull(legalEntityReport);
        }

        [Fact(DisplayName = "Извличане на история за справка юридически лица")]
        public void TestGetLegalEntitiesHistory()
        {
            List<ReportHistoryDTO> result = service.GetLegalEntitiesHistory(new List<int>() { LegalsData.Legals[1].Id, LegalsData.Legals[2].Id });
            Assert.NotNull(result);
        }

        [Fact(DisplayName = "Извличане на всички справки за физически лица без филтри")]
        public void TestGetAllPersonsReportEmptyFilters()
        {
            PersonsReportFilters filters = new PersonsReportFilters();
            IQueryable<PersonReportDTO> result = service.GetAllPersonsReport(filters);

            Assert.NotNull(result);
            Assert.Equal(1, result.Count());
        }

        [Fact(DisplayName = "Извличане на справки за физически лица по зададен текст за търсене")]
        public void TestGetAllPersonsReportFreeTextFilter()
        {
            PersonsReportFilters filters = new PersonsReportFilters { FreeTextSearch = "9509291111" };
            IQueryable<PersonReportDTO> result = service.GetAllPersonsReport(filters);

            Assert.NotNull(result);
            Assert.Equal(1, result.Count());
        }

        [Fact(DisplayName = "Извличане на справки за физически лица по зададени комплексни филтри")]
        public void TestGetParametersFilteredPersonsReport()
        {
            PersonsReportFilters filters = new PersonsReportFilters
            {
                EGN = PersonsData.Persons[11].EgnLnc,
                LastName = PersonsData.Persons[11].LastName
            };

            IQueryable<PersonReportDTO> result = service.GetAllPersonsReport(filters);

            Assert.NotNull(result);
            Assert.Equal(1, result.Count());
        }

        [Fact(DisplayName = "Извличане на справка за физическо лице по id")]
        public void TestGetPersonReport()
        {
            PersonReportInfoDTO personReport = service.GetPersonReport(PersonsData.Persons[11].Id);
            Assert.NotNull(personReport);
        }

        [Fact(DisplayName = "Извличане на история за справка физически лица")]
        public void TestGetPeopleHistory()
        {
            List<ReportHistoryDTO> result = service.GetPeopleHistory(new List<int> { PersonsData.Persons[0].Id });
            Assert.NotNull(result);
        }
    }
}
