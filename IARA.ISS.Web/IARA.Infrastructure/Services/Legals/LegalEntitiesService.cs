using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using IARA.Common.Enums;
using IARA.Common.Exceptions;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.LegalEntities;
using IARA.DomainModels.DTOModels.User;
using IARA.DomainModels.DTOModels.UserManagement;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using IARA.Interfaces.FSM;
using IARA.Interfaces.Legals;
using IARA.RegixAbstractions.Interfaces;
using IARA.RegixAbstractions.Models.ApplicationIds;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services.Legals
{
    public class LegalEntitiesService : Service, ILegalEntitiesService
    {
        private readonly ILegalService legalService;
        private readonly IPersonService personService;
        private readonly IApplicationService applicationService;
        private readonly IApplicationStateMachine stateMachine;
        private readonly IRegixApplicationInterfaceService regixApplicationService;
        public LegalEntitiesService(IARADbContext db,
                                    ILegalService legalService,
                                    IPersonService personService,
                                    IApplicationService applicationService,
                                    IApplicationStateMachine stateMachine,
                                    IRegixApplicationInterfaceService regixApplicationService)
            : base(db)
        {
            this.legalService = legalService;
            this.personService = personService;
            this.applicationService = applicationService;
            this.stateMachine = stateMachine;
            this.regixApplicationService = regixApplicationService;
        }

        public IQueryable<LegalEntityDTO> GetAllLegalEntities(LegalEntitiesFilters filters)
        {
            IQueryable<LegalEntityDTO> result;
            if (filters == null || !filters.HasAnyFilters(false))
            {
                result = GetAllLegalEntities();
            }
            else
            {
                result = filters.HasFreeTextSearch()
                    ? GetFreeTextFilteredLegalEntities(filters.FreeTextSearch, filters.TerritoryUnitId)
                    : GetParametersFilteredLegalEntities(filters);
            }

            return result;
        }

        public LegalEntityEditDTO GetLegalEntity(int id)
        {
            LegalEntityEditDTO result = (from legal in Db.Legals
                                         where legal.Id == id
                                         select new LegalEntityEditDTO
                                         {
                                             Id = legal.Id,
                                             ApplicationId = legal.ApplicationId
                                         }).First();

            result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(result.ApplicationId.Value, ApplicationHierarchyTypesEnum.Online);
            result.Legal = legalService.GetRegixLegalData(result.Id);
            result.Addresses = legalService.GetAddressRegistrations(result.Id);
            result.AuthorizedPeople = GetAuthorizedPeople(result.Id);
            result.Files = Db.GetFiles(Db.LegalFiles, result.Id);

            return result;
        }

        public LegalEntityEditDTO GetRegisterByApplicationId(int applicationId)
        {
            int legalId = (from legal in Db.Legals
                           where legal.ApplicationId == applicationId
                                && legal.RecordType == nameof(RecordTypesEnum.Register)
                           select legal.Id).First();

            return GetLegalEntity(legalId);
        }

        public int AddLegalEntity(LegalEntityEditDTO legal)
        {
            Legal entry;

            using (TransactionScope scope = new TransactionScope())
            {
                entry = Db.AddOrEditLegal(
                   new ApplicationRegisterDataDTO
                   {
                       ApplicationId = legal.ApplicationId.Value,
                       RecordType = RecordTypesEnum.Register
                   },
                   legal.Legal,
                   legal.Addresses
               );

                if (legal.Files != null)
                {
                    foreach (FileInfoDTO file in legal.Files)
                    {
                        Db.AddOrEditFile(entry, entry.LegalFiles, file);
                    }
                }

                Db.Legals.Add(entry);
                Db.SaveChanges();

                if (legal.AuthorizedPeople != null)
                {
                    foreach (AuthorizedPersonDTO person in legal.AuthorizedPeople)
                    {
                        AddOrEditLegalEntityAuthorizedPerson(entry, person, UserLegalStatusEnum.Approved);
                    }

                    Db.SaveChanges();
                }

                scope.Complete();
            }

            stateMachine.Act(legal.ApplicationId.Value);

            return entry.Id;
        }

        public void EditLegalEntity(LegalEntityEditDTO legal)
        {
            using TransactionScope scope = new TransactionScope();

            Legal dbLegal = (from leg in Db.Legals
                                .AsSplitQuery()
                                .Include(x => x.LegalFiles)
                                .Include(x => x.UserLegals)
                             where leg.Id == legal.Id
                             select leg).First();

            Legal entry = Db.AddOrEditLegal(
                new ApplicationRegisterDataDTO
                {
                    ApplicationId = dbLegal.ApplicationId.Value,
                    RecordType = RecordTypesEnum.Register
                },
                legal.Legal,
                legal.Addresses,
                dbLegal.Id
            );

            Db.SaveChanges();

            if (legal.Files != null)
            {
                foreach (FileInfoDTO file in legal.Files)
                {
                    Db.AddOrEditFile(dbLegal, dbLegal.LegalFiles, file);
                }
            }

            if (legal.AuthorizedPeople != null)
            {
                foreach (AuthorizedPersonDTO person in legal.AuthorizedPeople)
                {
                    AddOrEditLegalEntityAuthorizedPerson(dbLegal, person, UserLegalStatusEnum.Approved);
                    Db.SaveChanges();
                }
            }

            scope.Complete();
        }

        public LegalEntityRegixDataDTO GetApplicationRegixData(int applicationId)
        {
            LegalApplicationDataIds regixDataIds = GetApplicationDataIds(applicationId);

            LegalEntityRegixDataDTO regixData = new LegalEntityRegixDataDTO
            {
                Id = regixDataIds.LegalId,
                ApplicationId = applicationId
            };

            regixData.Requester = personService.GetRegixPersonData(regixDataIds.SubmittedByPersonId);
            regixData.RequesterAddresses = personService.GetAddressRegistrations(regixDataIds.SubmittedByPersonId);
            regixData.Legal = legalService.GetRegixLegalData(regixDataIds.SubmittedForLegalId);
            regixData.Addresses = legalService.GetAddressRegistrations(regixDataIds.SubmittedForLegalId);
            regixData.AuthorizedPeople = GetAuthorizedPeopleRegix(regixDataIds.LegalId);

            return regixData;
        }

        public RegixChecksWrapperDTO<LegalEntityRegixDataDTO> GetLegalEntityRegixData(int applicationId)
        {
            RegixChecksWrapperDTO<LegalEntityRegixDataDTO> result = new RegixChecksWrapperDTO<LegalEntityRegixDataDTO>
            {
                DialogDataModel = GetApplicationRegixData(applicationId),
                RegiXDataModel = regixApplicationService.GetLegalChecks(applicationId)
            };

            result.DialogDataModel.ApplicationRegiXChecks = result.RegiXDataModel.ApplicationRegiXChecks;

            return result;
        }

        public LegalEntityEditDTO GetApplicationDataForRegister(int applicationId)
        {
            int legalId = (from legal in Db.Legals
                           where legal.ApplicationId == applicationId
                              && legal.RecordType == nameof(RecordTypesEnum.Application)
                           select legal.Id).First();

            LegalEntityEditDTO result = GetLegalEntity(legalId);
            return result;
        }

        public LegalEntityApplicationEditDTO GetLegalEntityApplication(int applicationId)
        {
            Legal dbLegal = (from legal in Db.Legals
                             where legal.ApplicationId == applicationId
                                && legal.RecordType == nameof(RecordTypesEnum.Application)
                             select legal).FirstOrDefault();

            LegalEntityApplicationEditDTO result = null;

            if (dbLegal == null)
            {
                string draft = (from appl in Db.Applications
                                where appl.Id == applicationId
                                select appl.ApplicationDraftContents).First();

                if (!string.IsNullOrEmpty(draft))
                {
                    result = CommonUtils.Deserialize<LegalEntityApplicationEditDTO>(draft);
                    result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online);
                    result.Files = Db.GetFiles(Db.ApplicationFiles, applicationId);
                }
                else
                {
                    result = new LegalEntityApplicationEditDTO
                    {
                        IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online)
                    };
                }
            }
            else
            {
                result = new LegalEntityApplicationEditDTO
                {
                    Id = dbLegal.Id,
                    ApplicationId = dbLegal.ApplicationId
                };

                int submittedByPersonId = (from appl in Db.Applications
                                           where appl.Id == result.ApplicationId.Value
                                           select appl.SubmittedByPersonId.Value).First();

                result.Requester = personService.GetRegixPersonData(submittedByPersonId);
                result.RequesterAddresses = personService.GetAddressRegistrations(submittedByPersonId);

                result.Legal = legalService.GetRegixLegalData(result.Id.Value);
                result.Addresses = legalService.GetAddressRegistrations(result.Id.Value);

                result.AuthorizedPeople = GetAuthorizedPeople(result.Id.Value);
                result.Files = Db.GetFiles(Db.LegalFiles, result.Id.Value);
                result.IsOnlineApplication = applicationService.IsApplicationHierarchyType(applicationId, ApplicationHierarchyTypesEnum.Online);
            }

            return result;
        }

        public int AddLegalEntityApplication(LegalEntityApplicationEditDTO legal, ApplicationStatusesEnum? manualStatus)
        {
            Application application;
            Legal entry;

            using (TransactionScope scope = new TransactionScope())
            {
                application = (from appl in Db.Applications
                               where appl.Id == legal.ApplicationId
                               select appl).First();

                application.SubmittedByPerson = Db.AddOrEditPerson(legal.Requester, legal.RequesterAddresses);

                Db.SaveChanges();

                entry = Db.AddOrEditLegal(
                   new ApplicationRegisterDataDTO
                   {
                       ApplicationId = application.Id,
                       RecordType = RecordTypesEnum.Application
                   },
                   legal.Legal,
                   legal.Addresses
               );

                application.SubmittedForLegal = entry;

                if (legal.Files != null)
                {
                    foreach (FileInfoDTO file in legal.Files)
                    {
                        Db.AddOrEditFile(entry, entry.LegalFiles, file);
                    }
                }

                Db.SaveChanges();

                if (legal.AuthorizedPeople != null)
                {
                    foreach (AuthorizedPersonDTO person in legal.AuthorizedPeople)
                    {
                        AddOrEditLegalEntityAuthorizedPerson(entry, person, UserLegalStatusEnum.Requested);
                        Db.SaveChanges();
                    }
                }

                scope.Complete();
            }

            List<FileInfoDTO> files = legal.Files;
            legal.Files = null;
            stateMachine.Act(application.Id, CommonUtils.Serialize(legal), legal.Files, manualStatus);

            return entry.Id;
        }

        public void EditLegalEntityApplication(LegalEntityApplicationEditDTO legal, ApplicationStatusesEnum? manualStatus = null)
        {
            Application application;

            using (TransactionScope scope = new TransactionScope())
            {
                Legal dbLegal = (from leg in Db.Legals
                                    .AsSplitQuery()
                                    .Include(x => x.LegalFiles)
                                    .Include(x => x.UserLegals)
                                 where leg.Id == legal.Id
                                 select leg).First();

                application = (from appl in Db.Applications
                               where appl.Id == legal.ApplicationId
                               select appl).First();

                application.SubmittedByPerson = Db.AddOrEditPerson(legal.Requester, legal.RequesterAddresses, application.SubmittedByPersonId);

                Db.SaveChanges();

                Db.AddOrEditLegal(
                    new ApplicationRegisterDataDTO
                    {
                        ApplicationId = application.Id,
                        RecordType = RecordTypesEnum.Application
                    },
                    legal.Legal,
                    legal.Addresses,
                    application.SubmittedForLegalId
                );

                if (legal.Files != null)
                {
                    foreach (FileInfoDTO file in legal.Files)
                    {
                        Db.AddOrEditFile(dbLegal, dbLegal.LegalFiles, file);
                    }
                }

                Db.SaveChanges();

                if (legal.AuthorizedPeople != null)
                {
                    foreach (AuthorizedPersonDTO person in legal.AuthorizedPeople)
                    {
                        AddOrEditLegalEntityAuthorizedPerson(dbLegal, person, UserLegalStatusEnum.Requested);
                        Db.SaveChanges();
                    }
                }

                scope.Complete();
            }

            List<FileInfoDTO> files = legal.Files;
            legal.Files = null;
            stateMachine.Act(application.Id, CommonUtils.Serialize(legal), files, manualStatus, legal.StatusReason);
        }

        public void EditLegalEntityApplicationRegixData(LegalEntityRegixDataDTO legal)
        {
            Application application;

            using (TransactionScope scope = new TransactionScope())
            {
                Legal dbLegal = (from leg in Db.Legals
                                 where leg.Id == legal.Id
                                 select leg).First();

                application = (from appl in Db.Applications
                               where appl.Id == dbLegal.ApplicationId
                               select appl).First();

                application.SubmittedByPerson = Db.AddOrEditPerson(legal.Requester, legal.RequesterAddresses, application.SubmittedByPersonId);

                Db.SaveChanges();

                Db.AddOrEditLegal(
                    new ApplicationRegisterDataDTO
                    {
                        ApplicationId = application.Id,
                        RecordType = RecordTypesEnum.Application
                    },
                    legal.Legal,
                    legal.Addresses,
                    application.SubmittedForLegalId
                );

                Db.SaveChanges();

                if (legal.AuthorizedPeople != null)
                {
                    foreach (AuthorizedPersonRegixDataDTO person in legal.AuthorizedPeople)
                    {
                        EditLegalEntityAuthorizedPersonRegixData(person);
                        Db.SaveChanges();
                    }
                }

                scope.Complete();
            }

            stateMachine.Act(id: application.Id, toState: ApplicationStatusesEnum.EXT_CHK_STARTED);
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.Legals, id);
        }

        public SimpleAuditDTO GetAuthorizedPersonSimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.Persons, id);
        }

        public AuthorizedPersonDTO GetAuthorizedPersonFromUserId(int userId)
        {
            var data = (from user in Db.Users
                        where user.Id == userId
                        select new
                        {
                            user.PersonId,
                            user.Email
                        }).First();

            RegixPersonDataDTO person = personService.GetRegixPersonData(data.PersonId);
            person.Email = data.Email;

            AuthorizedPersonDTO result = new AuthorizedPersonDTO
            {
                Id = data.PersonId,
                UserId = userId,
                Person = person,
                IsActive = true
            };

            return result;
        }

        public bool HasUserAccessToLegalEntityFile(int userId, int fileId)
        {
            List<int> fileLegalIds = (from lf in Db.LegalFiles
                                      where lf.FileId == fileId
                                      select lf.RecordId).ToList();

            // file is in draft
            if (fileLegalIds.Count == 0)
            {
                List<int> applicationIds = (from af in Db.ApplicationFiles
                                            where af.FileId == fileId
                                            select af.RecordId).ToList();

                return applicationService.AreApplicationsSubmittedByUser(userId, applicationIds);
            }

            List<int> userLegalIds = GetUserLegalIds(userId);
            return fileLegalIds.All(x => userLegalIds.Contains(x));
        }

        public Task<ApplicationEDeliveryInfo> GetEDeliveryInfo(int applicationId, PageCodeEnum pageCode)
        {
            throw new NotImplementedException("Nothing to deliver for legal entity application with page code: " + pageCode.ToString());
        }

        private LegalApplicationDataIds GetApplicationDataIds(int applicationId)
        {
            LegalApplicationDataIds ids = (from legal in Db.Legals
                                           join appl in Db.Applications on legal.ApplicationId equals appl.Id
                                           where legal.ApplicationId == applicationId
                                              && legal.RecordType == nameof(RecordTypesEnum.Application)
                                           select new LegalApplicationDataIds
                                           {
                                               LegalId = legal.Id,
                                               SubmittedByPersonId = appl.SubmittedByPersonId.Value,
                                               SubmittedForLegalId = appl.SubmittedForLegalId.HasValue ? appl.SubmittedForLegalId.Value : default
                                           }).First();

            return ids;
        }

        private List<int> GetUserLegalIds(int userId)
        {
            List<int> result = (from ul in Db.UserLegals
                                where ul.UserId == userId
                                    && ul.IsActive
                                select ul.LegalId).ToList();
            return result;
        }

        private IQueryable<LegalEntityDTO> GetAllLegalEntities()
        {
            DateTime now = DateTime.Now;

            HashSet<int> legalIds = GetLegalEntityIds();

            IQueryable<LegalEntityDTO> legals = from legal in Db.Legals
                                                where legal.RecordType == nameof(RecordTypesEnum.Register)
                                                    && legal.ValidFrom <= now
                                                    && legal.ValidTo > now
                                                    && legalIds.Contains(legal.Id)
                                                orderby legal.ValidFrom descending, legal.Name
                                                select new LegalEntityDTO
                                                {
                                                    Id = legal.Id,
                                                    Name = legal.Name,
                                                    Eik = legal.Eik,
                                                    RegistrationDate = legal.ValidFrom,
                                                    ActiveUsersCount = legal.UserLegals.Count(x => x.IsActive && x.AccessValidFrom <= now && x.AccessValidTo > now)
                                                };
            return legals;
        }

        private IQueryable<LegalEntityDTO> GetParametersFilteredLegalEntities(LegalEntitiesFilters filters)
        {
            DateTime now = DateTime.Now;

            HashSet<int> legalIds = GetLegalEntityIds();

            var query = from legal in Db.Legals
                        join appl in Db.Applications on legal.ApplicationId equals appl.Id
                        where legal.RecordType == nameof(RecordTypesEnum.Register)
                            && legal.ValidFrom <= now
                            && legal.ValidTo > now
                            && legalIds.Contains(legal.Id)
                        orderby legal.ValidFrom descending, legal.Name
                        select new
                        {
                            Id = legal.Id,
                            Name = legal.Name,
                            Eik = legal.Eik,
                            RegistrationDate = legal.ValidFrom,
                            ActiveUsersCount = legal.UserLegals.Count(x => x.IsActive && x.AccessValidFrom <= now && x.AccessValidTo > now),
                            TerritoryUnitId = appl.TerritoryUnitId
                        };

            if (!string.IsNullOrEmpty(filters.LegalName))
            {
                query = query.Where(x => x.Name.ToLower().Contains(filters.LegalName.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.Eik))
            {
                query = query.Where(x => x.Eik.ToLower().Contains(filters.Eik.ToLower()));
            }

            if (filters.RegisteredDateFrom.HasValue)
            {
                query = query.Where(x => x.RegistrationDate >= filters.RegisteredDateFrom.Value);
            }

            if (filters.RegisteredDateTo.HasValue)
            {
                query = query.Where(x => x.RegistrationDate <= filters.RegisteredDateTo.Value);
            }

            if (filters.TerritoryUnitId.HasValue)
            {
                query = query.Where(x => x.TerritoryUnitId.HasValue && x.TerritoryUnitId == filters.TerritoryUnitId.Value);
            }

            IQueryable<LegalEntityDTO> legals = from legal in query
                                                select new LegalEntityDTO
                                                {
                                                    Id = legal.Id,
                                                    Name = legal.Name,
                                                    Eik = legal.Eik,
                                                    RegistrationDate = legal.RegistrationDate,
                                                    ActiveUsersCount = legal.ActiveUsersCount
                                                };

            return legals;
        }

        private IQueryable<LegalEntityDTO> GetFreeTextFilteredLegalEntities(string text, int? territoryUnitId)
        {
            DateTime now = DateTime.Now;

            text = text.ToLowerInvariant();
            DateTime? searchDate = DateTimeUtils.TryParseDate(text);

            HashSet<int> legalIds = GetLegalEntityIds();

            IQueryable<LegalEntityDTO> legalEntities = from legal in Db.Legals
                                                       join appl in Db.Applications on legal.ApplicationId equals appl.Id
                                                       where legal.RecordType == nameof(RecordTypesEnum.Register)
                                                           && legal.ValidFrom <= now
                                                           && legal.ValidTo > now
                                                           && legalIds.Contains(legal.Id)
                                                           && (legal.Name.ToLower().Contains(text)
                                                                || legal.Eik.ToLower().Contains(text)
                                                                || (searchDate.HasValue && legal.ValidFrom.Date == searchDate.Value))
                                                            && (!territoryUnitId.HasValue || appl.TerritoryUnitId == territoryUnitId.Value)
                                                       orderby legal.ValidFrom descending, legal.Name
                                                       select new LegalEntityDTO
                                                       {
                                                           Id = legal.Id,
                                                           Name = legal.Name,
                                                           Eik = legal.Eik,
                                                           RegistrationDate = legal.ValidFrom,
                                                           ActiveUsersCount = legal.UserLegals.Count(x => x.IsActive && x.AccessValidFrom <= now && x.AccessValidTo > now)
                                                       };
            return legalEntities;
        }

        private HashSet<int> GetLegalEntityIds()
        {
            HashSet<int> result = (from legal in Db.Legals
                                   join legalAppl in Db.Legals on legal.RegisterApplicationId equals legalAppl.Id
                                   join appl in Db.Applications on legalAppl.ApplicationId equals appl.Id
                                   join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                                   where legal.RecordType == nameof(RecordTypesEnum.Register)
                                        && legal.RegisterApplicationId.HasValue
                                        && applType.PageCode == nameof(PageCodeEnum.LE)
                                   select legal.Id).ToHashSet();

            return result;
        }

        private List<AuthorizedPersonDTO> GetAuthorizedPeople(int legalId)
        {
            HashSet<int> userIds = (from userLegal in Db.UserLegals
                                    where userLegal.LegalId == legalId
                                    select userLegal.UserId).ToHashSet();

            List<AuthorizedPersonDTO> people = (from user in Db.Users
                                                join person in Db.Persons on user.PersonId equals person.Id
                                                where userIds.Contains(user.Id)
                                                select new AuthorizedPersonDTO
                                                {
                                                    Id = person.Id,
                                                    UserId = user.Id
                                                }).ToList();

            List<int> personIds = people.Select(x => x.Id.Value).ToList();

            Dictionary<int, RegixPersonDataDTO> regixData = personService.GetRegixPersonsData(personIds);

            ILookup<int, RoleDTO> roles = (from userLegal in Db.UserLegals
                                           join role in Db.Roles on userLegal.RoleId equals role.Id
                                           where userLegal.LegalId == legalId
                                                && userIds.Contains(userLegal.UserId)
                                           select new
                                           {
                                               userLegal.UserId,
                                               Role = new RoleDTO
                                               {
                                                   Id = role.Id,
                                                   Name = role.Name,
                                                   AccessValidFrom = userLegal.AccessValidFrom,
                                                   AccessValidTo = userLegal.AccessValidTo,
                                                   IsActive = userLegal.IsActive
                                               }
                                           }).ToLookup(x => x.UserId, y => y.Role);

            foreach (AuthorizedPersonDTO person in people)
            {
                person.Person = regixData[person.Id.Value];
                person.Roles = roles[person.UserId].ToList();
                person.IsActive = person.Roles.Any(x => x.IsActive);
            }

            return people;
        }

        private List<AuthorizedPersonRegixDataDTO> GetAuthorizedPeopleRegix(int legalId)
        {
            HashSet<int> userIds = (from userLegal in Db.UserLegals
                                    where userLegal.LegalId == legalId
                                        && userLegal.IsActive
                                    select userLegal.UserId).ToHashSet();

            List<AuthorizedPersonRegixDataDTO> people = (from user in Db.Users
                                                         join person in Db.Persons on user.PersonId equals person.Id
                                                         where userIds.Contains(user.Id)
                                                         select new AuthorizedPersonRegixDataDTO
                                                         {
                                                             Id = person.Id,
                                                             UserId = user.Id,
                                                             IsActive = true
                                                         }).ToList();


            List<int> personIds = people.Select(x => x.Id.Value).ToList();

            Dictionary<int, RegixPersonDataDTO> regixData = personService.GetRegixPersonsData(personIds);

            foreach (AuthorizedPersonRegixDataDTO person in people)
            {
                person.Person = regixData[person.Id.Value];
            }

            return people;
        }

        private void EditLegalEntityAuthorizedPersonRegixData(AuthorizedPersonRegixDataDTO person)
        {
            AddOrEditUserFromAuthorizedPerson(person);
        }

        private void AddOrEditLegalEntityAuthorizedPerson(Legal legal, AuthorizedPersonDTO person, UserLegalStatusEnum status)
        {
            User user = AddOrEditUserFromAuthorizedPerson(person);

            АddOrEditUserLegalRoles(legal, user, person.Roles, status, person.IsActive.Value);
        }

        private User AddOrEditUserFromAuthorizedPerson(AuthorizedPersonRegixDataDTO person)
        {
            (bool exists, User user) = ValidateAuthorizedPerson(person);
            if (exists)
            {
                Db.AddOrEditPerson(person.Person, oldPersonId: user.PersonId);
            }
            else
            {
                user = AddExternalUser(person);
            }
            return user;
        }

        private (bool exists, User user) ValidateAuthorizedPerson(AuthorizedPersonRegixDataDTO person)
        {
            ObjectException egnAndEmailDontMatchException = new ObjectException(new AuthorizedPersonErrorDTO
            {
                EgnLnc = person.Person.EgnLnc.EgnLnc,
                Email = person.Person.Email,
                EgnAndEmailDontMatch = true
            });

            User userByEgn = (from usr in Db.Users
                              join per in Db.Persons on usr.PersonId equals per.Id
                              where per.EgnLnc == person.Person.EgnLnc.EgnLnc
                                 && per.IdentifierType == person.Person.EgnLnc.IdentifierType.ToString()
                              select usr).FirstOrDefault();

            User userByEmail = (from usr in Db.Users
                                where usr.Email == person.Person.Email
                                select usr).FirstOrDefault();

            string userByEmailEgnLnc = null;
            if (userByEmail != null)
            {
                userByEmailEgnLnc = (from per in Db.Persons
                                     where per.Id == userByEmail.PersonId
                                     select per.EgnLnc).First();
            }

            // не съществува потребител с това ЕГН
            if (userByEgn == null)
            {
                // имейлът вече се използва от потребител с друго ЕГН
                if (userByEmail?.Email == person.Person.Email)
                {
                    throw egnAndEmailDontMatchException;
                }
                else
                {
                    return (false, userByEgn);
                }
            }
            // съществува потребител с това ЕГН
            else
            {
                // подаденият имейл не съвпада с имейла на потребителя
                if (userByEgn.Email != person.Person.Email)
                {
                    throw egnAndEmailDontMatchException;
                }
                // имейлът вече се използва от потребител с друго ЕГН
                else if (userByEmailEgnLnc != null && userByEmailEgnLnc == userByEgn.Email)
                {
                    throw egnAndEmailDontMatchException;
                }
                else
                {
                    return (true, userByEgn);
                }
            }
        }

        private User AddExternalUser(AuthorizedPersonRegixDataDTO person)
        {
            UserRegistrationDTO registration = GetRegistrationFromAuthorizedPerson(person);

            List<RoleDTO> roles = person is AuthorizedPersonDTO authorizedPerson ? authorizedPerson.Roles : new List<RoleDTO>();
            User user = Db.AddExternalUser(person.Person, registration, roles);
            return user;
        }

        private UserRegistrationDTO GetRegistrationFromAuthorizedPerson(AuthorizedPersonRegixDataDTO person)
        {
            UserRegistrationDTO registration = new UserRegistrationDTO
            {
                Email = person.Person.Email,
                Password = null,
                EgnLnc = person.Person.EgnLnc,
                FirstName = person.Person.FirstName,
                MiddleName = person.Person.MiddleName,
                LastName = person.Person.LastName,
                HasUserPassLogin = false,
                HasEAuthLogin = false
            };

            return registration;
        }

        private void АddOrEditUserLegalRoles(Legal legal, User user, List<RoleDTO> roles, UserLegalStatusEnum status, bool isActive)
        {
            DateTime now = DateTime.Now;

            foreach (RoleDTO role in roles)
            {
                UserLegal userLegal = (from ul in legal.UserLegals
                                       where ul.RoleId == role.Id
                                            && ul.UserId == user.Id
                                       select ul).FirstOrDefault();

                if (userLegal != null)
                {
                    userLegal.AccessValidFrom = role.AccessValidFrom;
                    userLegal.AccessValidTo = role.AccessValidTo;
                    userLegal.Status = status.ToString();
                    userLegal.IsActive = isActive;
                }
                else
                {
                    Db.UserLegals.Add(new UserLegal
                    {
                        User = user,
                        Legal = legal,
                        RoleId = role.Id,
                        AccessValidFrom = role.AccessValidFrom,
                        AccessValidTo = role.AccessValidTo,
                        Status = status.ToString()
                    });
                }
            }
        }
    }
}
