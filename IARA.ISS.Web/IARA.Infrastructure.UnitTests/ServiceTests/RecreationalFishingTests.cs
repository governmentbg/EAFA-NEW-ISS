using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.RecreationalFishing;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Fakes.MockupData;
using IARA.Interfaces;
using Xunit;

namespace IARA.Infrastructure.UnitTests.ServiceTests
{
    public class RecreationalFishingTests
    {
        private readonly IARADbContext db;
        private readonly IRecreationalFishingService service;
        private readonly IRecreationalFishingAssociationService associationService;

        public RecreationalFishingTests(IARADbContext db, IRecreationalFishingService service, IRecreationalFishingAssociationService associationService)
        {
            Thread.CurrentPrincipal = new ClaimsPrincipal(new ClaimsIdentity("Bearer", "TEST_USER", "TEST_ROLE"));
            this.db = db;
            this.service = service;
            this.associationService = associationService;

            SetupDb();
        }

        [Fact(DisplayName = "Извличане на всички активни сдружения")]
        public void TestGetAllActiveAssociations()
        {
            RecreationalFishingAssociationsFilters filters = new() { ShowInactiveRecords = false };
            List<RecreationalFishingAssociationDTO> records = associationService.GetAllAssociations(filters).ToList();

            List<FishingAssociation> dbRecords = db.FishingAssociations.Where(x => x.IsActive).ToList();
            Assert.Equal(dbRecords.Count, records.Count);

            foreach (FishingAssociation dbAssoc in dbRecords)
            {
                RecreationalFishingAssociationDTO assoc = records.Where(x => x.ID == dbAssoc.Id).SingleOrDefault();
                Assert.NotNull(assoc);
                AssertMatching(assoc, dbAssoc);
            }
        }

        [Fact(DisplayName = "Извличане на всички неактивни сдружения")]
        public void TestGetAllInactiveAssociations()
        {
            RecreationalFishingAssociationsFilters filters = new() { ShowInactiveRecords = true };
            List<RecreationalFishingAssociationDTO> records = associationService.GetAllAssociations(filters).ToList();

            List<FishingAssociation> dbRecords = db.FishingAssociations.Where(x => !x.IsActive).ToList();
            Assert.Equal(dbRecords.Count, records.Count);

            foreach (FishingAssociation dbAssoc in dbRecords)
            {
                RecreationalFishingAssociationDTO assoc = records.Where(x => x.ID == dbAssoc.Id).SingleOrDefault();
                Assert.NotNull(assoc);
                AssertMatching(assoc, dbAssoc);
            }
        }

        [Fact(DisplayName = "Извличане на всички сдружения с комплексни филтри")]
        public void TestGetAllAssociationsWithComplexFilters()
        {
            RecreationalFishingAssociationsFilters filters = new()
            {
                Name = "юридическо",
                EIK = "1111",
                TerritoryUnitId = TerritoryData.TerritoryUnits[0].Id,
                RepresentativePersonId = PersonsData.Persons[0].Id
            };

            List<RecreationalFishingAssociationDTO> records = associationService.GetAllAssociations(filters).ToList();

            Assert.Single(records);

            FishingAssociation dbAssoc = db.FishingAssociations.Where(x => x.Id == FishingAssociationsData.FishingAssociations[0].Id).Single();

            RecreationalFishingAssociationDTO assoc = records[0];
            AssertMatching(assoc, dbAssoc);
        }

        [Fact(DisplayName = "Извличане на всички сдружения с филтър свободен текст (име)")]
        public void TestGetAllAssociationsWithTextFilterName()
        {
            string text = "юридическо";
            List<FishingAssociation> dbRecords = db.FishingAssociations
                .Where(x => x.IsActive && x.AssociationLegal.Name.ToLower().Contains(text))
                .ToList();

            TestFreeTextSearch(text, dbRecords);
        }

        [Fact(DisplayName = "Извличане на всички сдружения с филтър свободен текст (ЕИК)")]
        public void TestGetAllAssociationsWithTextFilterEIK()
        {
            string text = "111";
            List<FishingAssociation> dbRecords = db.FishingAssociations
                .Where(x => x.IsActive && x.AssociationLegal.Eik.ToLower().Contains(text))
                .ToList();

            TestFreeTextSearch(text, dbRecords);
        }

        [Fact(DisplayName = "Извличане на всички сдружения с филтър свободен текст (териториално звено)")]
        public void TestGetAllAssociationsWithTextFilterTerritoryUnit()
        {
            string text = "територия 1";
            List<FishingAssociation> dbRecords = db.FishingAssociations
                .Where(x => x.IsActive && x.TerritoryUnit.Name.ToLower().Contains(text))
                .ToList();

            TestFreeTextSearch(text, dbRecords);
        }

