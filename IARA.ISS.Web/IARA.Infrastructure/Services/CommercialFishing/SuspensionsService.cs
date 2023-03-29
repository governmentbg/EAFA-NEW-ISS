using System.Transactions;
using IARA.Common.Constants;
using IARA.Common.Exceptions;
using IARA.DomainModels.DTOModels.CommercialFishingRegister;
using IARA.DomainModels.Nomenclatures;
using IARA.FVMSModels.ExternalModels;
using IARA.Interfaces;
using IARA.Interfaces.CommercialFishing;
using IARA.Interfaces.FVMSIntegrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TL.Dependency.Abstractions;

namespace IARA.Infrastructure.Services.CommercialFishing
{
    public class SuspensionsService : Service, ISuspensionsService
    {
        private readonly IShipsRegisterService shipsRegisterService;
        private readonly IScopedServiceProviderFactory serviceProviderFactory;
        public SuspensionsService(IARADbContext dbContext, 
                                  IShipsRegisterService shipsRegisterService, 
                                  IScopedServiceProviderFactory serviceProviderFactory) 
            : base(dbContext)
        {
            this.shipsRegisterService = shipsRegisterService;
            this.serviceProviderFactory = serviceProviderFactory;
        }

        public List<SuspensionDataDTO> GetPermitSuspensions(int permitId)
        {
            List<SuspensionDataDTO> results = (from permitSuspension in Db.CommercialFishingPermitSuspensionChangeHistories
                                               join reason in Db.NsuspensionReasons on permitSuspension.ReasonId equals reason.Id
                                               join suspensionType in Db.NsuspensionTypes on reason.SuspensionTypeId equals suspensionType.Id
                                               where permitSuspension.PermitId == permitId
                                               select new SuspensionDataDTO
                                               {
                                                   Id = permitSuspension.Id,
                                                   SuspensionTypeId = reason.SuspensionTypeId,
                                                   SuspensionTypeName = suspensionType.Name,
                                                   ReasonId = reason.Id,
                                                   ReasonName = reason.Name,
                                                   ValidFrom = permitSuspension.SuspensionValidFrom,
                                                   ValidTo = permitSuspension.SuspensionValidTo,
                                                   EnactmentDate = permitSuspension.EnactmentDate,
                                                   OrderNumber = permitSuspension.OrderNumber,
                                                   IsActive = permitSuspension.IsActive
                                               }).ToList();

            return results;

        }

        public List<SuspensionDataDTO> GetPermitLicenseSuspensions(int permitLicenseId)
        {
            List<SuspensionDataDTO> results = (from permitSuspension in Db.CommercialFishingPermitLicenseSuspensionChangeHistories
                                               join reason in Db.NsuspensionReasons on permitSuspension.ReasonId equals reason.Id
                                               join suspensionType in Db.NsuspensionTypes on reason.SuspensionTypeId equals suspensionType.Id
                                               where permitSuspension.PermitLicenseId == permitLicenseId
                                               select new SuspensionDataDTO
                                               {
                                                   Id = permitSuspension.Id,
                                                   SuspensionTypeId = reason.SuspensionTypeId,
                                                   SuspensionTypeName = suspensionType.Name,
                                                   ReasonId = reason.Id,
                                                   ReasonName = reason.Name,
                                                   ValidFrom = permitSuspension.SuspensionValidFrom,
                                                   ValidTo = permitSuspension.SuspensionValidTo,
                                                   EnactmentDate = permitSuspension.EnactmentDate,
                                                   OrderNumber = permitSuspension.OrderNumber,
                                                   IsActive = permitSuspension.IsActive
                                               }).ToList();

            return results;
        }

        public void AddEditPermitSuspensions(int permitId, List<SuspensionDataDTO> suspensions, int currentUserId, SuspensionPermissionsDTO permissions)
        {
            using TransactionScope scope = Db.CreateTransaction();

            List<SuspensionDataDTO> originalSuspensions = new List<SuspensionDataDTO>(suspensions);

            // check for any permissions

            if (!permissions.CanAddSuspensions 
                && !permissions.CanEditSuspensions 
                && !permissions.CanDeleteSuspensions 
                && !permissions.CanRestoreSuspensions)
            {
                throw new UnauthorizedAccessException();
            }

            List<PermitSuspensionChangeHistory> dbSuspensions = (from susp in Db.CommercialFishingPermitSuspensionChangeHistories
                                                                                .Include(x => x.Reason)
                                                                                .AsSplitQuery()
                                                                 where susp.PermitId == permitId
                                                                 select susp).ToList();

            PermitRegister dbPermit = (from permit in Db.CommercialFishingPermitRegisters
                                       where permit.Id == permitId
                                       select permit).First();

            // check if add with no add permission

            if (!permissions.CanAddSuspensions && suspensions.Any(x => !x.Id.HasValue))
            {
                throw new UnauthorizedAccessException();
            }
            else
            {
                List<SuspensionDataDTO> suspensionsToAdd = suspensions.Where(x => !x.Id.HasValue && x.IsActive).ToList();

                foreach (SuspensionDataDTO suspension in suspensionsToAdd)
                {
                    AddOrEditPermitSuspension(dbPermit, dbSuspensions, suspension, currentUserId);
                    suspensions.Remove(suspension);
                }
            }

            // check if delete with no delete permission

            List<int> dbDeletedSuspensions = dbSuspensions.Where(x => !x.IsActive).Select(x => x.Id).ToList();
            if (!permissions.CanDeleteSuspensions && suspensions.Any(x => x.Id.HasValue && !x.IsActive && !dbDeletedSuspensions.Contains(x.Id.Value)))
            {
                throw new UnauthorizedAccessException();
            }
            else
            {
                List<SuspensionDataDTO> suspensionsToDelete = suspensions.Where(x => x.Id.HasValue && !x.IsActive && !dbDeletedSuspensions.Contains(x.Id.Value)).ToList();
                foreach (SuspensionDataDTO suspension in suspensionsToDelete)
                {
                    PermitSuspensionChangeHistory dbSuspension = (from susp in dbSuspensions
                                                                  where susp.Id == suspension.Id.Value
                                                                  select susp).First();
                    dbSuspension.IsActive = false;

                    suspensions.Remove(suspension);
                }
            }

            // check if restore with no restore permission

            List<int> dbActiveSuspenssions = dbSuspensions.Where(x => x.IsActive).Select(x => x.Id).ToList();
            if (!permissions.CanRestoreSuspensions && suspensions.Any(x => x.Id.HasValue && x.IsActive && !dbActiveSuspenssions.Contains(x.Id.Value)))
            {
                throw new UnauthorizedAccessException();
            }
            else
            {
                List<SuspensionDataDTO> suspensionsToRestore = suspensions.Where(x => x.Id.HasValue && x.IsActive && !dbActiveSuspenssions.Contains(x.Id.Value)).ToList();
                foreach (SuspensionDataDTO suspension in suspensionsToRestore)
                {
                    PermitSuspensionChangeHistory dbSuspension = (from susp in dbSuspensions
                                                                  where susp.Id == suspension.Id.Value
                                                                  select susp).First();
                    dbSuspension.IsActive = true;

                    suspensions.Remove(suspension);
                }
            }

            // check if edit with no edit permission

            if (!permissions.CanEditSuspensions)
            {
                List<SuspensionDataDTO> dbSuspensionsDtos = dbSuspensions.Select(x => new SuspensionDataDTO
                {
                    Id = x.Id,
                    SuspensionTypeId = x.Reason.SuspensionTypeId,
                    ReasonId = x.ReasonId,
                    OrderNumber = x.OrderNumber,
                    EnactmentDate = x.EnactmentDate,
                    ValidFrom = x.SuspensionValidFrom,
                    ValidTo = x.SuspensionValidTo,
                    IsActive = x.IsActive
                }).ToList();

                if (suspensions.Any(x => !dbSuspensionsDtos.Contains(x)))
                {
                    throw new UnauthorizedAccessException();
                }
            }
            else
            {
                foreach (SuspensionDataDTO suspension in suspensions)
                {
                    AddOrEditPermitSuspension(dbPermit, dbSuspensions, suspension, currentUserId);
                }
            }

            SetPermitSuspensionFlag(originalSuspensions, dbPermit);

            Db.SaveChanges();

            scope.Complete();
        }

