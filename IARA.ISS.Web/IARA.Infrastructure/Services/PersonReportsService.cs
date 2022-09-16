using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Reports;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces;
using IARA.Interfaces.Legals;

namespace IARA.Infrastructure.Services
{
    public class PersonReportsService : Service, IPersonReportsService
    {
        private readonly IPersonService personService;
        private readonly ILegalService legalService;

        public PersonReportsService(IARADbContext db, IPersonService personService, ILegalService legalService)
            : base(db)
        {
            this.personService = personService;
            this.legalService = legalService;
        }

        public IQueryable<LegalEntityReportDTO> GetAllLegalEntitiesReport(LegalEntitiesReportFilters filters)
        {
            IQueryable<LegalEntityReportDTO> result;
            if (filters == null || !filters.HasAnyFilters(false))
            {
                bool showInactive = filters?.ShowInactiveRecords ?? false;

                result = GetLegalEntitiesReport(showInactive);
            }
            else
            {
                result = filters.HasAnyFilters()
                    ? GetParametersFilteredLegalEntitiesReport(filters)
                    : GetFreeTextFilteredLegalEntitiesReport(filters.FreeTextSearch);
            }

            return result;
        }

        public IQueryable<PersonReportDTO> GetAllPersonsReport(PersonsReportFilters filters)
        {
            IQueryable<PersonReportDTO> result;
            if (filters == null || !filters.HasAnyFilters(false))
            {
                result = GetPersonsReport();
            }
            else
            {
                result = filters.HasAnyFilters()
                    ? GetParametersFilteredPersonsReport(filters)
                    : GetFreeTextFilteredPersonsReport(filters.FreeTextSearch);
            }

            return result;
        }

        public LegalEntityReportInfoDTO GetLegalEntityReport(int id)
        {
            LegalEntityReportInfoDTO legalEntityReport = (from legal in Db.Legals
                                                          where legal.Id == id
                                                          select new LegalEntityReportInfoDTO
                                                          {
                                                              Id = legal.Id,
                                                              Eik = legal.Eik,
                                                              LegalName = legal.Name
                                                          }).First();

            legalEntityReport.Email = legalService.GetLegalEmail(id);
            legalEntityReport.Phone = legalService.GetLegalPhoneNumber(id);
            legalEntityReport.PostalCode = legalService.GetLegalPostalCode(id);
            legalEntityReport.CorrespondenceAddress = legalService.GetAddressRegistrations(id).Where(x => x.AddressType == AddressTypesEnum.CORRESPONDENCE).FirstOrDefault();
            legalEntityReport.CourtRegistrationAddress = legalService.GetAddressRegistrations(id).Where(x => x.AddressType == AddressTypesEnum.COURT_REGISTRATION).FirstOrDefault();

            return legalEntityReport;
        }

        public PersonReportInfoDTO GetPersonReport(int id)
        {
            PersonReportInfoDTO personReport = (from person in Db.Persons
                                                where person.Id == id
                                                select new PersonReportInfoDTO
                                                {
                                                    Id = person.Id,
                                                    Comments = person.Comments
                                                }).First();

            personReport.RegixPersonData = personService.GetRegixPersonData(id);
            personReport.AddressRegistrations = personService.GetAddressRegistrations(id);

            return personReport;
        }

        public List<ReportHistoryDTO> GetPeopleHistory(IEnumerable<int> validPeopleIds)
        {
            List<string> peopleEGNs = (from person in Db.Persons
                                       where validPeopleIds.Contains(person.Id)
                                       select person.EgnLnc).ToList();

            List<int> peopleIds = (from p in Db.Persons
                                   where peopleEGNs.Contains(p.EgnLnc)
                                   select p.Id).ToList();

            List<ReportHistoryDTO> allRecordsForPerson = new List<ReportHistoryDTO>();

            allRecordsForPerson.AddRange(GetScientificPermitRegisterRecords(peopleIds));
            allRecordsForPerson.AddRange(GetFishermenRegisterRecords(peopleIds));
            allRecordsForPerson.AddRange(GetApplicationRegisterRecordsBy(peopleIds));
            allRecordsForPerson.AddRange(GetApplicationRegisterRecordsFor(peopleIds));
            allRecordsForPerson.AddRange(GetAquacultureFacilityRegisterRecords(peopleIds));
            allRecordsForPerson.AddRange(GetAquacultureLogBookPageRecords(peopleIds));
            allRecordsForPerson.AddRange(GetAuanregisterInspectedPeopleRecords(peopleIds));
            allRecordsForPerson.AddRange(GetBuyerRegisterAgentsRecords(peopleIds));
            allRecordsForPerson.AddRange(GetBuyerRegisterOrganizingPeopleRecords(peopleIds));
            allRecordsForPerson.AddRange(GetBuyerRegisterSubmittedForPeopleRecords(peopleIds));
            allRecordsForPerson.AddRange(GetCapacityCertificatesRegistersRecords(peopleIds));
            allRecordsForPerson.AddRange(GetFishingAssociationMembersRecords(peopleIds));
            allRecordsForPerson.AddRange(GetFishingTicketPeopleRecords(peopleIds));
            allRecordsForPerson.AddRange(GetFishingTicketPersonRepresentativesRecords(peopleIds));
            allRecordsForPerson.AddRange(GetInspectedPeopleRecords(peopleIds));
            allRecordsForPerson.AddRange(GetLogBooksRecords(peopleIds));
            allRecordsForPerson.AddRange(GetPenalPointsRegistersRecords(peopleIds));
            allRecordsForPerson.AddRange(GetPermitLicensesRegistersRecords(peopleIds));
            allRecordsForPerson.AddRange(GetPermitRegistersRecords(peopleIds));
            allRecordsForPerson.AddRange(GetShipOwnersRecords(peopleIds));
            allRecordsForPerson.AddRange(GetStatisticalFormsRegistersRecords(peopleIds));
            allRecordsForPerson.AddRange(GetBuyerApplicationChangeOfCircumstancesRecordsForPerson(peopleIds));
            allRecordsForPerson.AddRange(GetAquacultureApplicationChangeOfCircumstancesRecordsForPerson(peopleIds));
            allRecordsForPerson.AddRange(GetShipApplicationChangeOfCircumstancesRecordsForPerson(peopleIds));
            allRecordsForPerson.AddRange(GetRecordsForPeople(peopleIds));

            return allRecordsForPerson.OrderByDescending(x => x.ValidTo).ToLookup(x => x.Id).Select(x => x.First()).ToList();
        }

