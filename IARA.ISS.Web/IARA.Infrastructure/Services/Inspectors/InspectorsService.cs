using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.Common.Exceptions;
using IARA.Common.Resources;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Inspectors;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Interfaces;

namespace IARA.Infrastructure.Services.Inspectors
{
    public class InspectorsService : Service, IInspectorsService
    {
        public static readonly string IARA_INSTITUTION_CODE = "ИАРА";

        public InspectorsService(IARADbContext db)
            : base(db)
        {
        }

        public IQueryable<InspectorsRegisterDTO> GetAll(InspectorsFilters filters, bool isRegistered)
        {
            IQueryable<InspectorsRegisterDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                bool showInactive = filters?.ShowInactiveRecords ?? false;
                result = GetAllInspectors(showInactive, isRegistered);
            }
            else
            {
                result = filters.HasAnyFilters()
                    ? GetParameterFilteredInspectors(filters, isRegistered)
                    : GetFreeTextFilteredInspectors(filters.FreeTextSearch, filters.ShowInactiveRecords, isRegistered);
            }

            return result;
        }

        public int AddInspector(InspectorsRegisterEditDTO inspector)
        {
            if (InspectorAlreadyExists(inspector.UserId.Value))
            {
                throw new InspectorAlreadyExistsException();
            }

            Inspector entry = new Inspector
            {
                UserId = inspector.UserId,
                InstitutionId = (from institution in Db.Ninstitutions
                                 where institution.Code == IARA_INSTITUTION_CODE
                                 select institution.Id).Single(),
                InspectionSequenceNum = 0,
                InspectorCardNum = inspector.InspectorCardNum
            };

            Db.Inspectors.Add(entry);
            Db.SaveChanges();

            return entry.Id;
        }

        public int AddUnregisteredInspector(UnregisteredPersonEditDTO inspector)
        {
            if (UnregisteredInspectorAlreadyExists(inspector.EgnLnc, inspector.IdentifierType.Value))
            {
                throw new InspectorAlreadyExistsException();
            }

            Inspector entry = new Inspector
            {
                InstitutionId = inspector.InstitutionId,
                InspectionSequenceNum = 0,
                InspectorCardNum = inspector.InspectorCardNum
            };

            List<EgnLncDTO> unregPersonsEgns = (from person in Db.UnregisteredPersons
                                                select new EgnLncDTO
                                                {
                                                    EgnLnc = person.EgnLnc,
                                                    IdentifierType = Enum.Parse<IdentifierTypeEnum>(person.IdentifierType)
                                                }).ToList();

            if (unregPersonsEgns.Any(x => x.EgnLnc == inspector.EgnLnc && x.IdentifierType == inspector.IdentifierType))
            {
                entry.UnregisteredPerson = (from unregPerson in Db.UnregisteredPersons
                                            where unregPerson.EgnLnc == inspector.EgnLnc
                                                && unregPerson.IdentifierType == inspector.IdentifierType.ToString()
                                            select unregPerson).Single();
            }
            else
            {
                UnregisteredPerson unregPerson = new UnregisteredPerson
                {
                    EgnLnc = inspector.EgnLnc,
                    IdentifierType = inspector.IdentifierType.ToString(),
                    FirstName = inspector.FirstName,
                    LastName = inspector.LastName,
                    Comments = inspector.Comments
                };

                entry.UnregisteredPerson = unregPerson;
                Db.UnregisteredPersons.Add(unregPerson);
            }

            Db.Inspectors.Add(entry);
            Db.SaveChanges();

            return entry.Id;
        }

        public void EditInspector(InspectorsRegisterEditDTO inspectorRegister)
        {
            Inspector dbInspector = (from inspector in Db.Inspectors
                                     where inspector.Id == inspectorRegister.Id
                                     select inspector).First();

            dbInspector.InspectorCardNum = inspectorRegister.InspectorCardNum;

            Db.SaveChanges();
        }

        public void EditUnregisteredInspector(UnregisteredPersonEditDTO unregisteredInspector)
        {
            if (UnregisteredInspectorAlreadyExists(unregisteredInspector.EgnLnc, unregisteredInspector.IdentifierType.Value))
            {
                throw new InspectorAlreadyExistsException();
            }

            Inspector dbInspector = (from inspector in Db.Inspectors
                                     where inspector.Id == unregisteredInspector.Id
                                     select inspector).First();

            UnregisteredPerson dbUnregisteredPerson = (from unregPerson in Db.UnregisteredPersons
                                                       where unregPerson.Id == dbInspector.UnregisteredPersonId.Value
                                                       select unregPerson).First();

            dbInspector.InspectorCardNum = unregisteredInspector.InspectorCardNum;
            dbInspector.InstitutionId = unregisteredInspector.InstitutionId;
            dbUnregisteredPerson.EgnLnc = unregisteredInspector.EgnLnc;
            dbUnregisteredPerson.IdentifierType = unregisteredInspector.IdentifierType.Value.ToString();
            dbUnregisteredPerson.FirstName = unregisteredInspector.FirstName;
            dbUnregisteredPerson.LastName = unregisteredInspector.LastName;
            dbUnregisteredPerson.Comments = unregisteredInspector.Comments;

            Db.SaveChanges();
        }