        public void AddPermitSuspension(SuspensionDataDTO suspension, int permitId, int currentUserId)
        {
            using TransactionScope scope = Db.CreateTransaction();

            PermitRegister dbPermit = (from permit in Db.CommercialFishingPermitRegisters
                                                        .Include(x => x.PermitSuspensionChangeHistories)
                                                        .AsSplitQuery()
                                       where permit.Id == permitId
                                       select permit).First();

            bool wasPermitSuspended = dbPermit.IsSuspended;

            AddOrEditPermitSuspension(dbPermit, dbPermit.PermitSuspensionChangeHistories.ToList(), suspension, currentUserId);

            if (!wasPermitSuspended) // значи разрешителното се прекратява сега
            {
                AddShipMODEventIfNeeded(dbPermit, true);

                dbPermit.IsSuspended = true;
                Db.SaveChanges();

                SuspendRelatedPermitLicenses(dbPermit.Id);
            }

            Db.SaveChanges();

            scope.Complete();
        }

        public void AddEditPermitLicenseSuspensions(int permitLicenseId, List<SuspensionDataDTO> suspensions, int currentUserId, SuspensionPermissionsDTO permissions)
        {
            

            List<SuspensionDataDTO> originalSuspensions = new List<SuspensionDataDTO>(suspensions);

            // check for any permissions

            if (!permissions.CanAddSuspensions 
                && !permissions.CanEditSuspensions 
                && !permissions.CanDeleteSuspensions 
                && !permissions.CanRestoreSuspensions)
            {
                throw new UnauthorizedAccessException();
            }

            List<PermitLicenseSuspensionChangeHistory> dbSuspensions = (from susp in Db.CommercialFishingPermitLicenseSuspensionChangeHistories
                                                                                .Include(x => x.Reason)
                                                                                .AsSplitQuery()
                                                                        where susp.PermitLicenseId == permitLicenseId
                                                                        select susp).ToList();

            PermitLicensesRegister dbPermitLicense = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                                      where permitLicense.Id == permitLicenseId
                                                      select permitLicense).First();

            // check if add with no add permission

            if (!permissions.CanAddSuspensions && suspensions.Any(x => !x.Id.HasValue))
            {
                throw new UnauthorizedAccessException();
            }
            else
            {
                List<SuspensionDataDTO> suspensionsToAdd = suspensions.Where(x => !x.Id.HasValue && x.IsActive).ToList();

                foreach (SuspensionDataDTO suspension in suspensionsToAdd)
                {
                    AddOrEditPermitLicenseSuspension(dbPermitLicense, dbSuspensions, suspension, currentUserId);
                    suspensions.Remove(suspension);
                }
            }

            // check if delete with no delete permission

            List<int> dbDeletedSuspensions = dbSuspensions.Where(x => !x.IsActive).Select(x => x.Id).ToList();
            if (!permissions.CanDeleteSuspensions && suspensions.Any(x => x.Id.HasValue && !x.IsActive && !dbDeletedSuspensions.Contains(x.Id.Value)))
            {
                throw new UnauthorizedAccessException();
            }
            else
            {
                List<SuspensionDataDTO> suspensionsToDelete = suspensions.Where(x => x.Id.HasValue && !x.IsActive && !dbDeletedSuspensions.Contains(x.Id.Value)).ToList();
                foreach (SuspensionDataDTO suspension in suspensionsToDelete)
                {
                    PermitLicenseSuspensionChangeHistory dbSuspension = (from susp in dbSuspensions
                                                                         where susp.Id == suspension.Id.Value
                                                                         select susp).First();
                    dbSuspension.IsActive = false;

                    suspensions.Remove(suspension);
                }
            }

            // check if restore with no restore permission

            List<int> dbActiveSuspenssions = dbSuspensions.Where(x => x.IsActive).Select(x => x.Id).ToList();
            if (!permissions.CanRestoreSuspensions && suspensions.Any(x => x.Id.HasValue && x.IsActive && !dbActiveSuspenssions.Contains(x.Id.Value)))
            {
                throw new UnauthorizedAccessException();
            }
            else
            {
                List<SuspensionDataDTO> suspensionsToRestore = suspensions.Where(x => x.Id.HasValue && x.IsActive && !dbActiveSuspenssions.Contains(x.Id.Value)).ToList();
                foreach (SuspensionDataDTO suspension in suspensionsToRestore)
                {
                    PermitLicenseSuspensionChangeHistory dbSuspension = (from susp in dbSuspensions
                                                                         where susp.Id == suspension.Id.Value
                                                                         select susp).First();
                    dbSuspension.IsActive = true;

                    suspensions.Remove(suspension);
                }
            }

            // check if edit with no edit permission

            if (!permissions.CanEditSuspensions)
            {
                List<SuspensionDataDTO> dbSuspensionsDtos = dbSuspensions.Select(x => new SuspensionDataDTO
                {
                    Id = x.Id,
                    SuspensionTypeId = x.Reason.SuspensionTypeId,
                    ReasonId = x.ReasonId,
                    OrderNumber = x.OrderNumber,
                    EnactmentDate = x.EnactmentDate,
                    ValidFrom = x.SuspensionValidFrom,
                    ValidTo = x.SuspensionValidTo,
                    IsActive = x.IsActive
                }).ToList();

                if (suspensions.Any(x => !dbSuspensionsDtos.Contains(x)))
                {
                    throw new UnauthorizedAccessException();
                }
            }
            else
            {
                foreach (SuspensionDataDTO suspension in suspensions)
                {
                    AddOrEditPermitLicenseSuspension(dbPermitLicense, dbSuspensions, suspension, currentUserId);
                }
            }