        public List<ReportHistoryDTO> GetLegalEntitiesHistory(IEnumerable<int> ids)
        {
            List<ReportHistoryDTO> allRecords = new List<ReportHistoryDTO>();

            allRecords.AddRange(GetScientificPermitRegisterRecordsForLegal(ids));
            allRecords.AddRange(GetApplicationRegisterRecordsForLegal(ids));
            allRecords.AddRange(GetBuyerApplicationChangeOfCircumstancesRecordsForLegal(ids));
            allRecords.AddRange(GetAquacultureApplicationChangeOfCircumstancesRecordsForLegal(ids));
            allRecords.AddRange(GetShipApplicationChangeOfCircumstancesRecordsForLegal(ids));
            allRecords.AddRange(GetAquacultureFacilityRegisterRecordsForLegal(ids));
            allRecords.AddRange(GetAquacultureLogBookPagesRecordsForLegal(ids));
            allRecords.AddRange(GetAuanregisterRecordsForLegal(ids));
            allRecords.AddRange(GetBuyerRegisterRecordsForLegal(ids));
            allRecords.AddRange(GetCapacityCertificatesRegisterRecordsForLegal(ids));
            allRecords.AddRange(GetInspectedPeopleRecordsForLegal(ids));
            allRecords.AddRange(GetLogBookRecordsForLegal(ids));
            allRecords.AddRange(GetPenalPointsRegistersRecordsForLegal(ids));
            allRecords.AddRange(GetPermitLicensesRegisterRecordsForLegal(ids));
            allRecords.AddRange(GetPermitRegisterRecordsForLegal(ids));
            allRecords.AddRange(GetShipOwnerRecordsForLegal(ids));
            allRecords.AddRange(GetStatisticalFormsRegisterRecordsForLegal(ids));
            allRecords.AddRange(GetUserLegalRecordsForLegal(ids));
            allRecords.AddRange(GetAllRecordsForLegal(ids));

            return allRecords.OrderByDescending(x => x.ValidTo).ToLookup(x => x.Id).Select(x => x.First()).ToList();
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            throw new NotImplementedException();
        }

        private IQueryable<PersonReportDTO> GetPersonsReport()
        {
            DateTime now = DateTime.Now;

            IQueryable<PersonReportDTO> result = from person in Db.Persons
                                                 join personAddresses in Db.PersonAddresses.Where(x => x.IsActive)
                                                                      on person.Id
                                                                      equals personAddresses.PersonId
                                                                      into personAddressGroup
                                                 from personAddress in personAddressGroup.DefaultIfEmpty()
                                                 join addresses in Db.Addresses.Where(x => x.IsActive)
                                                                on personAddress.AddressId
                                                                equals addresses.Id
                                                                into addressesGroup
                                                 from addresses in addressesGroup.DefaultIfEmpty()
                                                 join populatedArea in Db.NpopulatedAreas.Where(x => x.ValidFrom <= now && x.ValidTo >= now)
                                                                    on addresses.PopulatedAreaId
                                                                    equals populatedArea.Id
                                                                    into populatedAreasGroup
                                                 from populatedArea in populatedAreasGroup.DefaultIfEmpty()
                                                 where person.ValidFrom <= now && person.ValidTo >= now
                                                 orderby person.FirstName
                                                 select new PersonReportDTO
                                                 {
                                                     Id = person.Id,
                                                     FullName = person.MiddleName != null
                                                        ? person.FirstName + " " + person.MiddleName + " " + person.LastName
                                                        : person.FirstName + " " + person.LastName,
                                                     EGN = person.EgnLnc,
                                                     PopulatedArea = populatedArea != null ? populatedArea.Name : string.Empty,
                                                     Street = addresses != null ? addresses.Street : string.Empty,
                                                     Number = addresses != null ? addresses.BlockNum : string.Empty
                                                 };

            return result;
        }