        public void DeleteInspector(int id)
        {
            DeleteRecordWithId(Db.Inspectors, id);
            Db.SaveChanges();
        }

        public void UndoDeleteInspector(int id)
        {
            UndoDeleteRecordWithId(Db.Inspectors, id);
            Db.SaveChanges();
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.Inspectors, id);
        }

        private IQueryable<InspectorsRegisterDTO> GetAllInspectors(bool showInactive, bool isRegistered)
        {
            IQueryable<InspectorsRegisterDTO> inspectors;

            if (isRegistered)
            {
                inspectors = from inspector in Db.Inspectors
                             join user in Db.Users on inspector.UserId equals user.Id
                             join person in Db.Persons on user.PersonId equals person.Id
                             where inspector.IsActive == !showInactive
                             select new InspectorsRegisterDTO
                             {
                                 Id = inspector.Id,
                                 UserId = user.Id,
                                 FirstName = person.FirstName,
                                 LastName = person.LastName,
                                 InspectorCardNum = inspector.InspectorCardNum,
                                 IsActive = inspector.IsActive
                             };
            }
            else
            {
                inspectors = from inspector in Db.Inspectors
                             join unregPerson in Db.UnregisteredPersons on inspector.UnregisteredPersonId equals unregPerson.Id
                             where inspector.IsActive == !showInactive
                             select new InspectorsRegisterDTO
                             {
                                 Id = inspector.Id,
                                 FirstName = unregPerson.FirstName,
                                 LastName = unregPerson.LastName,
                                 EgnLnc = unregPerson.EgnLnc,
                                 IdentifierType = unregPerson.IdentifierType,
                                 InstitutionId = inspector.InstitutionId,
                                 InspectorCardNum = inspector.InspectorCardNum,
                                 Comments = unregPerson.Comments,
                                 IsActive = inspector.IsActive
                             };
            }

            return inspectors;
        }

        private IQueryable<InspectorsRegisterDTO> GetFreeTextFilteredInspectors(string text, bool showInactive, bool isRegistered)
        {
            text = text.ToLowerInvariant();
            IQueryable<InspectorsRegisterDTO> inspectors;

            if (isRegistered)
            {
                inspectors = from inspector in Db.Inspectors
                             join user in Db.Users on inspector.UserId equals user.Id
                             join person in Db.Persons on user.PersonId equals person.Id
                             where inspector.IsActive == !showInactive
                                    && (user.Username.ToLower().Contains(text)
                                    || person.FirstName.ToLower().Contains(text)
                                    || person.LastName.ToLower().Contains(text)
                                    || inspector.InspectorCardNum.ToLower().Contains(text))
                             select new InspectorsRegisterDTO
                             {
                                 Id = inspector.Id,
                                 UserId = user.Id,
                                 FirstName = person.FirstName,
                                 LastName = person.LastName,
                                 InspectorCardNum = inspector.InspectorCardNum,
                                 IsActive = inspector.IsActive
                             };
            }
            else
            {
                inspectors = from inspector in Db.Inspectors
                             join unregPerson in Db.UnregisteredPersons on inspector.UnregisteredPersonId equals unregPerson.Id
                             join institution in Db.Ninstitutions on inspector.InstitutionId equals institution.Id
                             where inspector.IsActive == !showInactive
                                    && (unregPerson.EgnLnc == text
                                    || unregPerson.FirstName.ToLower().Contains(text)
                                    || unregPerson.LastName.ToLower().Contains(text)
                                    || institution.Name.ToLower().Contains(text)
                                    || inspector.InspectorCardNum.ToLower().Contains(text)
                                    || unregPerson.Comments.ToLower().Contains(text))
                             select new InspectorsRegisterDTO
                             {
                                 Id = inspector.Id,
                                 EgnLnc = unregPerson.EgnLnc,
                                 IdentifierType = unregPerson.IdentifierType,
                                 FirstName = unregPerson.FirstName,
                                 LastName = unregPerson.LastName,
                                 InstitutionId = inspector.InstitutionId,
                                 InspectorCardNum = inspector.InspectorCardNum,
                                 Comments = unregPerson.Comments,
                                 IsActive = inspector.IsActive
                             };
            }
            return inspectors;
        }

        private IQueryable<InspectorsRegisterDTO> GetParameterFilteredInspectors(InspectorsFilters filters, bool isRegistered)
        {
            var query = from inspector in Db.Inspectors
                        join institution in Db.Ninstitutions on inspector.InstitutionId equals institution.Id
                        join u in Db.Users on inspector.UserId equals u.Id into users
                        from user in users.DefaultIfEmpty()
                        join person in Db.Persons on user.PersonId equals person.Id into pers
                        from person in pers.DefaultIfEmpty()
                        join unregPerson in Db.UnregisteredPersons on inspector.UnregisteredPersonId equals unregPerson.Id into unregPersons
                        from unregisteredPerson in unregPersons.DefaultIfEmpty()
                        where inspector.IsActive == !filters.ShowInactiveRecords
                        select new
                        {
                            inspector.Id,
                            inspector.UserId,
                            inspector.UnregisteredPersonId,
                            Username = user != null ? user.Username : null,
                            FirstName = person != null ? person.FirstName : unregisteredPerson.FirstName,
                            LastName = person != null ? person.LastName : unregisteredPerson.LastName,
                            unregisteredPerson.EgnLnc,
                            unregisteredPerson.IdentifierType,
                            InstitutionName = institution.Name,
                            inspector.InstitutionId,
                            inspector.InspectorCardNum,
                            unregisteredPerson.Comments,
                            inspector.IsActive
                        };

            if (filters.UserId.HasValue)
            {
                query = query.Where(x => x.UserId == filters.UserId.Value);
            }

            if (filters.InstitutionId.HasValue)
            {
                query = query.Where(x => x.InstitutionId == filters.InstitutionId.Value);
            }

            if (!string.IsNullOrEmpty(filters.FirstName))
            {
                query = query.Where(x => x.FirstName.ToLower().Contains(filters.FirstName.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.LastName))
            {
                query = query.Where(x => x.LastName.ToLower().Contains(filters.LastName.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.UnregPersonName) && !isRegistered)
            {
                query = query.Where(x => (x.FirstName.ToLower() + " " + x.LastName.ToLower()).Contains(filters.UnregPersonName.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.EgnLnc) && !isRegistered)
            {
                query = query.Where(x => x.EgnLnc == filters.EgnLnc);
            }

            if (!string.IsNullOrEmpty(filters.InspectorCardNum))
            {
                query = query.Where(x => x.InspectorCardNum.ToLower().Contains(filters.InspectorCardNum.ToLower()));
            }

            IQueryable<InspectorsRegisterDTO> inspectors = from inspector in query
                                                           where (inspector.UserId.HasValue && isRegistered)
                                                                || (inspector.UserId == null
                                                                    && inspector.UnregisteredPersonId != null
                                                                    && !isRegistered)
                                                           select new InspectorsRegisterDTO
                                                           {
                                                               Id = inspector.Id,
                                                               UserId = inspector.UserId,
                                                               FirstName = inspector.FirstName,
                                                               LastName = inspector.LastName,
                                                               EgnLnc = inspector.EgnLnc,
                                                               IdentifierType = inspector.IdentifierType,
                                                               InstitutionId = inspector.InstitutionId,
                                                               InspectorCardNum = inspector.InspectorCardNum,
                                                               Comments = inspector.Comments,
                                                               IsActive = inspector.IsActive
                                                           };

            return inspectors;
        }

        private bool InspectorAlreadyExists(int userId)
        {
            bool result = (from inspectorRegister in Db.Inspectors
                           where inspectorRegister.UserId.HasValue
                             && inspectorRegister.UserId.Value == userId
                           select inspectorRegister.Id).Any();

            return result;
        }

        private bool UnregisteredInspectorAlreadyExists(string egn, IdentifierTypeEnum identifierType)
        {
            var inspectorsEgns = (from unregInspector in Db.Inspectors
                                  join person in Db.UnregisteredPersons on unregInspector.UnregisteredPersonId equals person.Id
                                  select new EgnLncDTO
                                  {
                                      EgnLnc = person.EgnLnc,
                                      IdentifierType = Enum.Parse<IdentifierTypeEnum>(person.IdentifierType)
                                  }).ToList();

            EgnLncDTO inspectorEgn = new EgnLncDTO
            {
                EgnLnc = egn,
                IdentifierType = identifierType
            };

            return inspectorsEgns.Any(x => x.EgnLnc == egn && x.IdentifierType == identifierType);
        }
    }
}
