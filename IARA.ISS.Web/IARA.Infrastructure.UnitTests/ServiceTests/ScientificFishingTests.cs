using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.ScientificFishing;
using IARA.DomainModels.Nomenclatures;
using IARA.EntityModels.Entities;
using IARA.Fakes.MockupData;
using IARA.Interfaces;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace IARA.Infrastructure.UnitTests.ServiceTests
{
    public class ScientificFishingTests
    {
        private readonly IARADbContext db;
        private readonly IScientificFishingService service;

        public ScientificFishingTests(IARADbContext db, IScientificFishingService service)
        {
            Thread.CurrentPrincipal = new ClaimsPrincipal(new ClaimsIdentity("Bearer", "TEST_USER", "TEST_ROLE"));
            this.db = db;
            this.service = service;

            SetupDb();
        }

        [Fact(DisplayName = "Създаване на разрешително")]
        public void TestAddPermit()
        {
            ScientificFishingPermitEditDTO permit = new ScientificFishingPermitEditDTO
            {
                RegistrationDate = DateTime.Now,
                //PermitStatusId = ScientificPermitData.NpermitStatuses.Where(x => x.Code == nameof(ScientificPermitStatusEnum.Requested)).Single().Id,
                ValidFrom = DateTime.Now.AddDays(30),
                ValidTo = DateTime.Now.AddDays(30).AddYears(1),
                PermitReasonsIds = new List<int> { ScientificPermitData.NpermitReasons[0].Id, ScientificPermitData.NpermitReasons[1].Id },
                IsAllowedDuringMatingSeason = true,
                //RequesterFirstName = "Петър",
                //RequesterMiddleName = "Петканов",
                //RequesterLastName = "Петранков",
                //RequesterEgn = "98526452",
                //RequesterScientificOrganizationId = LegalsData.Legals[1].Id,
                //RequesterPosition = "Позиция",
                Holders = new List<ScientificFishingPermitHolderDTO>
                {
                    new ScientificFishingPermitHolderDTO
                    {
                        ScientificPosition = "Рибар",
                        RegixPersonData = new RegixPersonDataDTO
                        {
                            EgnLnc = new EgnLncDTO
                            {
                                EgnLnc = "856427",
                                IdentifierType = IdentifierTypeEnum.EGN,
                            },
                            FirstName = "Иван",
                            MiddleName = "Иванов",
                            LastName = "Петров",
                            Document = new PersonDocumentDTO
                            {
                                DocumentTypeID = PersonsData.NdocumentTypes[0].Id,
                                DocumentNumber = "12345",
                                DocumentIssuedOn = new DateTime(2020, 1, 4),
                                DocumentIssuedBy = "МВР"
                            },
                            CitizenshipCountryId = AddressData.Countries[0].Id,
                            Email = "ivan.ivanov.email@email.com",
                            BirthDate = new DateTime(1978, 2, 5),
                            HasBulgarianAddressRegistration = true
                        },
                        AddressRegistrations = new List<AddressRegistrationDTO>
                        {
                            new AddressRegistrationDTO
                            {
                                AddressType = AddressTypesEnum.PERMANENT,
                                CountryId = AddressData.Countries[0].Id,
                                DistrictId = AddressData.Districts[0].Id,
                                MunicipalityId = AddressData.Municipalities[0].Id,
                                PopulatedAreaId = AddressData.PopulatedAreas[0].Id,
                                Region = "Регион",
                                PostalCode = "8001",
                                Street = "ул. Еди-коя си",
                                StreetNum = "23",
                                BlockNum = "2",
                                EntranceNum = "1",
                                FloorNum = "11",
                                ApartmentNum = "65"
                            },
                            new AddressRegistrationDTO
                            {
                                AddressType = AddressTypesEnum.CORRESPONDENCE,
                                CountryId = AddressData.Countries[0].Id,
                                DistrictId = AddressData.Districts[1].Id,
                                MunicipalityId = AddressData.Municipalities[1].Id,
                                PopulatedAreaId = AddressData.PopulatedAreas[1].Id,
                                Street = "ул. Втора"
                            }
                        }
                    },
                    new ScientificFishingPermitHolderDTO
                    {
                        ScientificPosition = "Учен",
                        RegixPersonData = new RegixPersonDataDTO
                        {
                            EgnLnc = new EgnLncDTO
                            {
                                EgnLnc = "7425654",
                                IdentifierType = IdentifierTypeEnum.EGN,
                            },
                            FirstName = "Петко",
                            LastName = "Каравелов",
                            Document = new PersonDocumentDTO
                            {
                                DocumentTypeID = PersonsData.NdocumentTypes[1].Id,
                                DocumentNumber = "5555"
                            },
                            CitizenshipCountryId = AddressData.Countries[1].Id,
                            Phone = "0887523154",
                            BirthDate = new DateTime(1863, 4, 12),
                            HasBulgarianAddressRegistration = false
                        },
                        AddressRegistrations = new List<AddressRegistrationDTO>
                        {
                            new AddressRegistrationDTO
                            {
                                AddressType = AddressTypesEnum.PERMANENT,
                                CountryId = AddressData.Countries[0].Id,
                                DistrictId = AddressData.Districts[1].Id,
                                MunicipalityId = AddressData.Municipalities[1].Id,
                                PopulatedAreaId = AddressData.PopulatedAreas[1].Id,
                                Street = "ул. Възрожденска"
                            }
                        }
                    }
                },
                ResearchPeriodFrom = DateTime.Now.AddDays(30),
                ResearchPeriodTo = DateTime.Now.AddDays(30).AddYears(1),
                ResearchWaterArea = "Язовир Белмекен",
                ResearchGoalsDescription = "Някакви цели",
                FishTypesDescription = "риба",
                FishTypesApp4ZBRDesc = "риба 2",
                FishTypesCrayFish = "раци",
                FishingGearDescription = "въдици",
                IsShipRegistered = false,
                ShipName = "Лодка",
                ShipExternalMark = "знак",
                ShipCaptainName = "Радой"
            };

            int id = service.AddPermit(permit);
            ScientificPermitRegister newPermit = db.ScientificPermitRegisters.Where(x => x.Id == id).SingleOrDefault();

            Assert.NotNull(newPermit);
            Assert.Equal(ScientificPermitData.ScientificPermitRegisters.Count + 1, db.ScientificPermitRegisters.Count());

            AssertMatching(newPermit, permit, adding: true);
        }

        [Fact(DisplayName = "Редактиране на разрешително")]
        public void TestEditPermit()
        {
            ScientificPermitRegister dbPermit = db.ScientificPermitRegisters
                                        .Where(x => x.Id == ScientificPermitData.ScientificPermitRegisters[1].Id)
                                        .Single();

            CopyEntityToDTO(dbPermit, out ScientificFishingPermitEditDTO permit);
            EditDTO(ref permit);

            service.EditPermit(permit);

            dbPermit = db.ScientificPermitRegisters.Where(x => x.Id == permit.ID).SingleOrDefault();

            Assert.NotNull(dbPermit);
            Assert.Equal(ScientificPermitData.ScientificPermitRegisters.Count, db.ScientificPermitRegisters.Count());

            AssertMatching(dbPermit, permit);
        }

        [Fact(DisplayName = "Изтриване на разрешително")]
        public void TestDeletePermit()
        {
            ScientificPermitRegister permit = db.ScientificPermitRegisters
                                        .Where(x => x.Id == ScientificPermitData.ScientificPermitRegisters[0].Id)
                                        .Single();

            service.DeletePermit(permit.Id);
            Assert.False(permit.IsActive);
        }

        [Fact(DisplayName = "Възстановяване на разрешително")]
        public void TestRestorePermit()
        {
            ScientificPermitRegister permit = db.ScientificPermitRegisters
                                        .Where(x => x.Id == ScientificPermitData.ScientificPermitRegisters[3].Id)
                                        .Single();

            service.UndoDeletePermit(permit.Id);
            Assert.True(permit.IsActive);
        }

        [Fact(DisplayName = "Добавяне на излет")]
        public void TestAddOuting()
        {
            ScientificFishingOutingDTO newOuting = new ScientificFishingOutingDTO
            {
                PermitID = ScientificPermitData.ScientificPermitRegisters[1].Id,
                DateOfOuting = ScientificPermitData.ScientificPermitRegisters[1].ResearchPeriodFrom.AddDays(60),
                WaterArea = "Река Въча",
                Catches = new List<ScientificFishingOutingCatchDTO>
                {
                    new ScientificFishingOutingCatchDTO
                    {
                        FishType = new NomenclatureDTO
                        {
                            Value = FishesData.Nfishes[0].Id,
                            DisplayName = FishesData.Nfishes[0].Name
                        },
                        CatchUnder100 = 100,
                        Catch100To500 = 200,
                        Catch500To1000 = 300,
                        CatchOver1000 = 400,
                        TotalKeptCount = 250
                    },
                    new ScientificFishingOutingCatchDTO
                    {
                        FishType = new NomenclatureDTO
                        {
                            Value = FishesData.Nfishes[1].Id,
                            DisplayName = FishesData.Nfishes[1].Name
                        },
                        CatchUnder100 = 150,
                        Catch100To500 = 140,
                        Catch500To1000 = 130,
                        CatchOver1000 = 120,
                        TotalKeptCount = 200
                    }
                }
            };

            service.AddOuting(newOuting);

            List<ScientificPermitOuting> dbOutings = db.ScientificPermitOutings
                                                    .Where(x => x.ScientificPermitId == newOuting.PermitID)
                                                    .OrderByDescending(x => x.Id)
                                                    .ToList();

            Assert.Equal(ScientificPermitData.ScientificPermitOutings.Count(x => x.ScientificPermitId == newOuting.PermitID) + 1, dbOutings.Count);

            ScientificPermitOuting newDbOuting = dbOutings[0];
            Assert.Equal(newOuting.PermitID, newDbOuting.ScientificPermitId);
            Assert.Equal(newOuting.DateOfOuting, newDbOuting.OutingDate);
            Assert.Equal(newOuting.WaterArea, newDbOuting.WaterAreaDesc);

            db.Entry(newDbOuting).Collection(x => x.ScientificPermitOutingCatches).Load();
            Assert.Equal(newOuting.Catches.Count, newDbOuting.ScientificPermitOutingCatches.Count);
            foreach (ScientificPermitOutingCatch newDbOutingCatch in newDbOuting.ScientificPermitOutingCatches)
            {
                ScientificFishingOutingCatchDTO ocatch = newOuting.Catches.Where(x => x.FishType.Value == newDbOutingCatch.FishId).Single();
                Assert.Equal(newDbOuting.Id, newDbOutingCatch.ScientificPermitOutingId);
                Assert.Equal(ocatch.FishType.Value, newDbOutingCatch.Fish.Id);
                Assert.Equal(ocatch.FishType.DisplayName, newDbOutingCatch.Fish.Name);
                Assert.Equal(ocatch.CatchUnder100, newDbOutingCatch.CatchUnder100);
                Assert.Equal(ocatch.Catch100To500, newDbOutingCatch.Catch100To500);
                Assert.Equal(ocatch.Catch500To1000, newDbOutingCatch.Catch500To1000);
                Assert.Equal(ocatch.CatchOver1000, newDbOutingCatch.CatchOver1000);
                Assert.Equal(ocatch.TotalKeptCount, newDbOutingCatch.TotalKeptCount);
                Assert.True(newDbOutingCatch.IsActive);
            }
        }

        [Fact(DisplayName = "Извличане на разрешително")]
        public void TestGetPermit()
        {
            ScientificPermitRegister permit = db.ScientificPermitRegisters
                                    .Where(x => x.Id == ScientificPermitData.ScientificPermitRegisters[1].Id)
                                    .Single();
            //ScientificFishingPermitEditDTO result = service.GetPermit(permit.Id);

            //AssertMatching(permit, result);
        }

        //[Fact(DisplayName = "Извличане на всички активни разрешителни")]
        //public void TestGetAllActivePermits()
        //{
        //    ScientificFishingFilters filters = new();
        //    List<ScientificFishingPermitDTO> result = service.GetAllPermits(filters).ToList();

        //    List<ScientificPermitRegister> activeDbRecords = db.ScientificPermitRegisters.Where(x => x.IsActive).ToList();
        //    Assert.Equal(activeDbRecords.Count, result.Count);
        //    foreach (ScientificFishingPermitDTO permit in result)
        //    {
        //        ScientificPermitRegister dbPermit = activeDbRecords.Where(x => x.Id == permit.ID).Single();
        //        AssertMatching(dbPermit, permit);
        //    }
        //}

        //[Fact(DisplayName = "Извличане на всички неактивни разрешителни")]
        //public void TestGetAllInctivePermits()
        //{
        //    ScientificFishingFilters filters = new() { ShowInactiveRecords = true };
        //    List<ScientificFishingPermitDTO> result = service.GetAllPermits(filters).ToList();

        //    List<ScientificPermitRegister> inactiveDbRecords = db.ScientificPermitRegisters.Where(x => !x.IsActive).ToList();
        //    Assert.Equal(inactiveDbRecords.Count, result.Count);
        //    foreach (ScientificFishingPermitDTO permit in result)
        //    {
        //        ScientificPermitRegister dbPermit = inactiveDbRecords.Where(x => x.Id == permit.ID).Single();
        //        AssertMatching(dbPermit, permit);
        //    }
        //}

        //[Fact(DisplayName = "Извличане на разрешителни с филтър по ключова дума")]
        //public void TestGetAllPermitsFreeText()
        //{
        //}

        //[Fact(DisplayName = "Извличане на разрешителни с филтри")]
        //public void TestGetAllPermitsFilters()
        //{
        //}

        [Fact(DisplayName = "Извличане на титуляри за detail row на основната таблица")]
        public void TestGetAllPermitsHolders()
        {
            List<ScientificPermitRegister> activePermits = (from permit in db.ScientificPermitRegisters
                                                                .Include(x => x.ScientificPermitOwners)
                                                            where permit.IsActive
                                                            select permit).ToList();

            List<ScientificPermitOwner> holders = activePermits
                                        .SelectMany(x => x.ScientificPermitOwners.Where(x => x.IsActive).ToList())
                                        .ToList();

            List<int> permitIds = activePermits.Select(x => x.Id).ToList();
            List<ScientificFishingPermitHolderDTO> result = service.GetPermitHoldersForTable(permitIds).ToList();

            Assert.Equal(holders.Count, result.Count);
            foreach (ScientificPermitOwner holder in holders)
            {
                Assert.Contains(holder.Id, result.Select(x => x.ID));
                ScientificFishingPermitHolderDTO resultHolder = result.Where(x => x.ID == holder.Id).Single();

                Assert.Equal(holder.Id, resultHolder.ID);
                Assert.Equal(holder.OwnerId, resultHolder.OwnerID);
                Assert.Equal(holder.ScientificPermit.Id, resultHolder.RequestNumber);
                Assert.Equal(holder.Id, resultHolder.PermitNumber);
                Assert.Equal($"{holder.Owner.FirstName} {holder.Owner.LastName}", resultHolder.Name);
                Assert.Equal(holder.RequestedByOrganizationPosition, resultHolder.ScientificPosition);
            }
        }

        private void SetupDb()
        {
            db.Ncountries.AddRange(AddressData.Countries);
            db.Ndistricts.AddRange(AddressData.Districts);
            db.Nmunicipalities.AddRange(AddressData.Municipalities);
            db.NpopulatedAreas.AddRange(AddressData.PopulatedAreas);
            db.NaddressTypes.AddRange(AddressData.AddressTypes);
            db.Addresses.AddRange(AddressData.Addresses);

            db.Nfishes.AddRange(FishesData.Nfishes);

            db.Legals.AddRange(LegalsData.Legals);

            db.Persons.AddRange(PersonsData.Persons);
            db.PersonAddresses.AddRange(PersonsData.PersonAddresses);
            db.NdocumentTypes.AddRange(PersonsData.NdocumentTypes);
            db.PersonDocuments.AddRange(PersonsData.PersonDocuments);
            db.EmailAddresses.AddRange(PersonsData.EmailAddresses);
            db.PersonEmailAddresses.AddRange(PersonsData.PersonEmailAddresses);
            db.PhoneNumbers.AddRange(PersonsData.PhoneNumbers);
            db.PersonPhoneNumbers.AddRange(PersonsData.PersonPhoneNumbers);

            db.NpermitStatuses.AddRange(ScientificPermitData.NpermitStatuses);
            db.NpermitReasons.AddRange(ScientificPermitData.NpermitReasons);
            db.ScientificPermitRegisters.AddRange(ScientificPermitData.ScientificPermitRegisters);
            db.ScientificPermitReasons.AddRange(ScientificPermitData.ScientificPermitReasons);
            db.ScientificPermitOwners.AddRange(ScientificPermitData.ScientificPermitOwners);
            db.ScientificPermitOutings.AddRange(ScientificPermitData.ScientificPermitOutings);
            db.ScientificPermitOutingCatches.AddRange(ScientificPermitData.ScientificPermitOutingCatches);
            db.SaveChanges();
        }

        private void AssertMatching(ScientificPermitRegister permit, ScientificFishingPermitEditDTO result, bool adding = false)
        {
            if (!adding)
            {
                Assert.Equal(permit.Id, result.ID);
            }

            //Assert.Equal(permit.PermitStatusId, result.PermitStatusId);
            Assert.Equal(permit.PermitRegistrationDateTime, result.RegistrationDate);
            //Assert.Equal(permit.RequestedByPerson.FirstName, result.RequesterFirstName);
            //Assert.Equal(permit.RequestedByPerson.MiddleName, result.RequesterMiddleName);
            //Assert.Equal(permit.RequestedByPerson.LastName, result.RequesterLastName);
            //Assert.Equal(permit.RequestedByPerson.EgnLnc, result.RequesterEgn);
            Assert.Equal(permit.PermitValidFrom, result.ValidFrom);
            Assert.Equal(permit.PermitValidTo, result.ValidTo);
            Assert.Equal(permit.IsAllowedDuringMatingSeason, result.IsAllowedDuringMatingSeason);
            //Assert.Equal(permit.RequestedByOrganizationId, result.RequesterScientificOrganizationId);
            //Assert.Equal(permit.RequestedByOrganizationPosition, result.RequesterPosition);
            Assert.Equal(permit.ResearchPeriodFrom, result.ResearchPeriodFrom);
            Assert.Equal(permit.ResearchPeriodTo, result.ResearchPeriodTo);
            Assert.Equal(permit.ResearchWaterAreas, result.ResearchWaterArea);
            Assert.Equal(permit.ResearchGoalsDesc, result.ResearchGoalsDescription);
            Assert.Equal(permit.FishTypesDesc, result.FishTypesDescription);
            Assert.Equal(permit.FishTypesApp4Zbrdesc, result.FishTypesApp4ZBRDesc);
            Assert.Equal(permit.FishTypesCrayFish, result.FishTypesCrayFish);
            Assert.Equal(permit.FishingGearDescr, result.FishingGearDescription);
            Assert.Equal(permit.IsShipRegistered, result.IsShipRegistered);
            Assert.Equal(permit.ShipName, result.ShipName);
            Assert.Equal(permit.ShipExternalMark, result.ShipExternalMark);
            Assert.Equal(permit.ShipCaptainName, result.ShipCaptainName);
            Assert.Equal(permit.CoordinationCommittee, result.CoordinationCommittee);
            Assert.Equal(permit.CoordinationLetterNo, result.CoordinationLetterNo);
            Assert.Equal(permit.CoordinationDate, result.CoordinationDate);
            //Assert.Equal(permit.CancellationDate, result.CancelationDate);
            //Assert.Equal(permit.CancellationReason, result.CancelationReason);

            db.Entry(permit).Collection(x => x.ScientificPermitReasons).Load();
            Assert.Equal(permit.ScientificPermitReasons.Where(x => x.IsActive).Count(), result.PermitReasonsIds.Count);
            foreach (int reasonId in permit.ScientificPermitReasons.Where(x => x.IsActive).Select(x => x.ReasonId))
            {
                Assert.Contains(reasonId, result.PermitReasonsIds);
            }

            db.Entry(permit).Collection(x => x.ScientificPermitOwners).Load();
            Assert.Equal(permit.ScientificPermitOwners.Count, result.Holders.Count);
            foreach (ScientificPermitOwner holder in permit.ScientificPermitOwners)
            {
                Assert.Contains(holder.Owner.EgnLnc, result.Holders.Select(x => x.RegixPersonData.EgnLnc.EgnLnc));
                ScientificFishingPermitHolderDTO resultHolder = result.Holders.Where(x => x.RegixPersonData.EgnLnc.EgnLnc == holder.Owner.EgnLnc).Single();

                if (!adding)
                {
                    Assert.Equal(holder.Id, resultHolder.ID);
                    Assert.Equal(holder.Id, resultHolder.PermitNumber);
                    Assert.Equal(holder.OwnerId, resultHolder.OwnerID);
                    Assert.Equal(holder.ScientificPermitId, resultHolder.RequestNumber);
                    Assert.Equal($"{holder.Owner.FirstName} {holder.Owner.MiddleName} {holder.Owner.LastName}", resultHolder.Name);
                    Assert.Equal(holder.IsActive, resultHolder.IsActive);
                }
                Assert.Equal(holder.RequestedByOrganizationPosition, resultHolder.ScientificPosition);

                Assert.Equal(holder.Owner.EgnLnc, resultHolder.RegixPersonData.EgnLnc.EgnLnc);
                Assert.Equal(holder.Owner.FirstName, resultHolder.RegixPersonData.FirstName);
                Assert.Equal(holder.Owner.MiddleName, resultHolder.RegixPersonData.MiddleName);
                Assert.Equal(holder.Owner.LastName, resultHolder.RegixPersonData.LastName);
                Assert.Equal(holder.Owner.CitizenshipCountryId, resultHolder.RegixPersonData.CitizenshipCountryId);
                Assert.Equal(holder.Owner.BirthDate, resultHolder.RegixPersonData.BirthDate);
                Assert.Equal(holder.Owner.HasBulgarianAddressRegistration, resultHolder.RegixPersonData.HasBulgarianAddressRegistration);

                db.Entry(holder.Owner).Collection(x => x.PersonDocuments).Load();
                PersonDocument document = holder.Owner.PersonDocuments.Where(x => x.IsActive).Single();
                Assert.Equal(document.DocumentTypeId, resultHolder.RegixPersonData.Document.DocumentTypeID);
                Assert.Equal(document.DocumentNumber, resultHolder.RegixPersonData.Document.DocumentNumber);
                Assert.Equal(document.DocumentIssueDate, resultHolder.RegixPersonData.Document.DocumentIssuedOn);
                Assert.Equal(document.DocumentPublisher, resultHolder.RegixPersonData.Document.DocumentIssuedBy);

                db.Entry(holder.Owner).Collection(x => x.PersonPhoneNumbers).Load();
                string phone = holder.Owner.PersonPhoneNumbers.Where(x => x.IsActive).SingleOrDefault()?.Phone?.Phone;
                Assert.Equal(phone, resultHolder.RegixPersonData.Phone);

                db.Entry(holder.Owner).Collection(x => x.PersonEmailAddresses).Load();
                string email = holder.Owner.PersonEmailAddresses.Where(x => x.IsActive).SingleOrDefault()?.EmailAddress?.Email;
                Assert.Equal(email, resultHolder.RegixPersonData.Email);

                db.Entry(holder.Owner).Collection(x => x.PersonAddresses).Load();
                Assert.Equal(holder.Owner.PersonAddresses.Count, resultHolder.AddressRegistrations.Count);
                foreach (PersonAddress address in holder.Owner.PersonAddresses.Where(x => x.IsActive))
                {
                    Assert.Contains(address.AddressType.Code, resultHolder.AddressRegistrations.Select(x => x.AddressType.ToString()));
                    AddressRegistrationDTO resultAddress = resultHolder.AddressRegistrations.Where(x => x.AddressType.ToString() == address.AddressType.Code).Single();

                    Assert.Equal(address.AddressType.Code, resultAddress.AddressType.ToString());
                    Assert.Equal(address.Address.CountryId, resultAddress.CountryId);
                    Assert.Equal(address.Address.DistrictId, resultAddress.DistrictId);
                    Assert.Equal(address.Address.MunicipalityId, resultAddress.MunicipalityId);
                    Assert.Equal(address.Address.PopulatedAreaId, resultAddress.PopulatedAreaId);
                    Assert.Equal(address.Address.Region, resultAddress.Region);
                    Assert.Equal(address.Address.PostCode, resultAddress.PostalCode);
                    Assert.Equal(address.Address.Street, resultAddress.Street);
                    Assert.Equal(address.Address.StreetNum, resultAddress.StreetNum);
                    Assert.Equal(address.Address.BlockNum, resultAddress.BlockNum);
                    Assert.Equal(address.Address.EntranceNum, resultAddress.EntranceNum);
                    Assert.Equal(address.Address.FloorNum, resultAddress.FloorNum);
                    Assert.Equal(address.Address.ApartmentNum, resultAddress.ApartmentNum);
                }
            }

            if (!adding)
            {
                db.Entry(permit).Collection(x => x.ScientificPermitOutings).Load();
                Assert.Equal(permit.ScientificPermitOutings.Count, result.Outings.Count);
                foreach (ScientificPermitOuting outing in permit.ScientificPermitOutings)
                {
                    Assert.Contains(outing.Id, result.Outings.Select(x => x.ID));
                    ScientificFishingOutingDTO resultOuting = result.Outings.Where(x => x.ID == outing.Id).Single();

                    Assert.Equal(outing.Id, resultOuting.ID);
                    Assert.Equal(outing.ScientificPermitId, resultOuting.PermitID);
                    Assert.Equal(outing.OutingDate, resultOuting.DateOfOuting);
                    Assert.Equal(outing.WaterAreaDesc, resultOuting.WaterArea);
                    Assert.Equal(outing.IsActive, resultOuting.IsActive);

                    db.Entry(outing).Collection(x => x.ScientificPermitOutingCatches).Load();
                    Assert.Equal(outing.ScientificPermitOutingCatches.Count, resultOuting.Catches.Count);
                    foreach (ScientificPermitOutingCatch ocatch in outing.ScientificPermitOutingCatches)
                    {
                        Assert.Contains(ocatch.Id, resultOuting.Catches.Select(x => x.Id));
                        ScientificFishingOutingCatchDTO resultCatch = resultOuting.Catches.Where(x => x.Id == ocatch.Id).Single();

                        Assert.Equal(ocatch.Id, resultCatch.Id);
                        Assert.Equal(ocatch.ScientificPermitOutingId, resultCatch.OutingId);
                        Assert.Equal(ocatch.Fish.Id, resultCatch.FishType.Value);
                        Assert.Equal(ocatch.Fish.Name, resultCatch.FishType.DisplayName);
                        Assert.Equal(ocatch.CatchUnder100, resultCatch.CatchUnder100);
                        Assert.Equal(ocatch.Catch100To500, resultCatch.Catch100To500);
                        Assert.Equal(ocatch.Catch500To1000, resultCatch.Catch500To1000);
                        Assert.Equal(ocatch.CatchOver1000, resultCatch.CatchOver1000);
                        Assert.Equal(ocatch.TotalKeptCount, resultCatch.TotalKeptCount);

                        int totalCatch = ocatch.CatchUnder100 + ocatch.Catch100To500 + ocatch.Catch500To1000 + ocatch.CatchOver1000;
                        Assert.Equal(totalCatch, resultCatch.TotalCatch);
                        Assert.Equal(ocatch.IsActive, resultCatch.IsActive);
                    }
                }
            }
        }

        //private void AssertMatching(ScientificPermitRegister permit, ScientificFishingPermitDTO result)
        //{
        //    Assert.Equal(permit.Id, result.ID);
        //    Assert.Equal(permit.Id, result.RequestNumber);
        //    Assert.Equal($"{permit.RequestedByFirstName} {permit.RequestedByLastName}", result.RequesterName);
        //    Assert.Equal(permit.RequestedByOrganization.Name, result.ScientificOrganizationName);
        //    Assert.Equal(permit.CoordinationDate, result.ValidFrom);
        //    Assert.Equal(permit.RequestStatus.Code, result.RequestStatus.ToString());
        //    Assert.Equal(permit.IsActive, result.IsActive);

        //    db.Entry(permit).Collection(x => x.ScientificPermitOutings).Load();
        //    Assert.Equal(permit.ScientificPermitOutings.Count, result.OutingsCount);

        //    db.Entry(permit).Collection(x => x.ScientificPermitReasons).Load();
        //    string reasons = string.Join("; ", permit.ScientificPermitReasons.Where(x => x.IsActive).Select(x => x.Reason).Select(x => x.Name));
        //    Assert.Equal(reasons, result.PermitReasons);
        //}

        private void CopyEntityToDTO(ScientificPermitRegister permit, out ScientificFishingPermitEditDTO result)
        {
            db.Entry(permit).Collection(x => x.ScientificPermitReasons).Load();
            db.Entry(permit).Collection(x => x.ScientificPermitOwners).Load();
            db.Entry(permit).Collection(x => x.ScientificPermitOutings).Load();

            result = new ScientificFishingPermitEditDTO
            {
                ID = permit.Id,
                //PermitStatusId = permit.PermitStatusId,
                RegistrationDate = permit.PermitRegistrationDateTime,
                ValidFrom = permit.PermitValidFrom,
                ValidTo = permit.PermitValidTo,
                PermitReasonsIds = permit.ScientificPermitReasons.Where(x => x.IsActive).Select(x => x.ReasonId).ToList(),
                IsAllowedDuringMatingSeason = permit.IsAllowedDuringMatingSeason,
                //RequesterFirstName = permit.RequestedByPerson.FirstName,
                //RequesterMiddleName = permit.RequestedByPerson.MiddleName,
                //RequesterLastName = permit.RequestedByPerson.LastName,
                //RequesterEgn = permit.RequestedByPerson.EgnLnc,
                //RequesterScientificOrganizationId = permit.RequestedByOrganizationId,
                //RequesterPosition = permit.RequestedByOrganizationPosition,
                Holders = (from holder in permit.ScientificPermitOwners
                           select new ScientificFishingPermitHolderDTO
                           {
                               ID = holder.Id,
                               OwnerID = holder.OwnerId,
                               RequestNumber = permit.Id,
                               PermitNumber = holder.Id,
                               Name = $"{holder.Owner.FirstName} {holder.Owner.MiddleName} {holder.Owner.LastName}",
                               ScientificPosition = holder.RequestedByOrganizationPosition,
                               IsActive = holder.IsActive,
                               RegixPersonData = new RegixPersonDataDTO
                               {
                                   EgnLnc = new EgnLncDTO
                                   {
                                       EgnLnc = holder.Owner.EgnLnc
                                   },
                                   FirstName = holder.Owner.FirstName,
                                   MiddleName = holder.Owner.MiddleName,
                                   LastName = holder.Owner.LastName,
                                   Document = new PersonDocumentDTO
                                   {
                                       DocumentTypeID = holder.Owner.PersonDocuments.Where(x => x.IsActive).Single().DocumentTypeId,
                                       DocumentNumber = holder.Owner.PersonDocuments.Where(x => x.IsActive).Single().DocumentNumber,
                                       DocumentIssuedOn = holder.Owner.PersonDocuments.Where(x => x.IsActive).Single().DocumentIssueDate,
                                       DocumentIssuedBy = holder.Owner.PersonDocuments.Where(x => x.IsActive).Single().DocumentPublisher
                                   },
                                   CitizenshipCountryId = holder.Owner.CitizenshipCountryId,
                                   Phone = holder.Owner.PersonPhoneNumbers.Where(x => x.IsActive).FirstOrDefault()?.Phone?.Phone,
                                   Email = holder.Owner.PersonEmailAddresses.Where(x => x.IsActive).FirstOrDefault()?.EmailAddress?.Email,
                                   BirthDate = holder.Owner.BirthDate,
                                   HasBulgarianAddressRegistration = holder.Owner.HasBulgarianAddressRegistration
                               },
                               AddressRegistrations = (from address in holder.Owner.PersonAddresses
                                                       select new AddressRegistrationDTO
                                                       {
                                                           AddressType = Enum.Parse<AddressTypesEnum>(address.AddressType.Code),
                                                           CountryId = address.Address.CountryId.Value,
                                                           DistrictId = address.Address.DistrictId,
                                                           MunicipalityId = address.Address.MunicipalityId,
                                                           PopulatedAreaId = address.Address.PopulatedAreaId,
                                                           Region = address.Address.Region,
                                                           PostalCode = address.Address.PostCode,
                                                           Street = address.Address.Street,
                                                           StreetNum = address.Address.StreetNum,
                                                           BlockNum = address.Address.BlockNum,
                                                           EntranceNum = address.Address.EntranceNum,
                                                           FloorNum = address.Address.FloorNum,
                                                           ApartmentNum = address.Address.ApartmentNum
                                                       }).ToList()
                           }).ToList(),
                ResearchPeriodFrom = permit.ResearchPeriodFrom,
                ResearchPeriodTo = permit.ResearchPeriodTo,
                ResearchWaterArea = permit.ResearchWaterAreas,
                ResearchGoalsDescription = permit.ResearchGoalsDesc,
                FishTypesDescription = permit.FishTypesDesc,
                FishTypesApp4ZBRDesc = permit.FishTypesApp4Zbrdesc,
                FishTypesCrayFish = permit.FishTypesCrayFish,
                FishingGearDescription = permit.FishingGearDescr,
                IsShipRegistered = permit.IsShipRegistered,
                ShipID = permit.ShipId,
                ShipName = permit.ShipName,
                ShipExternalMark = permit.ShipExternalMark,
                ShipCaptainName = permit.ShipCaptainName,
                Outings = (from outing in permit.ScientificPermitOutings
                           select new ScientificFishingOutingDTO
                           {
                               ID = outing.Id,
                               PermitID = outing.ScientificPermitId,
                               DateOfOuting = outing.OutingDate,
                               WaterArea = outing.WaterAreaDesc,
                               Catches = (from ocatch in outing.ScientificPermitOutingCatches
                                          select new ScientificFishingOutingCatchDTO
                                          {
                                              Id = ocatch.Id,
                                              OutingId = ocatch.ScientificPermitOutingId,
                                              FishType = new NomenclatureDTO
                                              {
                                                  Value = ocatch.FishId,
                                                  DisplayName = ocatch.Fish.Name
                                              },
                                              CatchUnder100 = ocatch.CatchUnder100,
                                              Catch100To500 = ocatch.Catch100To500,
                                              Catch500To1000 = ocatch.Catch500To1000,
                                              CatchOver1000 = ocatch.CatchOver1000,
                                              TotalKeptCount = ocatch.TotalKeptCount,
                                              TotalCatch = ocatch.CatchUnder100 + ocatch.Catch100To500 + ocatch.Catch500To1000 + ocatch.CatchOver1000,
                                              IsActive = ocatch.IsActive
                                          }).ToList(),
                               IsActive = outing.IsActive
                           }).ToList(),
                CoordinationCommittee = permit.CoordinationCommittee,
                CoordinationLetterNo = permit.CoordinationLetterNo,
                CoordinationDate = permit.CoordinationDate,
                //CancelationDate = permit.CancellationDate,
                //CancelationReason = permit.CancellationReason
            };
        }

        private void EditDTO(ref ScientificFishingPermitEditDTO permit)
        {
            permit.ValidFrom = permit.ValidFrom.AddDays(1);
            permit.ValidTo = permit.ValidTo.AddDays(-1);
            permit.PermitReasonsIds = new List<int> { ScientificPermitData.NpermitReasons[0].Id, ScientificPermitData.NpermitReasons[2].Id };
            permit.IsAllowedDuringMatingSeason = !permit.IsAllowedDuringMatingSeason;
            //permit.RequesterFirstName = "test first name";
            //permit.RequesterMiddleName = "test middle name";
            //permit.RequesterLastName = "test last name";
            //permit.RequesterEgn = "555123674586625";
            //permit.RequesterScientificOrganizationId = LegalsData.Legals[2].Id;
            //permit.RequesterPosition = "position test";
            permit.ResearchPeriodFrom = permit.ResearchPeriodFrom.AddDays(1);
            permit.ResearchPeriodTo = permit.ResearchPeriodTo.AddDays(-1);
            permit.ResearchWaterArea = "test water area";
            permit.ResearchGoalsDescription = "test research goals";
            permit.FishTypesDescription = null;
            permit.FishTypesApp4ZBRDesc = "test fishes";
            permit.FishTypesCrayFish = null;
            permit.FishingGearDescription = "test fishing gear";

            permit.Holders[0].IsActive = false;
            permit.Holders[1].ScientificPosition = "test holder position";
            permit.Holders[1].RegixPersonData.Phone = "0882346123541515";
            permit.Holders[1].RegixPersonData.Email = "test.email-edit.permit@email.com";

            permit.Outings[0].WaterArea = "editted water area";
            permit.Outings[0].Catches[0].IsActive = false;
            permit.Outings[0].Catches[1].TotalKeptCount = 20;
            permit.Outings[1].IsActive = false;
        }
    }
}