        private IQueryable<PersonReportDTO> GetParametersFilteredPersonsReport(PersonsReportFilters filters)
        {
            DateTime now = DateTime.Now;

            var query = from person in Db.Persons
                        join personAddresses in Db.PersonAddresses.Where(x => x.IsActive)
                                             on person.Id
                                             equals personAddresses.PersonId
                                             into personAddressGroup
                        from personAddress in personAddressGroup.DefaultIfEmpty()
                        join addresses in Db.Addresses.Where(x => x.IsActive)
                                       on personAddress.AddressId
                                       equals addresses.Id
                                       into addressesGroup
                        from address in addressesGroup.DefaultIfEmpty()
                        join populatedArea in Db.NpopulatedAreas.Where(x => x.ValidFrom <= now && x.ValidTo >= now)
                                           on address.PopulatedAreaId
                                           equals populatedArea.Id
                                           into populatedAreasGroup
                        from populatedArea in populatedAreasGroup.DefaultIfEmpty()
                        where person.ValidFrom <= now && person.ValidTo >= now
                        select new
                        {
                            person.Id,
                            person.FirstName,
                            person.MiddleName,
                            person.LastName,
                            person.EgnLnc,
                            address,
                            populatedArea
                        };

            if (!string.IsNullOrEmpty(filters.FirstName))
            {
                query = query.Where(x => x.FirstName.ToLower().Contains(filters.FirstName.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.MiddleName))
            {
                query = query.Where(x => x.MiddleName.ToLower().Contains(filters.MiddleName.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.LastName))
            {
                query = query.Where(x => x.LastName.ToLower().Contains(filters.LastName.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.EGN))
            {
                query = query.Where(x => x.EgnLnc == filters.EGN);
            }

            if (filters.CountryId.HasValue)
            {
                query = query.Where(x => x.address.CountryId == filters.CountryId);
            }

            if (filters.PopulatedAreaId.HasValue)
            {
                query = query.Where(x => x.populatedArea.Id == filters.PopulatedAreaId);
            }

            return from data in query
                   orderby data.FirstName
                   select new PersonReportDTO
                   {
                       Id = data.Id,
                       FullName = data.MiddleName != null
                            ? data.FirstName + " " + data.MiddleName + " " + data.LastName
                            : data.FirstName + " " + data.LastName,
                       EGN = data.EgnLnc,
                       PopulatedArea = data.populatedArea != null ? data.populatedArea.Name : string.Empty,
                       Street = data.address != null ? data.address.Street : string.Empty,
                       Number = data.address != null ? data.address.BlockNum : string.Empty
                   };
        }

        private IQueryable<PersonReportDTO> GetFreeTextFilteredPersonsReport(string text)
        {
            text = text.ToLowerInvariant();
            DateTime now = DateTime.Now;

            return from person in Db.Persons
                   join personAddresses in Db.PersonAddresses.Where(x => x.IsActive)
                                        on person.Id
                                        equals personAddresses.PersonId
                                        into personAddressGroup
                   from personAddress in personAddressGroup.DefaultIfEmpty()
                   join addresses in Db.Addresses.Where(x => x.IsActive)
                                  on personAddress.AddressId
                                  equals addresses.Id
                                  into addressesGroup
                   from address in addressesGroup.DefaultIfEmpty()
                   join populatedArea in Db.NpopulatedAreas.Where(x => x.ValidFrom <= now && x.ValidTo >= now)
                                      on address.PopulatedAreaId
                                      equals populatedArea.Id
                                      into populatedAreasGroup
                   from populatedArea in populatedAreasGroup.DefaultIfEmpty()
                   where person.EgnLnc == text
                                    || person.FirstName.ToLower().Contains(text)
                                    || person.MiddleName.ToLower().Contains(text)
                                    || person.LastName.ToLower().Contains(text)
                                    || address.Country.Name.ToLower().Contains(text)
                                    || populatedArea.Name.ToLower().Contains(text)
                                    || address.Street.ToLower().Contains(text)
                   where person.ValidFrom <= now && person.ValidTo >= now
                   select new PersonReportDTO
                   {
                       Id = person.Id,
                       FullName = person.MiddleName != null
                            ? person.FirstName + " " + person.MiddleName + " " + person.LastName
                            : person.FirstName + " " + person.LastName,
                       EGN = person.EgnLnc,
                       PopulatedArea = populatedArea != null ? populatedArea.Name : string.Empty,
                       Street = address != null ? address.Street : string.Empty,
                       Number = address != null ? address.BlockNum : string.Empty,
                   };
        }

        private IQueryable<LegalEntityReportDTO> GetLegalEntitiesReport(bool showInactive)
        {
            DateTime now = DateTime.Now;

            return from legal in Db.Legals
                   join legalAddresses in Db.LegalsAddresses.Where(x => x.IsActive)
                                       on legal.Id
                                       equals legalAddresses.LegalId
                                       into legalAddressGroup
                   from legalAddress in legalAddressGroup.DefaultIfEmpty()
                   join addresses in Db.Addresses.Where(x => x.IsActive)
                                  on legalAddress.AddressId
                                  equals addresses.Id
                                  into addressesGroup
                   from address in addressesGroup.DefaultIfEmpty()
                   join populatedAreas in Db.NpopulatedAreas.Where(x => x.ValidFrom <= now && x.ValidTo >= now)
                                       on address.PopulatedAreaId
                                       equals populatedAreas.Id
                                       into populatedAreasGroup
                   from populatedArea in populatedAreasGroup.DefaultIfEmpty()
                   where legal.ValidFrom <= now && legal.ValidTo >= now && legal.RecordType == RecordTypesEnum.Register.ToString()
                   select new LegalEntityReportDTO
                   {
                       Id = legal.Id,
                       LegalName = legal.Name,
                       Eik = legal.Eik,
                       PopulatedArea = populatedArea != null ? populatedArea.Name : string.Empty,
                       Street = address != null ? address.Street : string.Empty,
                       Number = address != null ? address.BlockNum : string.Empty
                   };
        }

        private IQueryable<LegalEntityReportDTO> GetParametersFilteredLegalEntitiesReport(LegalEntitiesReportFilters filters)
        {
            DateTime now = DateTime.Now;

            var query = from legal in Db.Legals
                        join legalAddresses in Db.LegalsAddresses.Where(x => x.IsActive)
                                            on legal.Id
                                            equals legalAddresses.LegalId
                                            into legalAddressgroup
                        from legalAddress in legalAddressgroup.DefaultIfEmpty()
                        join addresses in Db.Addresses.Where(x => x.IsActive)
                                       on legalAddress.AddressId
                                       equals addresses.Id
                                       into addressesGroup
                        from address in addressesGroup.DefaultIfEmpty()
                        join populatedAreas in Db.NpopulatedAreas.Where(x => x.ValidFrom <= now && x.ValidTo >= now)
                                            on address.PopulatedAreaId
                                            equals populatedAreas.Id
                                            into populatedAreasGroup
                        from populatedArea in populatedAreasGroup.DefaultIfEmpty()
                        where legal.RecordType == RecordTypesEnum.Register.ToString()
                        select new
                        {
                            legal.Id,
                            legal.Name,
                            legal.Eik,
                            populatedArea,
                            address
                        };

            if (!string.IsNullOrEmpty(filters.LegalName))
            {
                query = query.Where(x => x.Name.ToLower().Contains(filters.LegalName.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.Eik))
            {
                query = query.Where(x => x.Eik.ToLower().Contains(filters.Eik.ToLower()));
            }

            if (filters.CountryId.HasValue)
            {
                query = query.Where(x => x.address.CountryId == filters.CountryId);
            }

            if (filters.PopulatedAreaId.HasValue)
            {
                query = query.Where(x => x.populatedArea.Id == filters.PopulatedAreaId);
            }

            return from data in query
                   orderby data.Name
                   select new LegalEntityReportDTO
                   {
                       Id = data.Id,
                       LegalName = data.Name,
                       Eik = data.Eik,
                       PopulatedArea = data.populatedArea != null ? data.populatedArea.Name : string.Empty,
                       Street = data.address != null ? data.address.Street : string.Empty,
                       Number = data.address != null ? data.address.BlockNum : string.Empty
                   };
        }

        private IQueryable<LegalEntityReportDTO> GetFreeTextFilteredLegalEntitiesReport(string text)
        {
            text = text.ToLowerInvariant();
            DateTime now = DateTime.Now;

            return from legal in Db.Legals
                   join legalAddresses in Db.LegalsAddresses.Where(x => x.IsActive)
                                       on legal.Id
                                       equals legalAddresses.LegalId
                                       into legalAddressGroup
                   from legalAddress in legalAddressGroup.DefaultIfEmpty()
                   join addresses in Db.Addresses.Where(x => x.IsActive)
                                  on legalAddress.AddressId
                                  equals addresses.Id
                                  into addressesGroup
                   from address in addressesGroup.DefaultIfEmpty()
                   join populatedAreas in Db.NpopulatedAreas.Where(x => x.ValidFrom <= now && x.ValidTo >= now)
                                       on address.PopulatedAreaId
                                       equals populatedAreas.Id
                                       into populatedAreasGroup
                   from populatedArea in populatedAreasGroup.DefaultIfEmpty()
                   where legal.RecordType == RecordTypesEnum.Register.ToString()
                                    && (legal.Eik.ToLower().Contains(text)
                                    || legal.Name.ToLower().Contains(text)
                                    || address.Country.Name.ToLower().Contains(text)
                                    || populatedArea.Name.ToLower().Contains(text)
                                    || address.Street.ToLower().Contains(text))
                   select new LegalEntityReportDTO
                   {
                       Id = legal.Id,
                       LegalName = legal.Name,
                       Eik = legal.Eik,
                       PopulatedArea = populatedArea != null ? populatedArea.Name : string.Empty,
                       Street = address != null ? address.Street : string.Empty,
                       Number = address != null ? address.BlockNum : string.Empty
                   };
        }

        private List<ReportHistoryDTO> GetScientificPermitRegisterRecordsForLegal(IEnumerable<int> legalIds)
        {
            return (from legal in Db.Legals
                    join sPermitRegister in Db.ScientificPermitRegisters on legal.Id equals sPermitRegister.SubmittedForLegalId
                    where legalIds.Contains(sPermitRegister.SubmittedForLegalId)
                    select new ReportHistoryDTO
                    {
                        ValidFrom = legal.ValidFrom,
                        ValidTo = legal.ValidTo,
                        Id = legal.Id,
                        IsPerson = false,
                        PageCode = PageCodeEnum.SciFi
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetApplicationRegisterRecordsForLegal(IEnumerable<int> legalIds)
        {
            return (from legal in Db.Legals
                    join applFor in Db.Applications on legal.Id equals applFor.SubmittedForLegalId
                    where applFor.SubmittedForLegalId.HasValue && legalIds.Contains(applFor.SubmittedForLegalId.Value)
                    select new ReportHistoryDTO
                    {
                        ValidFrom = legal.ValidFrom,
                        ValidTo = legal.ValidTo,
                        IsPerson = false,
                        Id = legal.Id,
                        PageCode = PageCodeEnum.OnlineAppl
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetBuyerApplicationChangeOfCircumstancesRecordsForLegal(IEnumerable<int> legalIds)
        {
            return (from legal in Db.Legals
                    join applFor in Db.ApplicationChangeOfCircumstances on legal.Id equals applFor.LegalId
                    join buyer in Db.BuyerRegisters on applFor.BuyerId equals buyer.Id
                    where applFor.LegalId.HasValue && legalIds.Contains(applFor.LegalId.Value)
                    select new ReportHistoryDTO
                    {
                        ValidFrom = legal.ValidFrom,
                        ValidTo = legal.ValidTo,
                        IsPerson = false,
                        Id = legal.Id,
                        PageCode = PageCodeEnum.ChangeFirstSaleBuyer
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetShipApplicationChangeOfCircumstancesRecordsForLegal(IEnumerable<int> legalIds)
        {
            return (from legal in Db.Legals
                    join applFor in Db.ApplicationChangeOfCircumstances on legal.Id equals applFor.LegalId
                    join ship in Db.ShipsRegister on applFor.ShipId equals ship.Id
                    where applFor.LegalId.HasValue && legalIds.Contains(applFor.LegalId.Value)
                    select new ReportHistoryDTO
                    {
                        ValidFrom = legal.ValidFrom,
                        ValidTo = legal.ValidTo,
                        IsPerson = false,
                        Id = legal.Id,
                        PageCode = PageCodeEnum.ShipRegChange
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetAquacultureApplicationChangeOfCircumstancesRecordsForLegal(IEnumerable<int> legalIds)
        {
            return (from legal in Db.Legals
                    join applFor in Db.ApplicationChangeOfCircumstances on legal.Id equals applFor.LegalId
                    join aquaculture in Db.AquacultureFacilitiesRegister on applFor.AquacultureFacilityId equals aquaculture.Id
                    where applFor.LegalId.HasValue && legalIds.Contains(applFor.LegalId.Value)
                    select new ReportHistoryDTO
                    {
                        ValidFrom = legal.ValidFrom,
                        ValidTo = legal.ValidTo,
                        IsPerson = false,
                        Id = legal.Id,
                        PageCode = PageCodeEnum.AquaFarmChange
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetAquacultureFacilityRegisterRecordsForLegal(IEnumerable<int> legalIds)
        {
            return (from legal in Db.Legals
                    join aquaculture in Db.AquacultureFacilitiesRegister on legal.Id equals aquaculture.SubmittedForLegalId
                    where aquaculture.SubmittedForLegalId.HasValue && legalIds.Contains(aquaculture.SubmittedForLegalId.Value)
                    select new ReportHistoryDTO
                    {
                        ValidFrom = legal.ValidFrom,
                        ValidTo = legal.ValidTo,
                        Id = legal.Id,
                        IsPerson = false,
                        PageCode = PageCodeEnum.AquaFarmReg
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetAquacultureLogBookPagesRecordsForLegal(IEnumerable<int> legalIds)
        {
            return (from legal in Db.Legals
                    join page in Db.AquacultureLogBookPages on legal.Id equals page.LegalBuyerId
                    where page.LegalBuyerId.HasValue && legalIds.Contains(page.LegalBuyerId.Value)
                    select new ReportHistoryDTO
                    {
                        ValidFrom = legal.ValidFrom,
                        ValidTo = legal.ValidTo,
                        Id = legal.Id,
                        IsPerson = false,
                        PageCode = PageCodeEnum.AquacultureLogBookPage
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetAuanregisterRecordsForLegal(IEnumerable<int> legalIds)
        {
            return (from legal in Db.Legals
                        //TODO
                    join auan in Db.AuanRegister on legal.Id equals auan.InspectedLegalId
                    where auan.InspectedLegalId.HasValue && legalIds.Contains(auan.InspectedLegalId.Value)
                    select new ReportHistoryDTO
                    {
                        ValidFrom = legal.ValidFrom,
                        ValidTo = legal.ValidTo,
                        Id = legal.Id,
                        IsPerson = false,
                        PageCode = PageCodeEnum.AuanRegister
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetBuyerRegisterRecordsForLegal(IEnumerable<int> legalIds)
        {
            return (from legal in Db.Legals
                    join buyer in Db.BuyerRegisters on legal.Id equals buyer.SubmittedForLegalId
                    where buyer.SubmittedForLegalId.HasValue && legalIds.Contains(buyer.SubmittedForLegalId.Value)
                    select new ReportHistoryDTO
                    {
                        ValidFrom = legal.ValidFrom,
                        ValidTo = legal.ValidTo,
                        Id = legal.Id,
                        IsPerson = false,
                        PageCode = PageCodeEnum.Buyers
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetCapacityCertificatesRegisterRecordsForLegal(IEnumerable<int> legalIds)
        {
            return (from legal in Db.Legals
                    join capacity in Db.CapacityCertificatesRegister on legal.Id equals capacity.LegalId
                    where capacity.LegalId.HasValue && legalIds.Contains(capacity.LegalId.Value)
                    select new ReportHistoryDTO
                    {
                        ValidFrom = legal.ValidFrom,
                        ValidTo = legal.ValidTo,
                        Id = legal.Id,
                        IsPerson = false,
                        PageCode = PageCodeEnum.IncreaseFishCap
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetInspectedPeopleRecordsForLegal(IEnumerable<int> legalIds)
        {
            return (from legal in Db.Legals
                    join person in Db.InspectedPersons on legal.Id equals person.LegalId
                    where person.LegalId.HasValue && legalIds.Contains(person.LegalId.Value)
                    select new ReportHistoryDTO
                    {
                        ValidFrom = legal.ValidFrom,
                        ValidTo = legal.ValidTo,
                        Id = legal.Id,
                        IsPerson = false,
                        PageCode = PageCodeEnum.Inspections
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetLogBookRecordsForLegal(IEnumerable<int> legalIds)
        {
            return (from legal in Db.Legals
                    join logBook in Db.LogBooks on legal.Id equals logBook.LegalId
                    where logBook.LegalId.HasValue && legalIds.Contains(logBook.LegalId.Value)
                    select new ReportHistoryDTO
                    {
                        ValidFrom = legal.ValidFrom,
                        ValidTo = legal.ValidTo,
                        Id = legal.Id,
                        IsPerson = false,
                        PageCode = PageCodeEnum.AdmissionLogBookPage
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetPenalPointsRegistersRecordsForLegal(IEnumerable<int> legalIds)
        {
            return (from legal in Db.Legals
                    join points in Db.PenalPointsRegister on legal.Id equals points.PointsOwnerLegalId
                    where points.PointsOwnerLegalId.HasValue && legalIds.Contains(points.PointsOwnerLegalId.Value)
                    select new ReportHistoryDTO
                    {
                        ValidFrom = legal.ValidFrom,
                        ValidTo = legal.ValidTo,
                        Id = legal.Id,
                        IsPerson = false,
                        PageCode = PageCodeEnum.PenalDecrees //TODO
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetPermitLicensesRegisterRecordsForLegal(IEnumerable<int> legalIds)
        {
            return (from legal in Db.Legals
                    join permit in Db.CommercialFishingPermitLicensesRegisters on legal.Id equals permit.SubmittedForLegalId
                    where permit.SubmittedForLegalId.HasValue && legalIds.Contains(permit.SubmittedForLegalId.Value)
                    select new ReportHistoryDTO
                    {
                        ValidFrom = legal.ValidFrom,
                        ValidTo = legal.ValidTo,
                        Id = legal.Id,
                        IsPerson = false,
                        PageCode = PageCodeEnum.PoundnetCommFishLic
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetPermitRegisterRecordsForLegal(IEnumerable<int> legalIds)
        {
            return (from legal in Db.Legals
                    join permit in Db.CommercialFishingPermitRegisters on legal.Id equals permit.SubmittedForLegalId
                    where permit.SubmittedForLegalId.HasValue && legalIds.Contains(permit.SubmittedForLegalId.Value)
                    select new ReportHistoryDTO
                    {
                        ValidFrom = legal.ValidFrom,
                        ValidTo = legal.ValidTo,
                        Id = legal.Id,
                        IsPerson = false,
                        PageCode = PageCodeEnum.CommFish
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetShipOwnerRecordsForLegal(IEnumerable<int> legalIds)
        {
            return (from legal in Db.Legals
                    join owner in Db.ShipOwners on legal.Id equals owner.OwnerLegalId
                    where owner.OwnerLegalId.HasValue && legalIds.Contains(owner.OwnerLegalId.Value)
                    select new ReportHistoryDTO
                    {
                        ValidFrom = legal.ValidFrom,
                        ValidTo = legal.ValidTo,
                        Id = legal.Id,
                        IsPerson = false,
                        PageCode = PageCodeEnum.RegVessel
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetStatisticalFormsRegisterRecordsForLegal(IEnumerable<int> legalIds)
        {
            return (from legal in Db.Legals
                    join form in Db.StatisticalFormsRegister on legal.Id equals form.SubmittedForLegalId
                    where form.SubmittedForLegalId.HasValue && legalIds.Contains(form.SubmittedForLegalId.Value)
                    select new ReportHistoryDTO
                    {
                        ValidFrom = legal.ValidFrom,
                        ValidTo = legal.ValidTo,
                        Id = legal.Id,
                        IsPerson = false,
                        PageCode = PageCodeEnum.StatFormAquaFarm
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetUserLegalRecordsForLegal(IEnumerable<int> legalIds)
        {
            return (from legal in Db.Legals
                    join user in Db.UserLegals on legal.Id equals user.LegalId
                    where legalIds.Contains(user.LegalId)
                    select new ReportHistoryDTO
                    {
                        ValidFrom = legal.ValidFrom,
                        ValidTo = legal.ValidTo,
                        IsPerson = false,
                        Id = legal.Id,
                        PageCode = PageCodeEnum.OnlineAppl
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetAllRecordsForLegal(IEnumerable<int> ids)
        {
            return (from legal in Db.Legals
                    where ids.Contains(legal.Id)
                    select new ReportHistoryDTO
                    {
                        ValidFrom = legal.ValidFrom,
                        ValidTo = legal.ValidTo,
                        Id = legal.Id,
                        IsPerson = false,
                        DocumentsName = string.Empty,
                        PageCode = PageCodeEnum.OnlineAppl
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetScientificPermitRegisterRecords(IEnumerable<int> peopleIds)
        {
            return (from p in Db.Persons
                    join scientificPermitOwner in Db.ScientificPermitOwners on p.Id equals scientificPermitOwner.OwnerId
                    join sPermitRegister in Db.ScientificPermitRegisters on scientificPermitOwner.ScientificPermitId equals sPermitRegister.Id
                    where peopleIds.Contains(scientificPermitOwner.OwnerId)
                    select new ReportHistoryDTO
                    {
                        Id = p.Id,
                        ValidFrom = p.ValidFrom,
                        ValidTo = p.ValidTo,
                        EGN = p.EgnLnc,
                        PageCode = PageCodeEnum.SciFi,
                        IsPerson = true
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetFishermenRegisterRecords(IEnumerable<int> peopleIds)
        {
            return (from p in Db.Persons
                    join fishermen in Db.FishermenRegisters on p.Id equals fishermen.PersonId
                    where peopleIds.Contains(fishermen.PersonId)
                    select new ReportHistoryDTO
                    {
                        Id = p.Id,
                        ValidFrom = p.ValidFrom,
                        ValidTo = p.ValidTo,
                        EGN = p.EgnLnc,
                        PageCode = PageCodeEnum.CommFishLicense,
                        IsPerson = true
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetAquacultureFacilityRegisterRecords(IEnumerable<int> peopleIds)
        {
            return (from p in Db.Persons
                    join aquaculture in Db.AquacultureFacilitiesRegister on p.Id equals aquaculture.SubmittedForPersonId
                    join appl in Db.Applications on aquaculture.ApplicationId equals appl.Id
                    where aquaculture.SubmittedForPersonId.HasValue && peopleIds.Contains(aquaculture.SubmittedForPersonId.Value)
                    select new ReportHistoryDTO
                    {
                        Id = p.Id,
                        ValidFrom = p.ValidFrom,
                        ValidTo = p.ValidTo,
                        EGN = p.EgnLnc,
                        IsPerson = true,
                        PageCode = PageCodeEnum.AquaFarmReg
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetAquacultureLogBookPageRecords(IEnumerable<int> peopleIds)
        {
            return (from p in Db.Persons
                    join aquaLogBook in Db.AquacultureLogBookPages on p.Id equals aquaLogBook.PersonBuyerId
                    where aquaLogBook.PersonBuyerId.HasValue && peopleIds.Contains(aquaLogBook.PersonBuyerId.Value)
                    select new ReportHistoryDTO
                    {
                        Id = p.Id,
                        ValidFrom = p.ValidFrom,
                        ValidTo = p.ValidTo,
                        EGN = p.EgnLnc,
                        IsPerson = true,
                        PageCode = PageCodeEnum.AquacultureLogBookPage
                    }).ToList();
        }


        private List<ReportHistoryDTO> GetAuanregisterInspectedPeopleRecords(IEnumerable<int> peopleIds)
        {
            return (from p in Db.Persons
                    join auan in Db.AuanRegister on p.Id equals auan.InspectedPersonId
                    where auan.InspectedPersonId.HasValue && peopleIds.Contains(auan.InspectedPersonId.Value)
                    select new ReportHistoryDTO
                    {
                        Id = p.Id,
                        ValidFrom = p.ValidFrom,
                        ValidTo = p.ValidTo,
                        EGN = p.EgnLnc,
                        IsPerson = true,
                        PageCode = PageCodeEnum.AuanRegister
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetBuyerRegisterAgentsRecords(IEnumerable<int> peopleIds)
        {
            return (from p in Db.Persons
                    join buyer in Db.BuyerRegisters on p.Id equals buyer.AgentId
                    where buyer.AgentId.HasValue && peopleIds.Contains(buyer.AgentId.Value)
                    select new ReportHistoryDTO
                    {
                        Id = p.Id,
                        ValidFrom = p.ValidFrom,
                        ValidTo = p.ValidTo,
                        EGN = p.EgnLnc,
                        IsPerson = true,
                        PageCode = PageCodeEnum.Buyers
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetBuyerRegisterOrganizingPeopleRecords(IEnumerable<int> peopleIds)
        {
            return (from p in Db.Persons
                    join buyer in Db.BuyerRegisters on p.Id equals buyer.OrganizingPersonId
                    where buyer.OrganizingPersonId.HasValue && peopleIds.Contains(buyer.OrganizingPersonId.Value)
                    select new ReportHistoryDTO
                    {
                        Id = p.Id,
                        ValidFrom = p.ValidFrom,
                        ValidTo = p.ValidTo,
                        EGN = p.EgnLnc,
                        IsPerson = true,
                        PageCode = PageCodeEnum.Buyers
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetBuyerRegisterSubmittedForPeopleRecords(IEnumerable<int> peopleIds)
        {
            return (from p in Db.Persons
                    join buyer in Db.BuyerRegisters on p.Id equals buyer.SubmittedForPersonId
                    where buyer.SubmittedForPersonId.HasValue && peopleIds.Contains(buyer.SubmittedForPersonId.Value)
                    select new ReportHistoryDTO
                    {
                        Id = p.Id,
                        ValidFrom = p.ValidFrom,
                        ValidTo = p.ValidTo,
                        EGN = p.EgnLnc,
                        IsPerson = true,
                        PageCode = PageCodeEnum.Buyers
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetCapacityCertificatesRegistersRecords(IEnumerable<int> peopleIds)
        {
            return (from p in Db.Persons
                    join capacity in Db.CapacityCertificatesRegister on p.Id equals capacity.PersonId
                    where capacity.PersonId.HasValue && peopleIds.Contains(capacity.PersonId.Value)
                    select new ReportHistoryDTO
                    {
                        Id = p.Id,
                        ValidFrom = p.ValidFrom,
                        ValidTo = p.ValidTo,
                        EGN = p.EgnLnc,
                        IsPerson = true,
                        PageCode = PageCodeEnum.IncreaseFishCap
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetFishingAssociationMembersRecords(IEnumerable<int> peopleIds)
        {
            return (from p in Db.Persons
                    join member in Db.FishingAssociationMembers on p.Id equals member.PersonId
                    where peopleIds.Contains(member.PersonId)
                    select new ReportHistoryDTO
                    {
                        Id = p.Id,
                        ValidFrom = p.ValidFrom,
                        ValidTo = p.ValidTo,
                        EGN = p.EgnLnc,
                        IsPerson = true,
                        PageCode = PageCodeEnum.Assocs
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetFishingTicketPeopleRecords(IEnumerable<int> peopleIds)
        {
            return (from p in Db.Persons
                    join ticket in Db.FishingTickets on p.Id equals ticket.PersonId
                    where peopleIds.Contains(ticket.PersonId)
                    select new ReportHistoryDTO
                    {
                        Id = p.Id,
                        ValidFrom = p.ValidFrom,
                        ValidTo = p.ValidTo,
                        EGN = p.EgnLnc,
                        IsPerson = true,
                        PageCode = PageCodeEnum.RecFish
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetFishingTicketPersonRepresentativesRecords(IEnumerable<int> peopleIds)
        {
            return (from p in Db.Persons
                    join ticket in Db.FishingTickets on p.Id equals ticket.PersonRepresentativeId
                    where ticket.PersonRepresentativeId.HasValue && peopleIds.Contains(ticket.PersonRepresentativeId.Value)
                    select new ReportHistoryDTO
                    {
                        Id = p.Id,
                        ValidFrom = p.ValidFrom,
                        ValidTo = p.ValidTo,
                        EGN = p.EgnLnc,
                        IsPerson = true,
                        PageCode = PageCodeEnum.RecFish
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetInspectedPeopleRecords(IEnumerable<int> peopleIds)
        {
            return (from p in Db.Persons
                    join inspectedPerson in Db.InspectedPersons on p.Id equals inspectedPerson.PersonId
                    where inspectedPerson.PersonId.HasValue && peopleIds.Contains(inspectedPerson.PersonId.Value)
                    select new ReportHistoryDTO
                    {
                        Id = p.Id,
                        ValidFrom = p.ValidFrom,
                        ValidTo = p.ValidTo,
                        EGN = p.EgnLnc,
                        IsPerson = true,
                        PageCode = PageCodeEnum.Inspections
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetLogBooksRecords(IEnumerable<int> peopleIds)
        {
            return (from p in Db.Persons
                    join logBook in Db.LogBooks on p.Id equals logBook.PersonId
                    where logBook.PersonId.HasValue && peopleIds.Contains(logBook.PersonId.Value)
                    select new ReportHistoryDTO
                    {
                        Id = p.Id,
                        ValidFrom = p.ValidFrom,
                        ValidTo = p.ValidTo,
                        EGN = p.EgnLnc,
                        IsPerson = true,
                        PageCode = PageCodeEnum.AdmissionLogBookPage
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetPenalPointsRegistersRecords(IEnumerable<int> peopleIds)
        {
            return (from p in Db.Persons
                    join points in Db.PenalPointsRegister on p.Id equals points.PointsOwnerPersonId
                    where points.PointsOwnerPersonId.HasValue && peopleIds.Contains(points.PointsOwnerPersonId.Value)
                    select new ReportHistoryDTO
                    {
                        Id = p.Id,
                        ValidFrom = p.ValidFrom,
                        ValidTo = p.ValidTo,
                        EGN = p.EgnLnc,
                        IsPerson = true,
                        PageCode = PageCodeEnum.PenalDecrees //TODO
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetPermitLicensesRegistersRecords(IEnumerable<int> peopleIds)
        {
            return (from p in Db.Persons
                    join permitLicense in Db.CommercialFishingPermitLicensesRegisters on p.Id equals permitLicense.SubmittedForPersonId
                    where permitLicense.SubmittedForPersonId.HasValue && peopleIds.Contains(permitLicense.SubmittedForPersonId.Value)
                    select new ReportHistoryDTO
                    {
                        Id = p.Id,
                        ValidFrom = p.ValidFrom,
                        ValidTo = p.ValidTo,
                        EGN = p.EgnLnc,
                        IsPerson = true,
                        PageCode = PageCodeEnum.PoundnetCommFishLic
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetPermitRegistersRecords(IEnumerable<int> peopleIds)
        {
            return (from p in Db.Persons
                    join permit in Db.CommercialFishingPermitRegisters on p.Id equals permit.SubmittedForPersonId
                    where permit.SubmittedForPersonId.HasValue && peopleIds.Contains(permit.SubmittedForPersonId.Value)
                    select new ReportHistoryDTO
                    {
                        Id = p.Id,
                        ValidFrom = p.ValidFrom,
                        ValidTo = p.ValidTo,
                        EGN = p.EgnLnc,
                        IsPerson = true,
                        PageCode = PageCodeEnum.CommFish
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetShipOwnersRecords(IEnumerable<int> peopleIds)
        {
            return (from p in Db.Persons
                    join shipOwner in Db.ShipOwners on p.Id equals shipOwner.OwnerPersonId
                    where shipOwner.OwnerPersonId.HasValue && peopleIds.Contains(shipOwner.OwnerPersonId.Value)
                    select new ReportHistoryDTO
                    {
                        Id = p.Id,
                        ValidFrom = p.ValidFrom,
                        ValidTo = p.ValidTo,
                        EGN = p.EgnLnc,
                        IsPerson = true,
                        PageCode = PageCodeEnum.RegVessel
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetStatisticalFormsRegistersRecords(IEnumerable<int> peopleIds)
        {
            return (from p in Db.Persons
                    join form in Db.StatisticalFormsRegister on p.Id equals form.SubmittedForPersonId
                    where form.SubmittedForPersonId.HasValue && peopleIds.Contains(form.SubmittedForPersonId.Value)
                    select new ReportHistoryDTO
                    {
                        Id = p.Id,
                        ValidFrom = p.ValidFrom,
                        ValidTo = p.ValidTo,
                        EGN = p.EgnLnc,
                        IsPerson = true,
                        PageCode = PageCodeEnum.StatFormAquaFarm
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetRecordsForPeople(IEnumerable<int> peopleIds)
        {
            return (from p in Db.Persons
                    where peopleIds.Contains(p.Id)
                    select new ReportHistoryDTO
                    {
                        Id = p.Id,
                        ValidFrom = p.ValidFrom,
                        ValidTo = p.ValidTo,
                        EGN = p.EgnLnc,
                        DocumentsName = string.Empty,
                        IsPerson = true,
                        PageCode = PageCodeEnum.OnlineAppl
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetApplicationRegisterRecordsBy(IEnumerable<int> peopleIds)
        {
            return (from p in Db.Persons
                    join applBy in Db.Applications on p.Id equals applBy.SubmittedByPersonId
                    where applBy.SubmittedByPersonId.HasValue && peopleIds.Contains(applBy.SubmittedByPersonId.Value)
                    select new ReportHistoryDTO
                    {
                        Id = p.Id,
                        ValidFrom = p.ValidFrom,
                        ValidTo = p.ValidTo,
                        EGN = p.EgnLnc,
                        IsPerson = true,
                        PageCode = PageCodeEnum.OnlineAppl
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetApplicationRegisterRecordsFor(IEnumerable<int> peopleIds)
        {
            return (from p in Db.Persons
                    join applFor in Db.Applications on p.Id equals applFor.SubmittedForPersonId
                    where applFor.SubmittedByPersonId.HasValue && peopleIds.Contains(applFor.SubmittedByPersonId.Value)
                    select new ReportHistoryDTO
                    {
                        Id = p.Id,
                        ValidFrom = p.ValidFrom,
                        ValidTo = p.ValidTo,
                        EGN = p.EgnLnc,
                        IsPerson = true,
                        PageCode = PageCodeEnum.OnlineAppl //TODO
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetBuyerApplicationChangeOfCircumstancesRecordsForPerson(IEnumerable<int> legalIds)
        {
            return (from legal in Db.Legals
                    join applFor in Db.ApplicationChangeOfCircumstances on legal.Id equals applFor.LegalId
                    join buyer in Db.BuyerRegisters on applFor.BuyerId equals buyer.Id
                    where applFor.LegalId.HasValue && legalIds.Contains(applFor.LegalId.Value)
                    select new ReportHistoryDTO
                    {
                        ValidFrom = legal.ValidFrom,
                        ValidTo = legal.ValidTo,
                        IsPerson = false,
                        Id = legal.Id,
                        PageCode = PageCodeEnum.ChangeFirstSaleBuyer
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetShipApplicationChangeOfCircumstancesRecordsForPerson(IEnumerable<int> legalIds)
        {
            return (from legal in Db.Legals
                    join applFor in Db.ApplicationChangeOfCircumstances on legal.Id equals applFor.LegalId
                    join ship in Db.ShipsRegister on applFor.ShipId equals ship.Id
                    where applFor.LegalId.HasValue && legalIds.Contains(applFor.LegalId.Value)
                    select new ReportHistoryDTO
                    {
                        ValidFrom = legal.ValidFrom,
                        ValidTo = legal.ValidTo,
                        IsPerson = false,
                        Id = legal.Id,
                        PageCode = PageCodeEnum.ShipRegChange
                    }).ToList();
        }

        private List<ReportHistoryDTO> GetAquacultureApplicationChangeOfCircumstancesRecordsForPerson(IEnumerable<int> legalIds)
        {
            return (from legal in Db.Legals
                    join applFor in Db.ApplicationChangeOfCircumstances on legal.Id equals applFor.LegalId
                    join aquaculture in Db.AquacultureFacilitiesRegister on applFor.AquacultureFacilityId equals aquaculture.Id
                    where applFor.LegalId.HasValue && legalIds.Contains(applFor.LegalId.Value)
                    select new ReportHistoryDTO
                    {
                        ValidFrom = legal.ValidFrom,
                        ValidTo = legal.ValidTo,
                        IsPerson = false,
                        Id = legal.Id,
                        PageCode = PageCodeEnum.AquaFarmChange
                    }).ToList();
        }
    }
}