        [Fact(DisplayName = "Извличане на всички сдружения с филтър свободен текст (телефон)")]
        public void TestGetAllAssociationsWithTextFilterPhone()
        {
            string text = "0887397553";
            List<FishingAssociation> dbRecords = db.FishingAssociations
                .Where(x => x.IsActive && x.AssociationLegal.LegalPhoneNumbers.Where(x => x.IsActive).Select(x => x.Phone.Phone).Single().ToLower().Contains(text))
                .ToList();

            TestFreeTextSearch(text, dbRecords);
        }

        [Fact(DisplayName = "Извличане на сдружение")]
        public void TestGetAssociation()
        {
            FishingAssociation dbAssoc = db.FishingAssociations
                .Where(x => x.Id == FishingAssociationsData.FishingAssociations[2].Id)
                .Single();

            RecreationalFishingAssociationEditDTO assoc = associationService.GetAssociation(dbAssoc.Id);

            AssertMatching(assoc, dbAssoc, adding: false);
        }

        [Fact(DisplayName = "Добавяне на сдружение")]
        public void TestAddAssociation()
        {
            RecreationalFishingAssociationEditDTO assoc = new RecreationalFishingAssociationEditDTO
            {
                TerritoryUnitId = TerritoryData.TerritoryUnits[0].Id,
                Legal = new RegixLegalDataDTO
                {
                    EIK = "987654321",
                    Name = "Test",
                    Phone = "09456234",
                    Email = "recfish1@gmail.com"
                },
                LegalAddresses = new List<AddressRegistrationDTO>
                {
                    new AddressRegistrationDTO
                    {
                        AddressType = AddressTypesEnum.PERMANENT,
                        CountryId = AddressData.Countries[0].Id,
                        DistrictId = AddressData.Districts[0].Id,
                        MunicipalityId = AddressData.Municipalities[0].Id,
                        PopulatedAreaId = AddressData.PopulatedAreas[0].Id,
                        Region = AddressData.Addresses[0].Region,
                        PostalCode = AddressData.Addresses[0].PostCode,
                        Street = AddressData.Addresses[0].Street,
                        StreetNum = AddressData.Addresses[0].StreetNum,
                        BlockNum = AddressData.Addresses[0].BlockNum,
                        EntranceNum = AddressData.Addresses[0].EntranceNum,
                        FloorNum = AddressData.Addresses[0].FloorNum,
                        ApartmentNum = AddressData.Addresses[0].ApartmentNum
                    }
                },
                IsCanceled = false,
                CancellationDate = null,
                CancellationReason = null
            };

            int id = associationService.AddAssociation(assoc);

            FishingAssociation dbAssoc = db.FishingAssociations.Where(x => x.Id == id).SingleOrDefault();
            Assert.NotNull(dbAssoc);

            Assert.Equal(FishingAssociationsData.FishingAssociations.Count + 1, db.FishingAssociations.Count());

            AssertMatching(assoc, dbAssoc, adding: true);
        }

        [Fact(DisplayName = "Редактиране на сдружение")]
        public void TestEditAssociation()
        {
            FishingAssociation dbAssoc = db.FishingAssociations.Where(x => x.Id == FishingAssociationsData.FishingAssociations[0].Id).Single();

            RecreationalFishingAssociationEditDTO assoc = CopyEntityToDTO(dbAssoc);
            EditDTO(assoc);

            associationService.EditAssociation(assoc);

            dbAssoc = db.FishingAssociations.Where(x => x.Id == FishingAssociationsData.FishingAssociations[0].Id).Single();

            Assert.NotNull(dbAssoc);
            Assert.Equal(FishingAssociationsData.FishingAssociations.Count, db.FishingAssociations.Count());
            AssertMatching(assoc, dbAssoc, adding: false);
        }

        [Fact(DisplayName = "Изтриване на сдружение")]
        public void TestDeleteAssociation()
        {
            FishingAssociation association = db.FishingAssociations.Where(x => x.Id == FishingAssociationsData.FishingAssociations[0].Id).Single();
            associationService.DeleteAssociation(association.Id);

            Assert.False(association.IsActive);
        }

        [Fact(DisplayName = "Възстановяване на сдружение")]
        public void TestRestoreAssociation()
        {
            FishingAssociation association = db.FishingAssociations.Where(x => x.Id == FishingAssociationsData.FishingAssociations[1].Id).Single();
            associationService.UndoDeleteAssociation(association.Id);

            Assert.True(association.IsActive);
        }

