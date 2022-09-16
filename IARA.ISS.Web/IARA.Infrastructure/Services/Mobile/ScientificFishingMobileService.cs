using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.ScientificFishing;
using IARA.DomainModels.Nomenclatures;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;

namespace IARA.Infrastructure.Services
{
    public class ScientificFishingMobileService : Service, IScientificFishingMobileService
    {
        private readonly IPersonService personService;

        public ScientificFishingMobileService(IARADbContext dbContext, IPersonService personService) : base(dbContext)
        {
            this.personService = personService;
        }

        public ScientificFishingMobileDataDTO GetAllPermits(int currentUserId)
        {
            EgnLncDTO egn = Db.GetUserEgn(currentUserId);

            List<int> personIds = (
                from person in this.Db.Persons
                where person.EgnLnc == egn.EgnLnc
                    && person.IdentifierType == egn.IdentifierType.ToString()
                select person.Id
            ).ToList();

            List<ScientificFishingPermitMobileDTO> permits = (
                from permit in this.Db.ScientificPermitRegisters
                join application in this.Db.Applications on permit.ApplicationId equals application.Id
                join owner in this.Db.ScientificPermitOwners on permit.Id equals owner.ScientificPermitId
                join requester in this.Db.Persons on application.SubmittedByPersonId.Value equals requester.Id
                join legal in this.Db.Legals on application.SubmittedForLegalId.Value equals legal.Id
                where permit.IsActive
                    && personIds.Contains(owner.OwnerId)
                    && permit.RecordType == nameof(RecordTypesEnum.Register)
                orderby permit.Id descending
                select new ScientificFishingPermitMobileDTO
                {
                    Id = permit.Id,
                    RegistrationDate = permit.PermitRegistrationDateTime,
                    RequesterFirstName = requester.FirstName,
                    RequesterMiddleName = requester.MiddleName,
                    RequesterLastName = requester.LastName,
                    RequesterEgn = requester.EgnLnc == egn.EgnLnc ? requester.EgnLnc : string.Empty,
                    ValidFrom = permit.PermitValidFrom,
                    ValidTo = permit.PermitValidTo,
                    //IsAllowedDuringMatingSeason = permit.IsAllowedDuringMatingSeason,
                    RequesterScientificOrganizationName = legal.Name,
                    RequesterPosition = permit.SubmittedByPersonPosition,
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
                    CoordinationCommittee = permit.CoordinationCommittee,
                    CoordinationLetterNo = permit.CoordinationLetterNo,
                    CoordinationDate = permit.CoordinationDate
                }
            ).ToList();

            if (permits.Count == 0)
            {
                return null;
            }

            List<int> permitIds = permits.ConvertAll(f => f.Id);

            List<ScientificFishingPermitHolderMobileDTO> permitHolders = this.GetPermitHolders(permitIds);
            (List<ScientificFishingOutingDTO> permitOutings, List<ScientificFishingOutingCatchDTO> catches) = this.GetPermitOutingsAndCatches(permitIds);
            List<ScientificFishingPermitReasonDTO> permitReasons = this.GetPermitReasons(permitIds);

            return new ScientificFishingMobileDataDTO
            {
                Permits = permits,
                Holders = permitHolders,
                Outings = permitOutings,
                PermitReasons = permitReasons,
                Catches = catches
            };
        }

        public int AddOuting(ScientificFishingOutingDTO outing, int currentUserId)
        {
            List<int> personIds = this.GetUserPersonIds(currentUserId);

            var permit = (
                from perm in this.Db.ScientificPermitRegisters
                join owner in this.Db.ScientificPermitOwners on perm.Id equals owner.ScientificPermitId
                where perm.Id == outing.PermitId.Value
                    && personIds.Contains(owner.OwnerId)
                    && perm.IsActive
                select new
                {
                    Permit = perm,
                    Owner = owner
                }
            ).SingleOrDefault();

            if (permit == null)
            {
                return -1;
            }

            ScientificPermitOuting newOuting = new ScientificPermitOuting
            {
                ScientificPermit = permit.Permit,
                OutingDate = outing.DateOfOuting.Value,
                WaterAreaDesc = outing.WaterArea
            };

            foreach (ScientificFishingOutingCatchDTO outingCatch in outing.Catches)
            {
                this.Db.ScientificPermitOutingCatches.Add(new ScientificPermitOutingCatch
                {
                    ScientificPermitOuting = newOuting,
                    FishId = outingCatch.FishType.Value,
                    CatchUnder100 = outingCatch.CatchUnder100.Value,
                    Catch100To500 = outingCatch.Catch100To500.Value,
                    Catch500To1000 = outingCatch.Catch500To1000.Value,
                    CatchOver1000 = outingCatch.CatchOver1000.Value,
                    TotalKeptCount = outingCatch.TotalKeptCount.Value
                });
            }

            this.Db.ScientificPermitOutings.Add(newOuting);
            this.Db.SaveChanges();
            return newOuting.Id;
        }

