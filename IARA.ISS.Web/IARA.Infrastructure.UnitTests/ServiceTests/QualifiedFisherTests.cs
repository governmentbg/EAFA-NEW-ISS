//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading;
//using IARA.Common.Enums;
//using IARA.Common.Utils;
//using IARA.DataAccess;
//using IARA.DomainModels.DTOModels.Common;
//using IARA.DomainModels.DTOModels.QualifiedFrishersRegister;
//using IARA.DomainModels.RequestModels;
//using IARA.EntityModels.Entities;
//using IARA.Fakes.MockupData;
//using IARA.Interfaces;
//using Xunit;

//namespace IARA.Infrastructure.UnitTests.ServiceTests
//{
//    public class QualifiedFisherTests
//    {
//        private IARADbContext Db;
//        private IQualifiedFishersService service;

//        public QualifiedFisherTests(IARADbContext Db, IQualifiedFishersService service)
//        {
//            Thread.CurrentPrincipal = new ClaimsPrincipal(new ClaimsIdentity("Bearer", "TEST_USER", "TEST_ROLE"));
//            this.Db = Db;
//            this.service = service;
//            this.Db.NaddressTypes.AddRange(AddressData.AddressTypes);
//            this.Db.Ndistricts.AddRange(AddressData.Districts);
//            this.Db.Nmunicipalities.AddRange(AddressData.Municipalities);
//            this.Db.NpopulatedAreas.AddRange(AddressData.PopulatedAreas);
//            this.Db.Addresses.AddRange(AddressData.Addresses);
//            this.Db.Persons.AddRange(PersonsData.Persons);
//            this.Db.NterritoryUnits.AddRange(TerritoryData.TerritoryUnits);
//            this.Db.FishermenRegisters.AddRange(QualifiedFishersData.Fishers);
//            this.Db.SaveChanges();
//        }

//        [Fact(DisplayName = "Тества извличане на всички рибари без филтри")]
//        public void TestGetAllEmptyFilter()
//        {
//            QualifiedFishersFilters filter = new QualifiedFishersFilters();

//            var result = service.GetAll(filter).ToList();
//            var data = QualifiedFishersData.Fishers.Where(x => x.IsActive);

//            Assert.NotNull(result);
//            Assert.False(result.Any(x => !x.IsActive));
//            Assert.Equal(data.Count(), result.Count());

//            foreach (QualifiedFisherDTO dto in result)
//            {
//                FishermenRegister registerEntry = data.SingleOrDefault(x => x.Id == dto.Id);
//                Assert.NotNull(registerEntry);
//                AssertMatch(registerEntry, dto);
//            }
//        }

//        [Fact(DisplayName = "Тества извличане на всички рибари с филтър по дата")]
//        public void TestGetAllFilterDate()
//        {
//            QualifiedFishersFilters filter;

//            filter = new QualifiedFishersFilters();
//            filter.RegisteredDateFrom = new DateTime(2020, 1, 1);
//            filter.RegisteredDateTo = new DateTime(2022, 1, 1);

//            var result = service.GetAll(filter).ToList();
//            var data = QualifiedFishersData.Fishers.Where(x => x.IsActive && x.RegistrationDate <= filter.RegisteredDateTo && x.RegistrationDate >= filter.RegisteredDateFrom);

//            Assert.NotNull(result);
//            Assert.Equal(data.Count(), result.Count());

//            foreach (QualifiedFisherDTO dto in result)
//            {
//                FishermenRegister registerEntry = data.SingleOrDefault(x => x.Id == dto.Id);
//                Assert.NotNull(registerEntry);
//                AssertMatch(registerEntry, dto);
//            }
//        }

//        [Fact(DisplayName = "Тества извличане на всички рибари с филтър по номер на сертификат")]
//        public void TestGetAllFilterCert()
//        {
//            QualifiedFishersFilters filter;

//            filter = new QualifiedFishersFilters();
//            filter.ExamCert = true;
//            filter.ExamCertNr = "905";

//            var result = service.GetAll(filter).ToList();
//            var data = QualifiedFishersData.Fishers.Where(x => x.IsActive && x.HasExamLicense && x.ExamLicenseNum.Contains(filter.ExamCertNr));

//            Assert.NotNull(result);
//            Assert.Equal(data.Count(), result.Count());

//            foreach (QualifiedFisherDTO dto in result)
//            {
//                FishermenRegister registerEntry = data.SingleOrDefault(x => x.Id == dto.Id);
//                Assert.NotNull(registerEntry);
//                AssertMatch(registerEntry, dto);
//            }
//        }

//        [Fact(DisplayName = "Тества извличане на всички рибари с филтър по име")]
//        public void TestGetAllFilterName()
//        {
//            QualifiedFishersFilters filter;

//            filter = new QualifiedFishersFilters();
//            filter.Name = "ИвАн";