            SetPermitLicenseSuspensionFlag(originalSuspensions, dbPermitLicense);

            Db.SaveChanges();
        }

        public void AddPermitLicenseSuspension(SuspensionDataDTO suspension, int permitLicenseId, int currentUserId)
        {
            CheckAndThrowIfPermitLicenseSuspensionValidToExists(suspension.Id, permitLicenseId, suspension.ValidTo ?? DefaultConstants.MAX_VALID_DATE);

            using TransactionScope scope = Db.CreateTransaction();

            PermitLicensesRegister dbPermitLicense = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                                                              .Include(x => x.PermitLicenseSuspensionChangeHistories)
                                                                              .AsSplitQuery()
                                                      where permitLicense.Id == permitLicenseId
                                                      select permitLicense).First();

            bool wasPermitLicenseSuspended = dbPermitLicense.IsSuspended;

            AddOrEditPermitLicenseSuspension(dbPermitLicense,
                                             dbPermitLicense.PermitLicenseSuspensionChangeHistories.ToList(),
                                             suspension,
                                             currentUserId);

            if (!wasPermitLicenseSuspended)
            {
                dbPermitLicense.IsSuspended = true;
                Db.SaveChanges();

                SuspendPermitLicensesLogBooks(new HashSet<int> { dbPermitLicense.Id }); // Suspend related valid log books
            }

            Db.SaveChanges();