        public void EditOuting(ScientificFishingOutingDTO outing, int currentUserId)
        {
            List<int> personIds = this.GetUserPersonIds(currentUserId);

            bool ownsPermit = (
                from perm in this.Db.ScientificPermitRegisters
                join owner in this.Db.ScientificPermitOwners on perm.Id equals owner.ScientificPermitId
                where perm.Id == outing.PermitId.Value
                    && personIds.Contains(owner.OwnerId)
                    && perm.IsActive
                select owner
            ).Any();

            if (!ownsPermit)
            {
                return;
            }

            ScientificPermitOuting dbOuting = (
                from sOuting in this.Db.ScientificPermitOutings
                where outing.Id == sOuting.Id
                select sOuting
            ).Single();

            dbOuting.OutingDate = outing.DateOfOuting.Value;
            dbOuting.WaterAreaDesc = outing.WaterArea;
            dbOuting.IsActive = outing.IsActive.Value;

            foreach (ScientificFishingOutingCatchDTO outingCatch in outing.Catches)
            {
                ScientificPermitOutingCatch existing = (
                    from oCatch in this.Db.ScientificPermitOutingCatches
                    where oCatch.Id == outingCatch.Id
                    select oCatch
                ).SingleOrDefault();

                if (existing != null)
                {
                    existing.FishId = outingCatch.FishType.Value;
                    existing.CatchUnder100 = outingCatch.CatchUnder100.Value;
                    existing.Catch100To500 = outingCatch.Catch100To500.Value;
                    existing.Catch500To1000 = outingCatch.Catch500To1000.Value;
                    existing.CatchOver1000 = outingCatch.CatchOver1000.Value;
                    existing.TotalKeptCount = outingCatch.TotalKeptCount.Value;
                    existing.IsActive = outingCatch.IsActive.Value;
                }
                else
                {
                    this.Db.ScientificPermitOutingCatches.Add(new ScientificPermitOutingCatch
                    {
                        ScientificPermitOuting = dbOuting,
                        FishId = outingCatch.FishType.Value,
                        CatchUnder100 = outingCatch.CatchUnder100.Value,
                        Catch100To500 = outingCatch.Catch100To500.Value,
                        Catch500To1000 = outingCatch.Catch500To1000.Value,
                        CatchOver1000 = outingCatch.CatchOver1000.Value,
                        TotalKeptCount = outingCatch.TotalKeptCount.Value
                    });
                }
            }

            this.Db.SaveChanges();
        }

        public void DeleteOuting(int id, int currentUserId)
        {
            List<int> personIds = this.GetUserPersonIds(currentUserId);

            ScientificPermitOuting dbOuting = (
                from sOuting in this.Db.ScientificPermitOutings
                where sOuting.Id == id
                select sOuting
            ).Single();

            bool ownsPermit = (
                from perm in this.Db.ScientificPermitRegisters
                join owner in this.Db.ScientificPermitOwners on perm.Id equals owner.ScientificPermitId
                where perm.Id == dbOuting.ScientificPermitId
                    && personIds.Contains(owner.OwnerId)
                    && perm.IsActive
                select owner
            ).Any();

            if (!ownsPermit)
            {
                return;
            }

            dbOuting.IsActive = false;

            this.Db.ScientificPermitOutings.Update(dbOuting);
            this.Db.SaveChanges();
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return this.GetSimpleEntityAuditValues(this.Db.ScientificPermitRegisters, id);
        }

