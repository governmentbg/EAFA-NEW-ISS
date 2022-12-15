using System;
using System.Linq;
using IARA.Common.Exceptions;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.CommercialFishingRegister;
using IARA.DomainModels.DTOModels.PrintConfigurations;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Interfaces;

namespace IARA.Infrastructure.Services
{
    public class PrintConfigurationsService : Service, IPrintConfigurationsService
    {
        public PrintConfigurationsService(IARADbContext db)
            : base(db)
        {
        }

        public IQueryable<PrintConfigurationDTO> GetAllPrintConfigurations(PrintConfigurationFilters filters)
        {
            IQueryable<PrintConfigurationDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                bool showInactive = filters?.ShowInactiveRecords ?? false;
                result = GetAllPrintConfigurations(showInactive);
            }
            else
            {
                result = filters.HasFreeTextSearch()
                    ? GetFreeTextFilteredPrintConfigurations(filters.FreeTextSearch, filters.ShowInactiveRecords)
                    : GetParametersFilteredPrintConfigurations(filters);
            }

            return result;
        }

        public PrintConfigurationEditDTO GetPrintConfiguration(int id)
        {
            PrintConfigurationEditDTO result = (from applSignUser in Db.ApplicationPrintSignUser
                                                where applSignUser.Id == id
                                                select new PrintConfigurationEditDTO
                                                {
                                                    Id = applSignUser.Id,
                                                    ApplicationTypeId = applSignUser.ApplicationTypeId,
                                                    SignUserId = applSignUser.SignUserId,
                                                    TerritoryUnitId = applSignUser.TerritoryUnitId,
                                                    SubstituteUserId = applSignUser.SubstituteUserId,
                                                    SubstituteReason = applSignUser.SubstituteReason,
                                                }).First();
            return result;
        }

        public PrintConfigurationEditDTO AddOrEditPringConfiguration(PrintConfigurationEditDTO model)
        {
            CheckAndThrowIfPrintConfigurationExists(model.Id, model.ApplicationTypeId, model.TerritoryUnitId);

            ApplicationPrintSignUser entry;

            if (model.Id.HasValue) // edit
            {
                entry = (from applSignUser in Db.ApplicationPrintSignUser
                         where applSignUser.Id == model.Id.Value
                         select applSignUser).First();
            }
            else // add
            {
                entry = (from applSignUser in Db.ApplicationPrintSignUser
                         where applSignUser.ApplicationTypeId == model.ApplicationTypeId
                               && applSignUser.TerritoryUnitId == model.TerritoryUnitId
                               && !applSignUser.IsActive
                         select applSignUser).FirstOrDefault();

                if (entry == null) // the entry is totally new
                {
                    entry = new ApplicationPrintSignUser();
                }
                else // the entry is inactive and should become active again
                {
                    entry.IsActive = true;
                }
            }

            entry.ApplicationTypeId = model.ApplicationTypeId;
            entry.TerritoryUnitId = model.TerritoryUnitId;
            entry.SignUserId = model.SignUserId.Value;
            entry.SubstituteUserId = model.SubstituteUserId;

            if (model.SubstituteUserId.HasValue && string.IsNullOrEmpty(model.SubstituteReason))
            {
                throw new ArgumentException("Substitute reason is mandotory when substitute user id is present");
            }

            entry.SubstituteReason = model.SubstituteReason;

            if (!model.Id.HasValue)
            {
                Db.ApplicationPrintSignUser.Add(entry);
            }

            model.Id = Db.SaveChanges();

            return model;
        }

        public void DeletePrintConfiguration(int id)
        {
            DeleteRecordWithId(Db.ApplicationPrintSignUser, id);
            Db.SaveChanges();
        }