//            var result = service.GetAll(filter).ToList();
//            var data = QualifiedFishersData.Fishers.Where(x => x.IsActive && PersonsData.Persons.Where(y => y.Id == x.PersonId && (y.FirstName.ToLower().Contains(filter.Name.ToLower()) || y.MiddleName.ToLower().Contains(filter.Name.ToLower()) || y.LastName.ToLower().Contains(filter.Name.ToLower()))).Any());

//            Assert.NotNull(result);
//            Assert.Equal(data.Count(), result.Count());

//            foreach (QualifiedFisherDTO dto in result)
//            {
//                FishermenRegister registerEntry = data.SingleOrDefault(x => x.Id == dto.Id);
//                Assert.NotNull(registerEntry);
//                AssertMatch(registerEntry, dto);
//            }
//        }

//        [Fact(DisplayName = "Тества извличане на всички рибари с търсене на свободен текст, съдържащ дата")]
//        public void TestGetAllTextSearchDate()
//        {
//            QualifiedFishersFilters filter = new QualifiedFishersFilters();
//            filter.FreeTextSearch = "28.3.21";

//            var result = service.GetAll(filter).ToList();
//            DateTime tmpDate = DateTimeUtils.TryParseDate(filter.FreeTextSearch).Value;// new DateTime(2021, 3, 28);
//                                                                                       // var data = QualifiedFishersData.Fishers.Where(x => x.IsActive && (x.DiplomaGraduationDate == tmpDate || x.ExamLicenseDate == tmpDate || x.ExamDate == tmpDate || x.RegistrationDate == tmpDate));

//            Assert.NotNull(result);
//            Assert.Equal(data.Count(), result.Count());

//            foreach (QualifiedFisherDTO dto in result)
//            {
//                FishermenRegister registerEntry = data.SingleOrDefault(x => x.Id == dto.Id);
//                Assert.NotNull(registerEntry);
//                AssertMatch(registerEntry, dto);
//            }
//        }

//        [Fact(DisplayName = "Тества извличане на рибар за диалог за редакция")]
//        public void TestGetForEdit()
//        {
//            int id = 1;
//            QualifiedFisherEditDTO result = service.GetRegisterEntry(id);
//            FishermenRegister data = QualifiedFishersData.Fishers.Single(x => x.Id == id);

//            Assert.NotNull(result);
//            AssertMatch(data, result);
//        }

//        [Fact(DisplayName = "Тества добавяне на рибар")]
//        public void TestAdd()
//        {
//            RegixPersonDataDTO regixData = new RegixPersonDataDTO()
//            {
//                EgnLnc = new EgnLncDTO
//                {
//                    EgnLnc = "5508159221",
//                    IdentifierType = IdentifierTypeEnum.EGN
//                },
//                FirstName = "Димитър",
//                MiddleName = "Василев",
//                LastName = "Маринов",
//                Document = null,
//                CitizenshipCountryId = 1,
//                Phone = "0875318008",
//                Email = "dvedimq6tidula69@abv.bg",
//                BirthDate = null,
//                HasBulgarianAddressRegistration = true,
//            };

//            List<AddressRegistrationDTO> addressData = new List<AddressRegistrationDTO>
//            {
//                new AddressRegistrationDTO
//                {
//                    AddressType = AddressTypesEnum.PERMANENT,
//                    CountryId = 1,
//                    DistrictId = 5,
//                    MunicipalityId = 575,
//                    PopulatedAreaId = 7253,
//                    Region = null,
//                    PostalCode = null,
//                    Street = "Баба Мота",
//                    StreetNum = null,
//                    BlockNum = null,
//                    EntranceNum = null,
//                    FloorNum = null,
//                    ApartmentNum = null,
//                },
//                new AddressRegistrationDTO
//                {
//                    AddressType = AddressTypesEnum.CORRESPONDENCE,
//                    CountryId = 1,
//                    DistrictId = 3,
//                    MunicipalityId = 569,
//                    PopulatedAreaId = 7142,
//                    Region = null,
//                    PostalCode = null,
//                    Street = "Одрин",
//                    StreetNum = null,
//                    BlockNum = null,
//                    EntranceNum = null,
//                    FloorNum = null,
//                    ApartmentNum = null,
//                },
//            };

//            QualifiedFisherEditDTO fisher = new QualifiedFisherEditDTO
//            {
//                Id = null,
//                RegistrationDate = DateTime.Today,
//                Name = "Димитър Василев Маринов",
//                EGN = "5508159221",
//                HasExam = true,
//                ExamProtocolNumber = "52907692",
//                ExamProtocolDate = new DateTime(2015, 09, 12),
//                ExamLicenseNumber = "39074920",
//                ExamLicenseDate = new DateTime(2015, 10, 01),
//                DiplomaNumber = null,
//                DiplomaDate = null,
//                DiplomaOrExamLabel = null,
//                DiplomaOrExamNumber = null,
//                IsActive = true,
//                //TerritoryUnitName = "Тестова Територия 12",
//                //TerritoryUnitId = 2,
//                //RegixPersonData = regixData,
//                // AddressRegistrations = addressData,
//            };