        private void SetupDb()
        {
            db.Ncountries.AddRange(AddressData.Countries);
            db.Ndistricts.AddRange(AddressData.Districts);
            db.Nmunicipalities.AddRange(AddressData.Municipalities);
            db.NpopulatedAreas.AddRange(AddressData.PopulatedAreas);
            db.NdocumentTypes.AddRange(PersonsData.NdocumentTypes);
            db.NterritoryUnits.AddRange(TerritoryData.TerritoryUnits);

            db.PhoneNumbers.AddRange(PersonsData.PhoneNumbers);
            db.EmailAddresses.AddRange(PersonsData.EmailAddresses);
            db.PhoneNumbers.AddRange(LegalsData.PhoneNumbers);
            db.EmailAddresses.AddRange(LegalsData.EmailAddresses);
            db.NaddressTypes.AddRange(AddressData.AddressTypes);
            db.Addresses.AddRange(AddressData.Addresses);

            db.Legals.AddRange(LegalsData.Legals);
            db.LegalEmailAddresses.AddRange(LegalsData.LegalEmailAddresses);
            db.LegalPhoneNumbers.AddRange(LegalsData.LegalPhoneNumbers);
            db.LegalsAddresses.AddRange(LegalsData.LegalAddresses);

            db.Persons.AddRange(PersonsData.Persons);
            db.PersonEmailAddresses.AddRange(PersonsData.PersonEmailAddresses);
            db.PersonPhoneNumbers.AddRange(PersonsData.PersonPhoneNumbers);
            db.PersonAddresses.AddRange(PersonsData.PersonAddresses);
            db.PersonDocuments.AddRange(PersonsData.PersonDocuments);

            db.FishingAssociations.AddRange(FishingAssociationsData.FishingAssociations);

            db.SaveChanges();
        }

        private void TestFreeTextSearch(string text, List<FishingAssociation> dbRecords)
        {
            RecreationalFishingAssociationsFilters filters = new() { FreeTextSearch = text };
            List<RecreationalFishingAssociationDTO> records = associationService.GetAllAssociations(filters).ToList();

            Assert.Equal(dbRecords.Count, records.Count);

            foreach (FishingAssociation dbAssoc in dbRecords)
            {
                RecreationalFishingAssociationDTO assoc = records.Where(x => x.ID == dbAssoc.Id).SingleOrDefault();
                Assert.NotNull(assoc);
                AssertMatching(assoc, dbAssoc);
            }
        }

        private static void AssertMatching(RecreationalFishingAssociationEditDTO assoc, FishingAssociation dbAssoc, bool adding)
        {
            if (!adding)
            {
                Assert.Equal(dbAssoc.Id, assoc.ID);
            }

            Assert.Equal(dbAssoc.TerritoryUnitId, assoc.TerritoryUnitId);

            Legal dbLegal = dbAssoc.AssociationLegal;
            Assert.NotNull(dbLegal);
            Assert.Equal(dbLegal.Eik, assoc.Legal.EIK);
            Assert.Equal(dbLegal.Name, assoc.Legal.Name);
            Assert.Equal(dbLegal.LegalPhoneNumbers.Where(x => x.IsActive).Select(x => x.Phone.Phone).Single(), assoc.Legal.Phone);
            Assert.Equal(dbLegal.LegalEmailAddresses.Where(x => x.IsActive).Select(x => x.EmailAddress.Email).Single(), assoc.Legal.Email);

            AssertMatching(dbLegal.LegalsAddresses, assoc.LegalAddresses);

            Assert.Equal(dbAssoc.IsCanceled, assoc.IsCanceled);
            Assert.Equal(dbAssoc.CancellationDate, assoc.CancellationDate);
            Assert.Equal(dbAssoc.CancellationReason, assoc.CancellationReason);
        }

        private static void AssertMatching(RecreationalFishingAssociationDTO assoc, FishingAssociation dbAssoc)
        {
            Assert.Equal(dbAssoc.Id, assoc.ID);
            Assert.Equal(dbAssoc.AssociationLegal.Name, assoc.Name);
            Assert.Equal(dbAssoc.TerritoryUnit.Name, assoc.TerritoryUnit);
            Assert.Equal(dbAssoc.AssociationLegal.Eik, assoc.EIK);
            Assert.Equal(dbAssoc.AssociationLegal.LegalPhoneNumbers.Where(x => x.IsActive).Select(x => x.Phone.Phone).SingleOrDefault(), assoc.Phone);
            Assert.Equal(dbAssoc.FishingAssociationMembers.Count(x => x.IsActive), assoc.MembersCount);
            Assert.Equal(dbAssoc.IsCanceled, assoc.IsCanceled);
            Assert.Equal(dbAssoc.IsActive, assoc.IsActive);
        }

        private static void AssertMatching(ICollection<LegalsAddress> dbLegalAddresses, List<AddressRegistrationDTO> addresses)
        {
            Assert.Equal(dbLegalAddresses.Count(x => x.IsActive), addresses.Count);
            foreach (AddressRegistrationDTO address in addresses)
            {
                LegalsAddress dbLegalAddress = dbLegalAddresses.Where(x => x.AddressType.Code == address.AddressType.ToString()).SingleOrDefault();
                Assert.NotNull(dbLegalAddress);
                Assert.Equal(dbLegalAddress.AddressType.Code, address.AddressType.ToString());

                Address dbAddress = dbLegalAddress.Address;
                Assert.NotNull(dbAddress);
                AssertMatching(dbAddress, address);
            }
        }