        public void UndoDeletePrintConfiguration(int id)
        {
            UndoDeleteRecordWithId(Db.ApplicationPrintSignUser, id);
            Db.SaveChanges();
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.ApplicationPrintSignUser, id);
        }

        private IQueryable<PrintConfigurationDTO> GetAllPrintConfigurations(bool showInactive)
        {
            DateTime now = DateTime.Now;

            return from applPrintUser in Db.ApplicationPrintSignUser
                   join applType in Db.NapplicationTypes on applPrintUser.ApplicationTypeId equals applType.Id into leftApplType
                   from applType in leftApplType.DefaultIfEmpty()
                   join territoryUnit in Db.NterritoryUnits on applPrintUser.TerritoryUnitId equals territoryUnit.Id into leftTerritoryUnit
                   from territoryUnit in leftTerritoryUnit.DefaultIfEmpty()
                   join signUser in Db.Users on applPrintUser.SignUserId equals signUser.Id
                   join signPerson in Db.Persons on signUser.PersonId equals signPerson.Id
                   join activePerson in Db.Persons on new
                   {
                       signPerson.EgnLnc,
                       signPerson.IdentifierType
                   }
                    equals new
                    {
                        activePerson.EgnLnc,
                        activePerson.IdentifierType
                    }
                   join substituteUser in Db.Users on applPrintUser.SubstituteUserId equals substituteUser.Id into leftSubUser
                   from substituteUser in leftSubUser.DefaultIfEmpty()
                   join substitutePerson in Db.Persons on substituteUser.PersonId equals substitutePerson.Id into leftSubPerson
                   from substitutePerson in leftSubPerson.DefaultIfEmpty()
                   join activeSubstitutePerson in Db.Persons on new
                   {
                       substitutePerson.EgnLnc,
                       substitutePerson.IdentifierType
                   }
                    equals new
                    {
                        activeSubstitutePerson.EgnLnc,
                        activeSubstitutePerson.IdentifierType
                    } into leftActiveSubPerson
                   from activeSubstitutePerson in leftActiveSubPerson.DefaultIfEmpty()
                   where applPrintUser.IsActive == !showInactive
                         && activePerson.ValidFrom <= now
                         && activePerson.ValidTo > now
                         && (activeSubstitutePerson == null || (activeSubstitutePerson.ValidFrom <= now && activeSubstitutePerson.ValidTo > now))
                   orderby applType.Name
                   select new PrintConfigurationDTO
                   {
                       Id = applPrintUser.Id,
                       ApplicationTypeName = applType.Name,
                       TerritoryUnitName = territoryUnit.Name,
                       SignUserNames = $"{activePerson.FirstName} {activePerson.LastName} ({signUser.Username})",
                       SubstituteUserNames = substituteUser != null && activeSubstitutePerson != null
                                                ? $"{activeSubstitutePerson.FirstName} {activeSubstitutePerson.LastName} ({substituteUser.Username})"
                                                : "",
                       SubstituteReason = applPrintUser.SubstituteReason,
                       IsActive = applPrintUser.IsActive
                   };
        }

        private IQueryable<PrintConfigurationDTO> GetFreeTextFilteredPrintConfigurations(string freeTextSeach, bool showInactive)
        {
            DateTime now = DateTime.Now;
            string textSearch = freeTextSeach.ToLower().Trim();

            return from applPrintUser in Db.ApplicationPrintSignUser
                   join applType in Db.NapplicationTypes on applPrintUser.ApplicationTypeId equals applType.Id into leftApplType
                   from applType in leftApplType.DefaultIfEmpty()
                   join territoryUnit in Db.NterritoryUnits on applPrintUser.TerritoryUnitId equals territoryUnit.Id into leftTerritoryUnit
                   from territoryUnit in leftTerritoryUnit.DefaultIfEmpty()
                   join signUser in Db.Users on applPrintUser.SignUserId equals signUser.Id
                   join signPerson in Db.Persons on signUser.PersonId equals signPerson.Id
                   join activePerson in Db.Persons on new
                   {
                       signPerson.EgnLnc,
                       signPerson.IdentifierType
                   }
                    equals new
                    {
                        activePerson.EgnLnc,
                        activePerson.IdentifierType
                    }
                   join substituteUser in Db.Users on applPrintUser.SubstituteUserId equals substituteUser.Id into leftSubUser
                   from substituteUser in leftSubUser.DefaultIfEmpty()
                   join substitutePerson in Db.Persons on substituteUser.PersonId equals substitutePerson.Id into leftSubPerson
                   from substitutePerson in leftSubPerson.DefaultIfEmpty()
                   join activeSubstitutePerson in Db.Persons on new
                   {
                       substitutePerson.EgnLnc,
                       substitutePerson.IdentifierType
                   }
                    equals new
                    {
                        activeSubstitutePerson.EgnLnc,
                        activeSubstitutePerson.IdentifierType
                    } into leftActiveSubPerson
                   from activeSubstitutePerson in leftActiveSubPerson.DefaultIfEmpty()
                   where applPrintUser.IsActive == !showInactive
                         && activePerson.ValidFrom <= now
                         && activePerson.ValidTo > now
                         && (activeSubstitutePerson == null || (activeSubstitutePerson.ValidFrom <= now && activeSubstitutePerson.ValidTo > now))
                         && (applType.Name.ToLower().Contains(textSearch)
                              || territoryUnit.Name.ToLower().Contains(textSearch)
                              || (activePerson.FirstName + " " + activePerson.LastName + " (" + signUser.Username + ")").ToLower().Contains(textSearch)
                              || (activeSubstitutePerson.FirstName + " " + activeSubstitutePerson.LastName + " (" + substituteUser.Username + ")").ToLower().Contains(textSearch)
                              || applPrintUser.SubstituteReason.ToLower().Contains(textSearch))
                   orderby applType.Name
                   select new PrintConfigurationDTO
                   {
                       Id = applPrintUser.Id,
                       ApplicationTypeName = applType.Name,
                       TerritoryUnitName = territoryUnit.Name,
                       SignUserNames = $"{activePerson.FirstName} {activePerson.LastName} ({signUser.Username})",
                       SubstituteUserNames = substituteUser != null && activeSubstitutePerson != null
                                                ? $"{activeSubstitutePerson.FirstName} {activeSubstitutePerson.LastName} ({substituteUser.Username})"
                                                : "",
                       SubstituteReason = applPrintUser.SubstituteReason,
                       IsActive = applPrintUser.IsActive
                   };
        }

        private IQueryable<PrintConfigurationDTO> GetParametersFilteredPrintConfigurations(PrintConfigurationFilters filters)
        {
            DateTime now = DateTime.Now;

            return from applPrintUser in Db.ApplicationPrintSignUser
                   join applType in Db.NapplicationTypes on applPrintUser.ApplicationTypeId equals applType.Id into leftApplType
                   from applType in leftApplType.DefaultIfEmpty()
                   join territoryUnit in Db.NterritoryUnits on applPrintUser.TerritoryUnitId equals territoryUnit.Id into leftTerritoryUnit
                   from territoryUnit in leftTerritoryUnit.DefaultIfEmpty()
                   join signUser in Db.Users on applPrintUser.SignUserId equals signUser.Id
                   join signPerson in Db.Persons on signUser.PersonId equals signPerson.Id
                   join activePerson in Db.Persons on new
                   {
                       signPerson.EgnLnc,
                       signPerson.IdentifierType
                   }
                    equals new
                    {
                        activePerson.EgnLnc,
                        activePerson.IdentifierType
                    }
                   join substituteUser in Db.Users on applPrintUser.SubstituteUserId equals substituteUser.Id into leftSubUser
                   from substituteUser in leftSubUser.DefaultIfEmpty()
                   join substitutePerson in Db.Persons on substituteUser.PersonId equals substitutePerson.Id into leftSubPerson
                   from substitutePerson in leftSubPerson.DefaultIfEmpty()
                   join activeSubstitutePerson in Db.Persons on new
                   {
                       substitutePerson.EgnLnc,
                       substitutePerson.IdentifierType
                   }
                    equals new
                    {
                        activeSubstitutePerson.EgnLnc,
                        activeSubstitutePerson.IdentifierType
                    } into leftActiveSubPerson
                   from activeSubstitutePerson in leftActiveSubPerson.DefaultIfEmpty()
                   where applPrintUser.IsActive == !filters.ShowInactiveRecords
                         && activePerson.ValidFrom <= now
                         && activePerson.ValidTo > now
                         && (activeSubstitutePerson == null || (activeSubstitutePerson.ValidFrom <= now && activeSubstitutePerson.ValidTo > now))
                         && (filters.TerritoryUnitIds == null || (territoryUnit != null && filters.TerritoryUnitIds.Contains(territoryUnit.Id)))
                         && (filters.ApplicationTypeIds == null || (applType != null && filters.ApplicationTypeIds.Contains(applType.Id)))
                         && (string.IsNullOrEmpty(filters.SubstituteReason) || applPrintUser.SubstituteReason.ToLower().Contains(filters.SubstituteReason.ToLower()))
                         && (string.IsNullOrEmpty(filters.UserEgnLnch) || activePerson.EgnLnc == filters.UserEgnLnch || activeSubstitutePerson.EgnLnc == filters.UserEgnLnch)
                         && (string.IsNullOrEmpty(filters.UserNames)
                             || (activePerson.FirstName + " " + activePerson.LastName + " (" + signUser.Username + ")").ToLower().Contains(filters.UserNames.ToLower())
                             || (activeSubstitutePerson.FirstName + " " + activeSubstitutePerson.LastName + " (" + substituteUser.Username + ")").ToLower().Contains(filters.UserNames.ToLower()))
                   orderby applType.Name
                   select new PrintConfigurationDTO
                   {
                       Id = applPrintUser.Id,
                       ApplicationTypeName = applType.Name,
                       TerritoryUnitName = territoryUnit.Name,
                       SignUserNames = $"{activePerson.FirstName} {activePerson.LastName} ({signUser.Username})",
                       SubstituteUserNames = substituteUser != null && activeSubstitutePerson != null
                                                ? $"{activeSubstitutePerson.FirstName} {activeSubstitutePerson.LastName} ({substituteUser.Username})"
                                                : "",
                       SubstituteReason = applPrintUser.SubstituteReason,
                       IsActive = applPrintUser.IsActive
                   };
        }

        private void CheckAndThrowIfPrintConfigurationExists(int? id, int? applicationTypeId, int? territoryUnitId)
        {
            bool exists = (from applSignUser in Db.ApplicationPrintSignUser
                           where applSignUser.ApplicationTypeId == applicationTypeId
                                 && applSignUser.TerritoryUnitId == territoryUnitId
                                 && applSignUser.IsActive
                                 && (!id.HasValue || applSignUser.Id != id.Value)
                           select 1).Any();

            if (exists)
            {
                throw new PrintConfigurationAlreadyExistsException(applicationTypeId, territoryUnitId);
            }
        }
    }
}