//            int newFisherId = service.AddRegisterEntry(fisher);

//            QualifiedFisherEditDTO fisherFeedback = service.GetRegisterEntry(newFisherId);

//            Assert.NotNull(fisherFeedback);
//            AssertMatch(fisher, fisherFeedback);
//        }

//        [Fact(DisplayName = "Тества смяна на IsActive на рибар")]
//        public void TestRestore()
//        {
//            int id = 1;

//            service.Delete(id);

//            var fisher = service.GetRegisterEntry(id);

//            Assert.False(fisher.IsActive);

//            service.UndoDelete(id);

//            fisher = service.GetRegisterEntry(id);

//            Assert.True(fisher.IsActive);
//        }

//        private void AssertMatch(QualifiedFisherEditDTO first, QualifiedFisherEditDTO second)
//        {
//            Assert.Equal(first.RegistrationDate, second.RegistrationDate);
//            Assert.Equal(first.Name, second.Name);
//            Assert.Equal(first.EGN, second.EGN);
//            Assert.Equal(first.HasExam, second.HasExam);
//            Assert.Equal(first.ExamProtocolNumber, second.ExamProtocolNumber);
//            Assert.Equal(first.ExamProtocolDate, second.ExamProtocolDate);
//            Assert.Equal(first.ExamLicenseNumber, second.ExamLicenseNumber);
//            Assert.Equal(first.ExamLicenseDate, second.ExamLicenseDate);
//            Assert.Equal(first.DiplomaNumber, second.DiplomaNumber);
//            Assert.Equal(first.DiplomaDate, second.DiplomaDate);
//            Assert.Equal(first.IsActive, second.IsActive);
//            //Assert.Equal(first.TerritoryUnitId, second.TerritoryUnitId);
//        }

//        private void AssertMatch(FishermenRegister registerEntry, QualifiedFisherEditDTO dto)
//        {
//            //NterritoryUnit territoryUnit = TerritoryData.TerritoryUnits.Single(x => x.Id == registerEntry.TerritoryUnitId);
//            Person person = PersonsData.Persons.Single(x => x.Id == registerEntry.PersonId);

//            Assert.Equal(registerEntry.Id, dto.Id);
//            Assert.Equal(registerEntry.RegistrationDate, dto.RegistrationDate);
//            //Assert.Equal(territoryUnit.Id, dto.TerritoryUnitId);
//            Assert.Equal(person.FirstName + " " + (String.IsNullOrWhiteSpace(person.MiddleName) ? "" : person.MiddleName + " ") + person.LastName, dto.Name);
//            Assert.Equal(registerEntry.HasExamLicense, dto.HasExam);
//            Assert.Equal(registerEntry.ExamProtocolNum, dto.ExamProtocolNumber);
//            Assert.Equal(registerEntry.ExamDate, dto.ExamProtocolDate);
//            Assert.Equal(registerEntry.ExamLicenseNum, dto.ExamLicenseNumber);
//            Assert.Equal(registerEntry.ExamLicenseDate, dto.ExamLicenseDate);
//            Assert.Equal(registerEntry.DiplomaNum, dto.DiplomaNumber);
//            Assert.Equal(registerEntry.DiplomaGraduationDate, dto.DiplomaDate);
//            Assert.Equal(registerEntry.IsActive, dto.IsActive);
//            Assert.Equal(registerEntry.HasExamLicense ? "e" : "d", dto.DiplomaOrExamLabel);
//        }

//        private void AssertMatch(FishermenRegister registerEntry, QualifiedFisherDTO dto)
//        {
//            //NterritoryUnit territoryUnit = TerritoryData.TerritoryUnits.Single(x => x.Id == registerEntry.TerritoryUnitId);
//            Person person = PersonsData.Persons.Single(x => x.Id == registerEntry.PersonId);

//            Assert.Equal(registerEntry.Id, dto.Id);
//            Assert.Equal(registerEntry.RegistrationDate, dto.RegistrationDate);
//            //Assert.Equal(territoryUnit.Name, dto.TerritoryUnitName);
//            Assert.Equal(person.FirstName + " " + person.LastName, dto.Name);
//            Assert.Equal(registerEntry.HasExamLicense, dto.HasExam);
//            Assert.Equal(registerEntry.ExamProtocolNum, dto.ExamProtocolNumber);
//            Assert.Equal(registerEntry.ExamDate, dto.ExamProtocolDate);
//            Assert.Equal(registerEntry.ExamLicenseNum, dto.ExamLicenseNumber);
//            Assert.Equal(registerEntry.ExamLicenseDate, dto.ExamLicenseDate);
//            Assert.Equal(registerEntry.DiplomaNum, dto.DiplomaNumber);
//            Assert.Equal(registerEntry.DiplomaGraduationDate, dto.DiplomaDate);
//            Assert.Equal(registerEntry.IsActive, dto.IsActive);
//            Assert.Equal(registerEntry.HasExamLicense ? "e" : "d", dto.DiplomaOrExamLabel);
//        }
//    }
//}