        private static void AssertMatching(ICollection<PersonAddress> dbPersonAddresses, List<AddressRegistrationDTO> addresses)
        {
            Assert.Equal(dbPersonAddresses.Count(x => x.IsActive), addresses.Count);
            foreach (AddressRegistrationDTO address in addresses)
            {
                PersonAddress dbPersonAddress = dbPersonAddresses.Where(x => x.AddressType.Code == address.AddressType.ToString()).SingleOrDefault();
                Assert.NotNull(dbPersonAddress);
                Assert.Equal(dbPersonAddress.AddressType.Code, address.AddressType.ToString());

                Address dbAddress = dbPersonAddress.Address;
                Assert.NotNull(dbAddress);
                AssertMatching(dbAddress, address);
            }
        }

        private static void AssertMatching(Address dbAddress, AddressRegistrationDTO address)
        {
            Assert.Equal(dbAddress.CountryId, address.CountryId);
            Assert.Equal(dbAddress.DistrictId, address.DistrictId);
            Assert.Equal(dbAddress.MunicipalityId, address.MunicipalityId);
            Assert.Equal(dbAddress.PopulatedAreaId, address.PopulatedAreaId);
            Assert.Equal(dbAddress.Region, address.Region);
            Assert.Equal(dbAddress.PostCode, address.PostalCode);
            Assert.Equal(dbAddress.Street, address.Street);
            Assert.Equal(dbAddress.StreetNum, address.StreetNum);
            Assert.Equal(dbAddress.BlockNum, address.BlockNum);
            Assert.Equal(dbAddress.EntranceNum, address.EntranceNum);
            Assert.Equal(dbAddress.FloorNum, address.FloorNum);
            Assert.Equal(dbAddress.ApartmentNum, address.ApartmentNum);
        }

        private static RecreationalFishingAssociationEditDTO CopyEntityToDTO(FishingAssociation dbAssoc)
        {
            return new RecreationalFishingAssociationEditDTO
            {
                ID = dbAssoc.Id,
                TerritoryUnitId = dbAssoc.TerritoryUnitId,
                Legal = new RegixLegalDataDTO
                {
                    EIK = dbAssoc.AssociationLegal.Eik,
                    Name = dbAssoc.AssociationLegal.Name,
                    Phone = dbAssoc.AssociationLegal.LegalPhoneNumbers.Where(x => x.IsActive).Select(x => x.Phone.Phone).SingleOrDefault(),
                    Email = dbAssoc.AssociationLegal.LegalEmailAddresses.Where(x => x.IsActive).Select(x => x.EmailAddress.Email).SingleOrDefault()
                },
                LegalAddresses = dbAssoc.AssociationLegal.LegalsAddresses.Where(x => x.IsActive).Select(x => AddressToDTO(x.AddressType.Code, x.Address)).ToList(),
                IsCanceled = dbAssoc.IsCanceled,
                CancellationDate = dbAssoc.CancellationDate,
                CancellationReason = dbAssoc.CancellationReason
            };
        }

        private static AddressRegistrationDTO AddressToDTO(string addressTypeCode, Address address)
        {
            return new AddressRegistrationDTO
            {
                AddressType = Enum.Parse<AddressTypesEnum>(addressTypeCode),
                CountryId = address.CountryId.Value,
                DistrictId = address.DistrictId,
                MunicipalityId = address.MunicipalityId,
                PopulatedAreaId = address.PopulatedAreaId,
                Region = address.Region,
                PostalCode = address.PostCode,
                Street = address.Street,
                StreetNum = address.StreetNum,
                BlockNum = address.BlockNum,
                EntranceNum = address.EntranceNum,
                FloorNum = address.FloorNum,
                ApartmentNum = address.ApartmentNum
            };
        }

        private static void EditDTO(RecreationalFishingAssociationEditDTO assoc)
        {
            assoc.TerritoryUnitId = TerritoryData.TerritoryUnits[1].Id;

            assoc.Legal.Name = $"{assoc.Legal.Name} editted";
            assoc.Legal.Phone = $"{assoc.Legal.Phone}1234";
            assoc.Legal.Email = $"{assoc.Legal.Email}.other.com";

            assoc.IsCanceled = !assoc.IsCanceled;
            assoc.CancellationDate = assoc?.CancellationDate?.AddDays(20) ?? new DateTime(1996, 8, 18);
            assoc.CancellationReason = $"{assoc.CancellationReason} editted";
        }
    }
}