        private List<ScientificFishingPermitHolderMobileDTO> GetPermitHolders(List<int> permitIds)
        {
            List<int> personIds = (
                from owner in this.Db.ScientificPermitOwners
                where permitIds.Contains(owner.ScientificPermitId)
                select owner.OwnerId
            ).ToList();

            Dictionary<int, RegixPersonDataDTO> regixData = this.personService.GetRegixPersonsData(personIds);

            List<ScientificFishingPermitHolderMobileDTO> result = (
                from permitOwner in this.Db.ScientificPermitOwners
                join person in this.Db.Persons on permitOwner.OwnerId equals person.Id
                where permitIds.Contains(permitOwner.ScientificPermitId)
                    && permitOwner.IsActive
                orderby permitOwner.Id
                select new ScientificFishingPermitHolderMobileDTO
                {
                    Id = permitOwner.Id,
                    OwnerId = permitOwner.OwnerId,
                    RequestNumber = permitOwner.ScientificPermitId,
                    ScientificPosition = permitOwner.RequestedByOrganizationPosition
                }
            ).ToList();

            foreach (ScientificFishingPermitHolderMobileDTO holder in result)
            {
                RegixPersonDataDTO regixPersonData = regixData[holder.OwnerId];
                holder.Name = $"{regixPersonData.FirstName} {regixPersonData.MiddleName} {regixPersonData.LastName}";
            }

            return result;
        }

        private (List<ScientificFishingOutingDTO>, List<ScientificFishingOutingCatchDTO>) GetPermitOutingsAndCatches(List<int> permitIds)
        {
            List<ScientificFishingOutingDTO> outings = (
                from outing in this.Db.ScientificPermitOutings
                where permitIds.Contains(outing.ScientificPermitId)
                    && outing.IsActive
                orderby outing.OutingDate descending
                select new ScientificFishingOutingDTO
                {
                    Id = outing.Id,
                    PermitId = outing.ScientificPermitId,
                    DateOfOuting = outing.OutingDate,
                    WaterArea = outing.WaterAreaDesc,
                    IsActive = outing.IsActive
                }
            ).ToList();

            List<int> outingIds = outings.ConvertAll(x => x.Id.Value);

            List<ScientificFishingOutingCatchDTO> catches = (
                from oCatch in this.Db.ScientificPermitOutingCatches
                join fishType in this.Db.Nfishes on oCatch.FishId equals fishType.Id
                where outingIds.Contains(oCatch.ScientificPermitOutingId)
                    && oCatch.IsActive
                orderby oCatch.Id
                select new ScientificFishingOutingCatchDTO
                {
                    Id = oCatch.Id,
                    OutingId = oCatch.ScientificPermitOutingId,
                    FishType = new NomenclatureDTO
                    {
                        Value = fishType.Id,
                        DisplayName = fishType.Name
                    },
                    CatchUnder100 = oCatch.CatchUnder100,
                    Catch100To500 = oCatch.Catch100To500,
                    Catch500To1000 = oCatch.Catch500To1000,
                    CatchOver1000 = oCatch.CatchOver1000,
                    TotalKeptCount = oCatch.TotalKeptCount,
                    TotalCatch = oCatch.CatchUnder100
                                    + oCatch.Catch100To500
                                    + oCatch.Catch500To1000
                                    + oCatch.CatchOver1000,
                    IsActive = oCatch.IsActive
                }
            ).ToList();

            return (outings, catches);
        }

        private List<ScientificFishingPermitReasonDTO> GetPermitReasons(List<int> permitIds)
        {
            return (
                from reason in this.Db.ScientificPermitReasons
                where permitIds.Contains(reason.ScientificPermitId)
                    && reason.IsActive
                select new ScientificFishingPermitReasonDTO
                {
                    PermitReasonId = reason.ReasonId,
                    PermitId = reason.ScientificPermitId
                }
            ).ToList();
        }

        private List<int> GetUserPersonIds(int currentUserId)
        {
            EgnLncDTO egn = Db.GetUserEgn(currentUserId);

            return (
                from person in this.Db.Persons
                where person.EgnLnc == egn.EgnLnc && person.IdentifierType == egn.IdentifierType.ToString()
                select person.Id
            ).ToList();
        }
    }
}
