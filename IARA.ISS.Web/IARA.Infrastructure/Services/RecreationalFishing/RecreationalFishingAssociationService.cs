using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.RecreationalFishing;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using IARA.Interfaces.Legals;
using IARA.Security.Permissions;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services
{
    public class RecreationalFishingAssociationService : Service, IRecreationalFishingAssociationService
    {
        private readonly ILegalService legalService;

        public RecreationalFishingAssociationService(IARADbContext db, ILegalService legalService)
            : base(db)
        {
            this.legalService = legalService;
        }

        public IQueryable<RecreationalFishingAssociationDTO> GetAllAssociations(RecreationalFishingAssociationsFilters filters)
        {
            IQueryable<RecreationalFishingAssociationDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                bool showInactive = filters?.ShowInactiveRecords ?? false;
                result = GetAllAssociations(showInactive);
            }
            else
            {
                result = filters.HasAnyFilters()
                    ? GetParametersFilteredAssociations(filters)
                    : GetFreeTextFilteredAssociations(filters.FreeTextSearch, filters.ShowInactiveRecords);
            }
            return result;
        }

        public RecreationalFishingAssociationEditDTO GetAssociation(int id)
        {
            var association = (from assoc in Db.FishingAssociations
                               where assoc.Id == id
                               select new
                               {
                                   assoc.Id,
                                   assoc.AssociationLegalId,
                                   assoc.TerritoryUnitId,
                                   assoc.IsCanceled,
                                   assoc.CancellationDate,
                                   assoc.CancellationReason
                               }).First();

            RecreationalFishingAssociationEditDTO result = new RecreationalFishingAssociationEditDTO
            {
                Id = association.Id,
                TerritoryUnitId = association.TerritoryUnitId,
                IsCanceled = association.IsCanceled,
                CancellationDate = association.CancellationDate,
                CancellationReason = association.CancellationReason
            };

            result.Legal = legalService.GetRegixLegalData(association.AssociationLegalId);
            result.LegalAddresses = legalService.GetAddressRegistrations(association.AssociationLegalId);
            result.Files = Db.GetFiles(Db.FishingAssociationFiles, association.Id);
            return result;
        }

        public List<RecreationalFishingPossibleAssociationLegalDTO> GetPossibleAssociationLegals()
        {
            DateTime now = DateTime.Now;

            string[] perms = {
                Permissions.AssociationsTicketsRead,
                Permissions.AssociationsTicketsAddRecords,
                Permissions.AssociationsTicketApplicationsRead,
                Permissions.AssociationsTicketApplicationsEditRecords,
                Permissions.AssociationsTicketApplicationsDeleteRecords,
                Permissions.AssociationsTicketApplicationsRestoreRecords
            };

            List<string> existingAssociationEiks = (from association in Db.FishingAssociations
                                                    join legal in Db.Legals on association.AssociationLegalId equals legal.Id
                                                    where legal.ValidFrom <= now
                                                        && legal.ValidTo > now
                                                    select legal.Eik).ToList();

            var legalsPerms = (from legal in Db.Legals
                               join userLegal in Db.UserLegals on legal.Id equals userLegal.LegalId
                               join rolePermission in Db.RolePermissions on userLegal.RoleId equals rolePermission.RoleId
                               join permission in Db.Npermissions on rolePermission.PermissionId equals permission.Id
                               where legal.RecordType == nameof(RecordTypesEnum.Register)
                                 && legal.ValidFrom <= now
                                 && legal.ValidTo > now
                                 && !existingAssociationEiks.Contains(legal.Eik)
                               select new
                               {
                                   legal.Id,
                                   Permission = permission.Name
                               }).ToLookup(x => x.Id, y => y.Permission);

            List<int> legalIds = legalsPerms.Select(x => x.Key).ToList();

            var result = (from legal in Db.Legals
                          where legalIds.Contains(legal.Id)
                          orderby legal.Name
                          select new RecreationalFishingPossibleAssociationLegalDTO
                          {
                              Id = legal.Id,
                              EIK = legal.Eik,
                              Name = legal.Name,
                              IsChecked = false
                          }).ToList();

            foreach (var legal in result)
            {
                legal.HasPermissions = legalsPerms[legal.Id].Any(x => perms.Contains(x));
            }

            return result;
        }

        public RecreationalFishingAssociationEditDTO GetLegalForAssociation(int id)
        {
            RecreationalFishingAssociationEditDTO result = new RecreationalFishingAssociationEditDTO
            {
                Id = id,
                IsCanceled = false
            };

            result.Legal = legalService.GetRegixLegalData(id);
            result.LegalAddresses = legalService.GetAddressRegistrations(id);

            return result;
        }

        public int AddAssociation(RecreationalFishingAssociationEditDTO association)
        {
            FishingAssociation entry = new FishingAssociation
            {
                TerritoryUnitId = association.TerritoryUnitId.Value,
                AssociationLegalId = association.LegalId.Value,
                IsCanceled = false
            };

            if (association.Files != null)
            {
                foreach (FileInfoDTO file in association.Files)
                {
                    Db.AddOrEditFile(entry, entry.FishingAssociationFiles, file);
                }
            }

            Db.FishingAssociations.Add(entry);
            Db.SaveChanges();
            return entry.Id;
        }

        public void EditAssociation(RecreationalFishingAssociationEditDTO association)
        {
            FishingAssociation dbAssociation = (from assoc in Db.FishingAssociations
                                                    .AsSplitQuery()
                                                    .Include(x => x.AssociationLegal)
                                                    .Include(x => x.FishingAssociationFiles)
                                                where assoc.Id == association.Id.Value
                                                select assoc).First();

            dbAssociation.TerritoryUnitId = association.TerritoryUnitId.Value;
            dbAssociation.AssociationLegal = Db.AddOrEditLegal(new ApplicationRegisterDataDTO
            {
                ApplicationId = dbAssociation.AssociationLegal.ApplicationId,
                RecordType = RecordTypesEnum.Register
            }, association.Legal, association.LegalAddresses, dbAssociation.AssociationLegalId);

            dbAssociation.IsCanceled = association.IsCanceled.Value;
            if (dbAssociation.IsCanceled)
            {
                dbAssociation.CancellationDate = association.CancellationDate;
                dbAssociation.CancellationReason = association.CancellationReason;
            }
            else
            {
                dbAssociation.CancellationDate = null;
                dbAssociation.CancellationReason = null;
            }

            if (association.Files != null)
            {
                foreach (FileInfoDTO file in association.Files)
                {
                    Db.AddOrEditFile(dbAssociation, dbAssociation.FishingAssociationFiles, file);
                }
            }

            Db.SaveChanges();
        }

        public void DeleteAssociation(int id)
        {
            DeleteRecordWithId(Db.FishingAssociations, id);
            Db.SaveChanges();
        }

        public void UndoDeleteAssociation(int id)
        {
            UndoDeleteRecordWithId(Db.FishingAssociations, id);
            Db.SaveChanges();
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.FishingAssociations, id);
        }

        public List<NomenclatureDTO> GetUserFishingAssociations(int userId)
        {
            ILookup<int, string> legalPermissions = (from assoc in Db.FishingAssociations
                                                     join legal in Db.Legals on assoc.AssociationLegalId equals legal.Id
                                                     join userLegal in Db.UserLegals on legal.Id equals userLegal.LegalId
                                                     join rolePermission in Db.RolePermissions on userLegal.RoleId equals rolePermission.RoleId
                                                     join permission in Db.Npermissions on rolePermission.PermissionId equals permission.Id
                                                     where assoc.IsActive
                                                           && !assoc.IsCanceled
                                                           && userLegal.UserId == userId
                                                     select new
                                                     {
                                                         userLegal.LegalId,
                                                         PermissionCode = permission.Name
                                                     }).ToLookup(x => x.LegalId, y => y.PermissionCode);

            List<int> legalIds = new List<int>();

            string[] perms = {
                Permissions.AssociationsTicketsRead,
                Permissions.AssociationsTicketsAddRecords,
                Permissions.AssociationsTicketApplicationsRead,
                Permissions.AssociationsTicketApplicationsEditRecords,
                Permissions.AssociationsTicketApplicationsDeleteRecords,
                Permissions.AssociationsTicketApplicationsRestoreRecords
            };

            foreach (IGrouping<int, string> legalPerms in legalPermissions)
            {
                foreach (string perm in perms)
                {
                    if (legalPerms.Contains(perm))
                    {
                        legalIds.Add(legalPerms.Key);
                        break;
                    }
                }
            }

            List<NomenclatureDTO> result = (from assoc in Db.FishingAssociations
                                            join legal in Db.Legals on assoc.AssociationLegalId equals legal.Id
                                            where legalIds.Contains(assoc.AssociationLegalId)
                                            select new NomenclatureDTO
                                            {
                                                Value = assoc.Id,
                                                DisplayName = legal.Name
                                            }).ToList();

            return result;
        }

        public List<NomenclatureDTO> GetAllAssociationNomenclatures()
        {
            List<NomenclatureDTO> associations = (from assoc in Db.FishingAssociations
                                                  join legal in Db.Legals on assoc.AssociationLegalId equals legal.Id
                                                  orderby legal.Name
                                                  select new NomenclatureDTO
                                                  {
                                                      Value = assoc.Id,
                                                      DisplayName = legal.Name,
                                                      IsActive = assoc.IsActive
                                                  }).ToList();
            return associations;
        }

        private IQueryable<RecreationalFishingAssociationDTO> GetAllAssociations(bool showInactive)
        {
            IQueryable<RecreationalFishingAssociationDTO> query = from assoc in Db.FishingAssociations
                                                                  join legal in Db.Legals on assoc.AssociationLegalId equals legal.Id
                                                                  where assoc.IsActive == !showInactive
                                                                  orderby legal.Name
                                                                  select new RecreationalFishingAssociationDTO
                                                                  {
                                                                      Id = assoc.Id,
                                                                      Name = legal.Name,
                                                                      TerritoryUnit = assoc.TerritoryUnit.Name,
                                                                      EIK = legal.Eik,
                                                                      Phone = legal.LegalPhoneNumbers.Where(x => x.IsActive).Select(x => x.Phone.Phone).SingleOrDefault(),
                                                                      MembersCount = assoc.FishingAssociationMembers.Count(x => x.IsActive),
                                                                      IsCanceled = assoc.IsCanceled,
                                                                      IsActive = assoc.IsActive
                                                                  };
            return query;
        }

        private IQueryable<RecreationalFishingAssociationDTO> GetParametersFilteredAssociations(RecreationalFishingAssociationsFilters filters)
        {
            var query = from assoc in Db.FishingAssociations
                        join legal in Db.Legals on assoc.AssociationLegalId equals legal.Id
                        where assoc.IsActive == !filters.ShowInactiveRecords
                        select new
                        {
                            assoc.Id,
                            legal.Name,
                            assoc.TerritoryUnitId,
                            TerritoryUnit = assoc.TerritoryUnit.Name,
                            legal.Eik,
                            Phone = legal.LegalPhoneNumbers.Where(x => x.IsActive).Select(x => x.Phone.Phone).SingleOrDefault(),
                            MembersCount = assoc.FishingAssociationMembers.Count(x => x.IsActive),
                            assoc.IsCanceled,
                            assoc.IsActive
                        };

            if (!string.IsNullOrEmpty(filters.Name))
            {
                query = query.Where(x => x.Name.ToLower().Contains(filters.Name.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.EIK))
            {
                query = query.Where(x => x.Eik.ToLower().Contains(filters.EIK.ToLower()));
            }

            if (filters.TerritoryUnitId.HasValue)
            {
                query = query.Where(x => x.TerritoryUnitId == filters.TerritoryUnitId.Value);
            }

            if (filters.ShowCanceled.HasValue)
            {
                query = query.Where(x => x.IsCanceled == filters.ShowCanceled.Value);
            }

            IQueryable<RecreationalFishingAssociationDTO> result = from assoc in query
                                                                   orderby assoc.Name
                                                                   select new RecreationalFishingAssociationDTO
                                                                   {
                                                                       Id = assoc.Id,
                                                                       Name = assoc.Name,
                                                                       TerritoryUnit = assoc.TerritoryUnit,
                                                                       EIK = assoc.Eik,
                                                                       Phone = assoc.Phone,
                                                                       MembersCount = assoc.MembersCount,
                                                                       IsCanceled = assoc.IsCanceled,
                                                                       IsActive = assoc.IsActive
                                                                   };
            return result;
        }

        private IQueryable<RecreationalFishingAssociationDTO> GetFreeTextFilteredAssociations(string text, bool showInactive)
        {
            text = text.ToLowerInvariant();

            IQueryable<RecreationalFishingAssociationDTO> query = from assoc in Db.FishingAssociations
                                                                  join legal in Db.Legals on assoc.AssociationLegalId equals legal.Id
                                                                  select new RecreationalFishingAssociationDTO
                                                                  {
                                                                      Id = assoc.Id,
                                                                      Name = legal.Name,
                                                                      TerritoryUnit = assoc.TerritoryUnit.Name,
                                                                      EIK = legal.Eik,
                                                                      Phone = legal.LegalPhoneNumbers.Where(x => x.IsActive).Select(x => x.Phone.Phone).SingleOrDefault(),
                                                                      MembersCount = assoc.FishingAssociationMembers.Count(x => x.IsActive),
                                                                      IsCanceled = assoc.IsCanceled,
                                                                      IsActive = assoc.IsActive
                                                                  };

            query = from assoc in query
                    where assoc.IsActive == !showInactive
                        && (assoc.Name.ToLower().Contains(text)
                         || assoc.EIK.ToLower().Contains(text)
                         || assoc.TerritoryUnit.ToLower().Contains(text)
                         || assoc.Phone.ToLower().Contains(text))
                    orderby assoc.Name
                    select assoc;

            return query;
        }
    }
}