            scope.Complete();
        }

        public void UpdatePermitsIsSuspendedFlag()
        {
            List<string> permitRegistrationNumbers = new List<string>();

            using (TransactionScope scope = Db.CreateTransaction())
            {
                DateTime now = DateTime.Now;

                HashSet<int> permitIdsToBeSuspended = (from permit in Db.CommercialFishingPermitRegisters
                                                       join permitSuspension in Db.CommercialFishingPermitSuspensionChangeHistories on permit.Id equals permitSuspension.PermitId
                                                       where permit.RecordType == nameof(RecordTypesEnum.Register)
                                                             && permit.IsActive
                                                             && !permit.IsSuspended
                                                             && permitSuspension.IsActive
                                                             && permitSuspension.SuspensionValidFrom <= now
                                                             && permitSuspension.SuspensionValidTo > now
                                                       select permit.Id).ToHashSet();

                var permitsToBeSuspended = (from permit in Db.CommercialFishingPermitRegisters
                                            join permitType in Db.NcommercialFishingPermitTypes on permit.PermitTypeId equals permitType.Id
                                            where permitIdsToBeSuspended.Contains(permit.Id)
                                            select new
                                            {
                                                PermitType = Enum.Parse<CommercialFishingTypesEnum>(permitType.Code),
                                                Permit = permit
                                            }).ToList();

                permitRegistrationNumbers.AddRange(permitsToBeSuspended.Select(x => x.Permit.RegistrationNum));

                foreach (var permitRegisterData in permitsToBeSuspended)
                {
                    if (!permitRegisterData.Permit.IsSuspended)
                    {
                        if (permitRegisterData.PermitType != CommercialFishingTypesEnum.PoundNetPermit)
                        {
                            List<int> shipValidPermitIds = CommercialFishingHelper.GetValidShipPermitIds(Db, permitRegisterData.Permit.ShipId);

                            if (shipValidPermitIds.Count == 1)
                            {
                                shipsRegisterService.EditShipRsr(permitRegisterData.Permit.ShipId, permitRegisterData.Permit.ApplicationId, false);
                            }
                        }

                        permitRegisterData.Permit.IsSuspended = true;
                        Db.SaveChanges();
                    }
                }

                HashSet<int> permitIdsToBeValid = (from permit in Db.CommercialFishingPermitRegisters
                                                   where permit.RecordType == nameof(RecordTypesEnum.Register)
                                                         && permit.IsActive
                                                         && permit.IsSuspended
                                                         && (from permitSusHist in Db.CommercialFishingPermitSuspensionChangeHistories
                                                             where permitSusHist.PermitId == permit.Id
                                                                   && permitSusHist.IsActive
                                                                   && permitSusHist.SuspensionValidFrom <= now
                                                                   && permitSusHist.SuspensionValidTo > now
                                                             select permitSusHist.Id).Count() == 0
                                                   select permit.Id).ToHashSet();

                var permitsToBeValid = (from permit in Db.CommercialFishingPermitRegisters
                                        join permitType in Db.NcommercialFishingPermitTypes on permit.PermitTypeId equals permitType.Id
                                        where permitIdsToBeValid.Contains(permit.Id)
                                        select new
                                        {
                                            PermitType = Enum.Parse<CommercialFishingTypesEnum>(permitType.Code),
                                            Permit = permit
                                        }).ToList();

                permitRegistrationNumbers.AddRange(permitsToBeValid.Select(x => x.Permit.RegistrationNum));

                foreach (var permitRegisterData in permitsToBeValid)
                {
                    if (permitRegisterData.Permit.IsSuspended)
                    {
                        if (permitRegisterData.PermitType != CommercialFishingTypesEnum.PoundNetPermit)
                        {
                            List<int> shipValidPermitIds = CommercialFishingHelper.GetValidShipPermitIds(Db, permitRegisterData.Permit.ShipId);

                            if (shipValidPermitIds.Count == 0)
                            {
                                permitRegisterData.Permit.ShipId = shipsRegisterService.EditShipRsr(permitRegisterData.Permit.ShipId, permitRegisterData.Permit.ApplicationId, true);
                            }
                        }

                        permitRegisterData.Permit.IsSuspended = false;

                        Db.SaveChanges();
                    }
                }

                Db.SaveChanges();

                scope.Complete();
            }

            if (permitRegistrationNumbers.Any())
            {
                PermitsDataChanged(serviceProviderFactory, permitRegistrationNumbers);
            }
        }

        public void UpdatePermitLicensesIsSuspendedFlag()
        {
            DateTime now = DateTime.Now;
            List<string> permitRegistrationNumbers = new List<string>();

            // Update permit licenses which should to be suspended

            HashSet<int> permitLicensesIdsToBeSuspended = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                                           join permitLicenseSuspension in Db.CommercialFishingPermitLicenseSuspensionChangeHistories on permitLicense.Id equals permitLicenseSuspension.PermitLicenseId
                                                           where permitLicense.RecordType == nameof(RecordTypesEnum.Register)
                                                                 && permitLicense.IsActive
                                                                 && !permitLicense.IsSuspended
                                                                 && permitLicenseSuspension.IsActive
                                                                 && permitLicenseSuspension.SuspensionValidFrom <= now
                                                                 && permitLicenseSuspension.SuspensionValidTo > now
                                                           select permitLicense.Id).ToHashSet();

            var permitLicensesToBeSuspended = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                               join permit in Db.CommercialFishingPermitRegisters on permitLicense.PermitId equals permit.Id
                                               where permitLicensesIdsToBeSuspended.Contains(permitLicense.Id)
                                               select new
                                               {
                                                   PermitLicense = permitLicense,
                                                   PermitRegistrationNumber = permit.RegistrationNum
                                               }).ToList();

            permitRegistrationNumbers.AddRange(permitLicensesToBeSuspended.Select(x => x.PermitRegistrationNumber));

            foreach (var permitLicenseRegister in permitLicensesToBeSuspended)
            {
                permitLicenseRegister.PermitLicense.IsSuspended = true;
            }

            // Suspend all valid log books for all the suspended permit licenses

            HashSet<int> suspendedPermitLicensesIds = permitLicensesToBeSuspended.Select(x => x.PermitLicense.Id).ToHashSet();
            SuspendPermitLicensesLogBooks(suspendedPermitLicensesIds);

            // Update permit licenses which should to be active

            HashSet<int> permitLicenseIdsToBeValid = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                                      where permitLicense.RecordType == nameof(RecordTypesEnum.Register)
                                                            && permitLicense.IsActive
                                                            && permitLicense.IsSuspended
                                                            && (from permitLicenseSusHist in Db.CommercialFishingPermitLicenseSuspensionChangeHistories
                                                                where permitLicenseSusHist.PermitLicenseId == permitLicense.Id
                                                                      && permitLicenseSusHist.IsActive
                                                                      && permitLicenseSusHist.SuspensionValidFrom <= now
                                                                      && permitLicenseSusHist.SuspensionValidTo > now
                                                                select permitLicenseSusHist.Id).Count() == 0
                                                      select permitLicense.Id).ToHashSet();

            List<PermitLicensesRegister> permitLicensesToBeValid = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                                                    where permitLicenseIdsToBeValid.Contains(permitLicense.Id)
                                                                    select permitLicense).ToList();

            permitRegistrationNumbers.AddRange(permitLicensesToBeValid.Select(x => x.RegistrationNum));

            foreach (PermitLicensesRegister permitLicenseRegister in permitLicensesToBeValid)
            {
                permitLicenseRegister.IsSuspended = false;
            }

            // Activate suspended log books for all the valid permit licenses

            HashSet<int> activePermitLicensesIds = permitLicensesToBeValid.Select(x => x.Id).ToHashSet();
            ActivateSuspendedLogBooks(activePermitLicensesIds);

            Db.SaveChanges();

            if (permitRegistrationNumbers.Any())
            {
                PermitsDataChanged(serviceProviderFactory, permitRegistrationNumbers);
            }
        }

        /// <summary>
        /// Прекратява всички непрекратени удостоверения и техните неприключени дневници към дата 01.01 на текущата година.
        /// </summary>
        public void SuspendPermitLicensesAndLogBooksFromPreviousYears()
        {
            DateTime suspendBeforeDate = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime suspentionValidTo = DefaultConstants.MAX_VALID_DATE;
            int systemUserId = DefaultConstants.SYSTEM_USER_ID;

            List<string> permitRegistrationNumbers = new List<string>(); // permits for which an update to FVMS (СНРК) will be needed

            var permitLicensesToSuspendData = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                               join permit in Db.CommercialFishingPermitRegisters on permitLicense.PermitId equals permit.Id
                                               where permitLicense.RecordType == nameof(RecordTypesEnum.Register)
                                                     && permitLicense.IsActive
                                                     && permitLicense.PermitLicenseValidFrom.Value.Date < suspendBeforeDate.Date
                                                     && permitLicense.PermitLicenseValidTo.Value.Date < suspendBeforeDate.Date
                                                     && !permitLicense.IsSuspended
                                               select new
                                               {
                                                   PermitLicense = permitLicense,
                                                   PermitRegistrationNumber = permit.RegistrationNum
                                               }).ToList();

            if (permitLicensesToSuspendData.Any())
            {
                permitRegistrationNumbers.AddRange(permitLicensesToSuspendData.Select(x => x.PermitRegistrationNumber));

                List<int> permitLicenseToSuspendIds = permitLicensesToSuspendData.Select(x => x.PermitLicense.Id).ToList();
                List<PermitLicenseSuspensionChangeHistory> oldDbPermitLicenseSuspensions = (from suspension in Db.CommercialFishingPermitLicenseSuspensionChangeHistories
                                                                                            where permitLicenseToSuspendIds.Contains(suspension.PermitLicenseId)
                                                                                                  && suspension.SuspensionValidTo == suspentionValidTo
                                                                                            select suspension).ToList();

                int suspensionReasonId = (from suspensionReason in Db.NsuspensionReasons
                                          join suspensionType in Db.NsuspensionTypes on suspensionReason.SuspensionTypeId equals suspensionType.Id
                                          where suspensionType.Code == nameof(CommercialFishingSuspensionTypesEnum.PermanentLicense)
                                                && suspensionReason.Code == nameof(CommercialFishingSuspensionReasonsEnum.ExpiredPermitLicense)
                                          orderby suspensionType.ValidTo descending
                                          select suspensionReason.Id).First();

                foreach (var permitLicenseData in permitLicensesToSuspendData)
                {
                    PermitLicensesRegister dbPermitLicense = permitLicenseData.PermitLicense;

                    // Add suspension history for each permit license

                    PermitLicenseSuspensionChangeHistory oldDbPermitLicenseSuspension = oldDbPermitLicenseSuspensions.Where(x => x.PermitLicenseId == dbPermitLicense.Id)
                                                                                                                     .SingleOrDefault();

                    PermitLicenseSuspensionChangeHistory newPermitLicenseSuspension = null;

                    if (oldDbPermitLicenseSuspension != null)
                    {
                        if (!oldDbPermitLicenseSuspension.IsActive)
                        {
                            oldDbPermitLicenseSuspension.SuspensionValidFrom = suspendBeforeDate;
                            oldDbPermitLicenseSuspension.SuspensionValidTo = suspentionValidTo;
                            oldDbPermitLicenseSuspension.EnactmentDate = suspendBeforeDate;
                            oldDbPermitLicenseSuspension.OrderNumber = "-";
                            oldDbPermitLicenseSuspension.ModifiedByUserId = systemUserId;
                            oldDbPermitLicenseSuspension.ReasonId = suspensionReasonId;
                            oldDbPermitLicenseSuspension.IsActive = true;
                        }
                        else
                        {
                            oldDbPermitLicenseSuspension.SuspensionValidTo = suspendBeforeDate;

                            newPermitLicenseSuspension = new PermitLicenseSuspensionChangeHistory
                            {
                                PermitLicenseId = dbPermitLicense.Id,
                                SuspensionValidFrom = suspendBeforeDate,
                                SuspensionValidTo = suspentionValidTo,
                                EnactmentDate = suspendBeforeDate,
                                OrderNumber = "-",
                                ModifiedByUserId = systemUserId,
                                ReasonId = suspensionReasonId,
                                IsActive = true
                            };
                        }
                    }
                    else
                    {
                        newPermitLicenseSuspension = new PermitLicenseSuspensionChangeHistory
                        {
                            PermitLicenseId = dbPermitLicense.Id,
                            SuspensionValidFrom = suspendBeforeDate,
                            SuspensionValidTo = suspentionValidTo,
                            EnactmentDate = suspendBeforeDate,
                            OrderNumber = "-",
                            ModifiedByUserId = systemUserId,
                            ReasonId = suspensionReasonId,
                            IsActive = true
                        };
                    }

                    if (newPermitLicenseSuspension != null)
                    {
                        Db.CommercialFishingPermitLicenseSuspensionChangeHistories.Add(newPermitLicenseSuspension);
                    }

                    // Suspend Permit License
                    permitLicenseData.PermitLicense.IsSuspended = true;
                }

                // Suspend all valid log books for all the suspended permit licenses

                HashSet<int> suspendedPermitLicensesIds = permitLicensesToSuspendData.Select(x => x.PermitLicense.Id).ToHashSet();
                SuspendPermitLicensesLogBooks(suspendedPermitLicensesIds);

                Db.SaveChanges();


                if (permitRegistrationNumbers.Any())
                {
                    PermitsDataChanged(serviceProviderFactory, permitRegistrationNumbers);
                }
            }
        }

        // Simple audit

        public SimpleAuditDTO GetPermitSuspensionSimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.CommercialFishingPermitSuspensionChangeHistories, id);
        }

        public SimpleAuditDTO GetPermitLicenseSuspensionSimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.CommercialFishingPermitLicenseSuspensionChangeHistories, id);
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            throw new NotImplementedException();
        }

        private void AddOrEditPermitSuspension(PermitRegister dbPermit, List<PermitSuspensionChangeHistory> dbSuspensions, SuspensionDataDTO suspension, int currentUserId)
        {
            CheckAndThrowIfPermitSuspensionValidToExists(suspension.Id, dbPermit.Id, suspension.ValidTo ?? DefaultConstants.MAX_VALID_DATE);

            DateTime now = DateTime.Now;

            if (suspension.ValidFrom.HasValue)
            {
                suspension.ValidFrom = suspension.ValidFrom.Value;
            }
            else if (suspension.EnactmentDate.HasValue)
            {
                suspension.ValidFrom = suspension.EnactmentDate.Value;
            }
            else
            {
                suspension.ValidFrom = now;
            }

            suspension.ValidTo = suspension.ValidTo.HasValue ? suspension.ValidTo.Value : DefaultConstants.MAX_VALID_DATE;

            if (suspension.Id == null) // New suspension to add
            {
                PermitSuspensionChangeHistory oldDbPermitSuspension = (from permitSuspension in dbSuspensions
                                                                       where permitSuspension.PermitId == dbPermit.Id
                                                                             && permitSuspension.SuspensionValidTo == suspension.ValidTo.Value
                                                                             && !permitSuspension.IsActive
                                                                       select permitSuspension).FirstOrDefault();

                if (oldDbPermitSuspension != null)
                {
                    UpdatePermitSuspensionData(oldDbPermitSuspension, suspension, currentUserId);
                }
                else
                {
                    PermitSuspensionChangeHistory entry = new PermitSuspensionChangeHistory
                    {
                        PermitId = dbPermit.Id
                    };

                    UpdatePermitSuspensionData(entry, suspension, currentUserId);

                    Db.CommercialFishingPermitSuspensionChangeHistories.Add(entry);
                }
            }
            else
            {
                PermitSuspensionChangeHistory dbPermitSuspension = (from permitSuspension in dbSuspensions
                                                                    where permitSuspension.Id == suspension.Id
                                                                    select permitSuspension).First();

                UpdatePermitSuspensionData(dbPermitSuspension, suspension, currentUserId);
            }
        }

        /// <summary>
        /// Has multiple Db.SaveChanges() inside
        /// </summary>
        private void SetPermitSuspensionFlag(List<SuspensionDataDTO> suspensions, PermitRegister dbPermit)
        {
            DateTime now = DateTime.Now;
            bool isPermitSuspended = false;

            if (suspensions.Any(x => x.ValidFrom <= now && x.ValidTo > now && x.IsActive))
            {
                isPermitSuspended = true;
            }
            else
            {
                isPermitSuspended = false;
            }

            PageCodeEnum pageCode = (from appl in Db.Applications
                                     join applType in Db.NapplicationTypes on appl.ApplicationTypeId equals applType.Id
                                     where appl.Id == dbPermit.ApplicationId
                                     select Enum.Parse<PageCodeEnum>(applType.PageCode)).First();
            bool isForPoundNet = pageCode == PageCodeEnum.PoundnetCommFish;

            if (!isForPoundNet && dbPermit.IsSuspended != isPermitSuspended) // there is a change in the isSuspended flag, so maybe a ship MOD is required
            {
                AddShipMODEventIfNeeded(dbPermit, isPermitSuspended);
            }

            if (isPermitSuspended)
            {
                if (dbPermit.IsSuspended != isPermitSuspended)
                {
                    dbPermit.IsSuspended = isPermitSuspended;
                    Db.SaveChanges();

                    SuspendRelatedPermitLicenses(dbPermit.Id);
                }
                else
                {
                    dbPermit.IsSuspended = isPermitSuspended;
                    Db.SaveChanges();

                    UpdateRelatedPermitLicensesSuspensionData(dbPermit.Id);
                }
            }
            else
            {
                if (dbPermit.IsSuspended != isPermitSuspended)
                {
                    dbPermit.IsSuspended = isPermitSuspended;
                    Db.SaveChanges();

                    ActivateRelatedPermitLicenses(dbPermit.Id);
                }
                else
                {
                    dbPermit.IsSuspended = isPermitSuspended;
                    Db.SaveChanges();

                    UpdateRelatedPermitLicensesSuspensionData(dbPermit.Id);
                }
            }
        }

        private void SuspendRelatedPermitLicenses(int permitId)
        {
            DateTime now = DateTime.Now;

            // Get last valid suspension for permit

            var lastValidPermitSuspensionType = (from permitSuspension in Db.CommercialFishingPermitSuspensionChangeHistories
                                                 join suspensionReason in Db.NsuspensionReasons on permitSuspension.ReasonId equals suspensionReason.Id
                                                 join suspensionType in Db.NsuspensionTypes on suspensionReason.SuspensionTypeId equals suspensionType.Id
                                                 where permitSuspension.PermitId == permitId
                                                       && permitSuspension.IsActive
                                                       && permitSuspension.SuspensionValidFrom <= now
                                                       && permitSuspension.SuspensionValidTo > now
                                                 orderby permitSuspension.SuspensionValidTo descending
                                                 select new
                                                 {
                                                     EnacmentDate = permitSuspension.EnactmentDate,
                                                     OrderNumber = permitSuspension.OrderNumber,
                                                     ValidFrom = permitSuspension.SuspensionValidFrom,
                                                     ValidTo = permitSuspension.SuspensionValidTo,
                                                     ModifiedUserId = permitSuspension.ModifiedByUserId
                                                 }).First();

            // Get all valid active not suspended permit licenses

            List<PermitLicensesRegister> permitLicensesToSuspend = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                                                    where permitLicense.RecordType == nameof(RecordTypesEnum.Register)
                                                                          && permitLicense.IsActive
                                                                          && permitLicense.PermitId == permitId
                                                                          && (!permitLicense.IsSuspended
                                                                              || !(from susp in Db.CommercialFishingPermitLicenseSuspensionChangeHistories
                                                                                   where susp.PermitLicenseId == permitLicense.Id
                                                                                         && susp.IsActive
                                                                                         && susp.SuspensionValidTo >= lastValidPermitSuspensionType.ValidTo
                                                                                   select 1).Any())
                                                                    select permitLicense).ToList();

            if (permitLicensesToSuspend.Count > 0)
            {
                int permitLicenseReasonId = (from suspensionReason in Db.NsuspensionReasons
                                             join suspensionType in Db.NsuspensionTypes on suspensionReason.SuspensionTypeId equals suspensionType.Id
                                             where suspensionType.Code == nameof(CommercialFishingSuspensionTypesEnum.PermitSuspendedLicense)
                                             orderby suspensionType.ValidTo descending
                                             select suspensionReason.Id).First();

                foreach (PermitLicensesRegister permitLicense in permitLicensesToSuspend) // Add suspension history for each permit license
                {
                    DateTime permitLicenseSuspensionValidFrom = lastValidPermitSuspensionType.ValidFrom;
                    DateTime permitLicenseSuspensionValidToDate = lastValidPermitSuspensionType.ValidTo;

                    if (permitLicenseSuspensionValidFrom <= now && permitLicenseSuspensionValidToDate > now)
                    {
                        permitLicense.IsSuspended = true; // Suspend License
                    }

                    PermitLicenseSuspensionChangeHistory oldDbPermitLicenseSuspension = (from suspension in Db.CommercialFishingPermitLicenseSuspensionChangeHistories
                                                                                         where suspension.PermitLicenseId == permitLicense.Id
                                                                                               && suspension.SuspensionValidTo == permitLicenseSuspensionValidToDate
                                                                                               && !suspension.IsActive
                                                                                         select suspension).FirstOrDefault();

                    if (oldDbPermitLicenseSuspension != null)
                    {
                        oldDbPermitLicenseSuspension.SuspensionValidFrom = permitLicenseSuspensionValidFrom;
                        oldDbPermitLicenseSuspension.SuspensionValidTo = permitLicenseSuspensionValidToDate;
                        oldDbPermitLicenseSuspension.EnactmentDate = lastValidPermitSuspensionType.EnacmentDate;
                        oldDbPermitLicenseSuspension.OrderNumber = lastValidPermitSuspensionType.OrderNumber;
                        oldDbPermitLicenseSuspension.ModifiedByUserId = lastValidPermitSuspensionType.ModifiedUserId;
                        oldDbPermitLicenseSuspension.ReasonId = permitLicenseReasonId;
                        oldDbPermitLicenseSuspension.IsActive = true;
                    }
                    else
                    {
                        PermitLicenseSuspensionChangeHistory permitLicenseSuspension = new PermitLicenseSuspensionChangeHistory
                        {
                            PermitLicense = permitLicense,
                            SuspensionValidFrom = permitLicenseSuspensionValidFrom,
                            SuspensionValidTo = permitLicenseSuspensionValidToDate,
                            EnactmentDate = lastValidPermitSuspensionType.EnacmentDate,
                            OrderNumber = lastValidPermitSuspensionType.OrderNumber,
                            ModifiedByUserId = lastValidPermitSuspensionType.ModifiedUserId,
                            ReasonId = permitLicenseReasonId,
                            IsActive = true
                        };

                        Db.CommercialFishingPermitLicenseSuspensionChangeHistories.Add(permitLicenseSuspension);
                    }
                }

                // Get all valid log books for the suspended permit licenses and change their status to SuspLic

                HashSet<int> permitLicenseIds = permitLicensesToSuspend.Where(x => x.IsSuspended).Select(x => x.Id).ToHashSet();
                SuspendPermitLicensesLogBooks(permitLicenseIds);
            }
        }

        private void AddOrEditPermitLicenseSuspension(PermitLicensesRegister dbPermitLicense,
                                                      List<PermitLicenseSuspensionChangeHistory> dbSuspensions,
                                                      SuspensionDataDTO suspension,
                                                      int currentUserId)
        {
            CheckAndThrowIfPermitLicenseSuspensionValidToExists(suspension.Id, dbPermitLicense.Id, suspension.ValidTo ?? DefaultConstants.MAX_VALID_DATE);

            suspension.ValidFrom = suspension.ValidFrom.HasValue ? suspension.ValidFrom.Value : suspension.EnactmentDate.Value;
            suspension.ValidTo = suspension.ValidTo.HasValue ? suspension.ValidTo.Value : DefaultConstants.MAX_VALID_DATE;

            if (suspension.Id == null) // new suspension
            {
                PermitLicenseSuspensionChangeHistory oldEntry = (from susp in dbSuspensions
                                                                 where susp.PermitLicenseId == dbPermitLicense.Id
                                                                       && susp.SuspensionValidTo == suspension.ValidTo.Value
                                                                       && !susp.IsActive
                                                                 select susp).FirstOrDefault();

                if (oldEntry != null)
                {
                    UpdatePermitLicenseSuspnesionData(oldEntry, suspension, currentUserId);
                }
                else
                {
                    PermitLicenseSuspensionChangeHistory entry = new PermitLicenseSuspensionChangeHistory
                    {
                        PermitLicense = dbPermitLicense
                    };

                    UpdatePermitLicenseSuspnesionData(entry, suspension, currentUserId);

                    Db.CommercialFishingPermitLicenseSuspensionChangeHistories.Add(entry);
                }
            }
            else // edit suspension
            {
                PermitLicenseSuspensionChangeHistory dbPermitSuspension = (from permitSuspension in dbSuspensions
                                                                           where permitSuspension.Id == suspension.Id
                                                                           select permitSuspension).First();

                UpdatePermitLicenseSuspnesionData(dbPermitSuspension, suspension, currentUserId);
            }
        }

        private void SetPermitLicenseSuspensionFlag(List<SuspensionDataDTO> suspensions, PermitLicensesRegister dbPermitLicense)
        {
            DateTime now = DateTime.Now;
            bool isPermitLicenseSuspended = false;

            if (suspensions.Any(x => x.ValidFrom <= now && x.ValidTo > now && x.IsActive))
            {
                isPermitLicenseSuspended = true;

                if (dbPermitLicense.IsSuspended != isPermitLicenseSuspended) // The permit license is suspended now
                {
                    SuspendPermitLicensesLogBooks(new HashSet<int> { dbPermitLicense.Id }); // Suspend related valid log books
                }
            }
            else
            {
                isPermitLicenseSuspended = false;

                if (dbPermitLicense.IsSuspended != isPermitLicenseSuspended) // The permit licenses is activated now
                {
                    ActivateSuspendedLogBooks(new HashSet<int> { dbPermitLicense.Id }); // Activate related suspended log books
                }
            }

            dbPermitLicense.IsSuspended = isPermitLicenseSuspended;
        }

        /// <summary>
        /// Gets all active unfinished log books for the suspended permit licenses and changes their status to SuspLic
        /// </summary>
        /// <param name="permitLicenseIds"></param>
        private void SuspendPermitLicensesLogBooks(HashSet<int> permitLicenseIds)
        {
            DateTime now = DateTime.Now;

            List<LogBook> logBooksToSuspend = (from logBook in Db.LogBooks
                                               join logBookPermitLicense in Db.LogBookPermitLicenses on logBook.Id equals logBookPermitLicense.LogBookId
                                               join logBookStatus in Db.NlogBookStatuses on logBook.StatusId equals logBookStatus.Id
                                               where logBook.IsActive
                                                     && logBookStatus.Code != nameof(LogBookStatusesEnum.Finished)
                                                     && permitLicenseIds.Contains(logBookPermitLicense.PermitLicenseRegisterId)
                                                     && logBookPermitLicense.LogBookValidFrom <= now
                                                     && logBookPermitLicense.LogBookValidTo > now
                                               select logBook).ToList();

            if (logBooksToSuspend.Count > 0)
            {
                int suspLicStatusId = (from lbStatus in Db.NlogBookStatuses
                                       where lbStatus.Code == nameof(LogBookStatusesEnum.SuspLic)
                                       orderby lbStatus.ValidTo descending
                                       select lbStatus.Id).First();

                foreach (LogBook logBook in logBooksToSuspend)
                {
                    logBook.StatusId = suspLicStatusId;
                }
            }
        }

        private void ActivateRelatedPermitLicenses(int permitId)
        {
            DateTime now = DateTime.Now;

            List<PermitLicenseSuspensionChangeHistory> licenseSuspensionsToUpdate = GetPermitLicenseSuspensionsToUpdate(permitId);

            if (licenseSuspensionsToUpdate.Count > 0)
            {
                HashSet<int> suspendedLicensesIds = licenseSuspensionsToUpdate.Select(x => x.PermitLicenseId).ToHashSet();

                List<PermitLicensesRegister> suspendedPermitLicenses = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                                                        where permitLicense.RecordType == nameof(RecordTypesEnum.Register)
                                                                              && permitLicense.IsActive
                                                                              && permitLicense.IsSuspended
                                                                              && permitLicense.PermitId == permitId
                                                                              && suspendedLicensesIds.Contains(permitLicense.Id)
                                                                        select permitLicense).ToList();

                if (suspendedPermitLicenses.Count > 0)
                {
                    var permitSuspensionData = (from permit in Db.CommercialFishingPermitRegisters
                                                join suspension in Db.CommercialFishingPermitSuspensionChangeHistories on permit.Id equals suspension.PermitId
                                                where permit.Id == permitId
                                                orderby suspension.SuspensionValidTo descending
                                                select new
                                                {
                                                    EnacmentDate = suspension.EnactmentDate,
                                                    ValidFrom = suspension.SuspensionValidFrom,
                                                    ValidTo = suspension.SuspensionValidTo,
                                                    OrderNumber = suspension.OrderNumber,
                                                    ModifiedByUserId = suspension.ModifiedByUserId,
                                                    IsActive = suspension.IsActive
                                                }).First();

                    foreach (PermitLicensesRegister permitLicense in suspendedPermitLicenses)
                    {
                        permitLicense.IsSuspended = false;

                        // Update suspension history data
                        var suspensions = licenseSuspensionsToUpdate.Where(x => x.PermitLicenseId == permitLicense.Id)
                                                                    .Select(x => x)
                                                                    .ToList();

                        if (permitSuspensionData.IsActive)
                        {
                            foreach (PermitLicenseSuspensionChangeHistory suspension in suspensions)
                            {
                                suspension.EnactmentDate = permitSuspensionData.EnacmentDate;
                                suspension.SuspensionValidFrom = permitSuspensionData.ValidFrom;
                                suspension.SuspensionValidTo = permitSuspensionData.ValidTo;
                                suspension.OrderNumber = permitSuspensionData.OrderNumber;
                                suspension.ModifiedByUserId = permitSuspensionData.ModifiedByUserId;
                            }
                        }
                        else
                        {
                            foreach (PermitLicenseSuspensionChangeHistory suspension in suspensions)
                            {
                                suspension.IsActive = false;
                            }
                        }
                    }

                    // Get all suspneded log books for all the suspnded permit licenses and make them NEW again

                    HashSet<int> permitLicenseIds = suspendedPermitLicenses.Select(x => x.Id).ToHashSet();
                    ActivateSuspendedLogBooks(permitLicenseIds);
                }
            }
        }

        /// <summary>
        /// Gets the permit license suspensions which are with a reason SUSPENDED_PERMIT for a current suspension
        /// </summary>
        /// <param name="permitId">Id of suspended permit</param>
        /// <returns></returns>
        private List<PermitLicenseSuspensionChangeHistory> GetPermitLicenseSuspensionsToUpdate(int permitId)
        {
            DateTime now = DateTime.Now;

            List<PermitLicenseSuspensionChangeHistory> result = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                                                 join licenseSuspension in Db.CommercialFishingPermitLicenseSuspensionChangeHistories on permitLicense.Id equals licenseSuspension.PermitLicenseId
                                                                 join suspensionReason in Db.NsuspensionReasons on licenseSuspension.ReasonId equals suspensionReason.Id
                                                                 join suspensionType in Db.NsuspensionTypes on suspensionReason.SuspensionTypeId equals suspensionType.Id
                                                                 where permitLicense.PermitId == permitId
                                                                       && permitLicense.IsActive
                                                                       && permitLicense.RecordType == nameof(RecordTypesEnum.Register)
                                                                       && permitLicense.IsSuspended
                                                                       && licenseSuspension.IsActive
                                                                       && suspensionType.Code == nameof(CommercialFishingSuspensionTypesEnum.PermitSuspendedLicense)
                                                                 select licenseSuspension).ToList();

            return result;
        }

        /// <summary>
        /// Gets all suspneded log books for all the suspnded permit licenses and makes them in status NEW again
        /// </summary>
        /// <param name="permitLicenseIds">Permit licenses for which suspended log books will be activated</param>
        private void ActivateSuspendedLogBooks(HashSet<int> permitLicenseIds)
        {
            DateTime now = DateTime.Now;

            List<LogBook> logBooksToActivate = (from logBook in Db.LogBooks
                                                join logBookPermitLicense in Db.LogBookPermitLicenses on logBook.Id equals logBookPermitLicense.LogBookId
                                                join logBookStatus in Db.NlogBookStatuses on logBook.StatusId equals logBookStatus.Id
                                                where logBook.IsActive
                                                      && logBookStatus.Code == nameof(LogBookStatusesEnum.SuspLic)
                                                      && permitLicenseIds.Contains(logBookPermitLicense.PermitLicenseRegisterId)
                                                      && logBookPermitLicense.LogBookValidFrom <= now
                                                      && logBookPermitLicense.LogBookValidTo > now
                                                select logBook).ToList();

            if (logBooksToActivate.Count > 0)
            {
                int newLogBookStatusId = (from lbStatus in Db.NlogBookStatuses
                                          where lbStatus.Code == nameof(LogBookStatusesEnum.New)
                                          orderby lbStatus.ValidTo descending
                                          select lbStatus.Id).First();

                foreach (LogBook logBook in logBooksToActivate)
                {
                    logBook.StatusId = newLogBookStatusId;
                }
            }
        }

        private void UpdatePermitLicenseSuspnesionData(PermitLicenseSuspensionChangeHistory dbEntry, SuspensionDataDTO suspension, int currentUserId)
        {
            dbEntry.SuspensionValidFrom = suspension.ValidFrom.Value;
            dbEntry.SuspensionValidTo = suspension.ValidTo.Value;
            dbEntry.EnactmentDate = suspension.EnactmentDate.Value;
            dbEntry.OrderNumber = suspension.OrderNumber;
            dbEntry.ReasonId = suspension.ReasonId.Value;
            dbEntry.ModifiedByUserId = currentUserId;
            dbEntry.IsActive = suspension.IsActive;
        }

        private void UpdatePermitSuspensionData(PermitSuspensionChangeHistory dbPermitSuspension, SuspensionDataDTO suspension, int currentUserId)
        {
            DateTime now = DateTime.Now;

            dbPermitSuspension.SuspensionValidFrom = suspension.ValidFrom.Value;
            dbPermitSuspension.SuspensionValidTo = suspension.ValidTo.Value;
            dbPermitSuspension.EnactmentDate = suspension.EnactmentDate.HasValue ? suspension.EnactmentDate.Value : now;
            dbPermitSuspension.OrderNumber = suspension.OrderNumber;
            dbPermitSuspension.ReasonId = suspension.ReasonId.Value;
            dbPermitSuspension.IsActive = suspension.IsActive;
            dbPermitSuspension.ModifiedByUserId = currentUserId;
        }

        private void UpdateRelatedPermitLicensesSuspensionData(int permitId)
        {
            var permitSuspensionData = (from permit in Db.CommercialFishingPermitRegisters
                                        join suspension in Db.CommercialFishingPermitSuspensionChangeHistories on permit.Id equals suspension.PermitId
                                        where permit.Id == permitId
                                        orderby suspension.SuspensionValidTo descending
                                        select new
                                        {
                                            EnacmentDate = suspension.EnactmentDate,
                                            ValidFrom = suspension.SuspensionValidFrom,
                                            ValidTo = suspension.SuspensionValidTo,
                                            OrderNumber = suspension.OrderNumber,
                                            ModifiedByUserId = suspension.ModifiedByUserId,
                                            IsActive = suspension.IsActive
                                        }).First();

            List<PermitLicenseSuspensionChangeHistory> licenseSuspensionsToUpdate = GetPermitLicenseSuspensionsToUpdate(permitId);

            // Update permit license suspension with the last permit suspension data

            if (permitSuspensionData.IsActive)
            {
                foreach (PermitLicenseSuspensionChangeHistory suspension in licenseSuspensionsToUpdate)
                {
                    suspension.EnactmentDate = permitSuspensionData.EnacmentDate;
                    suspension.SuspensionValidFrom = permitSuspensionData.ValidFrom;
                    suspension.SuspensionValidTo = permitSuspensionData.ValidTo;
                    suspension.OrderNumber = permitSuspensionData.OrderNumber;
                    suspension.ModifiedByUserId = permitSuspensionData.ModifiedByUserId;
                }
            }
            else
            {
                foreach (PermitLicenseSuspensionChangeHistory suspension in licenseSuspensionsToUpdate)
                {
                    suspension.IsActive = false;
                }
            }
        }

        private void AddShipMODEventIfNeeded(PermitRegister dbPermit, bool isPermitSuspended)
        {
            List<int> shipValidPermitIds = CommercialFishingHelper.GetValidShipPermitIds(Db, dbPermit.ShipId);
            if (isPermitSuspended && shipValidPermitIds.Count == 1) // this was the only valid permit for the ship
            {
                shipsRegisterService.EditShipRsr(dbPermit.ShipId, dbPermit.ApplicationId, false);
            }
            else if (!isPermitSuspended && shipValidPermitIds.Count == 0) // there was no valid permit, this is the newly valid one
            {
                dbPermit.ShipId = shipsRegisterService.EditShipRsr(dbPermit.ShipId, dbPermit.ApplicationId, true);
            }
        }

        private void PermitsDataChanged(IScopedServiceProviderFactory serviceProviderFactory, List<string> registrationNumbers)
        {
            using (IScopedServiceProvider serviceProvider = serviceProviderFactory.GetServiceProvider())
            {
                IPermitsAndLicencesService permitsAndLicencesService = serviceProvider.GetRequiredService<IPermitsAndLicencesService>();
                IFVMSReceiverIntegrationService fvmsIntegrationService = serviceProvider.GetRequiredService<IFVMSReceiverIntegrationService>();
                List<FVMSModels.ExternalModels.Permit> fvmsPermits = permitsAndLicencesService.GetPermits(registrationNumbers);
                fvmsIntegrationService.EnqueuePermitsChange(fvmsPermits);
            }
        }

        private void CheckAndThrowIfPermitSuspensionValidToExists(int? suspensionId, int permitId, DateTime validTo)
        {
            bool exists = (from permitSuspension in Db.CommercialFishingPermitSuspensionChangeHistories
                           where (!suspensionId.HasValue || permitSuspension.Id != suspensionId)
                                  && permitSuspension.PermitId == permitId
                                  && permitSuspension.SuspensionValidTo == validTo
                                  && permitSuspension.IsActive
                           select 1).Any();

            if (exists)
            {
                throw new PermitSuspensionValidToAlreadyExistsException(validTo);
            }
        }

        private void CheckAndThrowIfPermitLicenseSuspensionValidToExists(int? suspensionId, int permitLicenseId, DateTime validTo)
        {
            bool exists = (from permitLicenseSuspension in Db.CommercialFishingPermitLicenseSuspensionChangeHistories
                           where (!suspensionId.HasValue || permitLicenseSuspension.Id != suspensionId)
                                  && permitLicenseSuspension.PermitLicenseId == permitLicenseId
                                  && permitLicenseSuspension.SuspensionValidTo == validTo
                                  && permitLicenseSuspension.IsActive
                           select 1).Any();

            if (exists)
            {
                throw new PermitLicenseSuspensionValidToAlreadyExistsException(validTo);
            }
        }
    }
}
