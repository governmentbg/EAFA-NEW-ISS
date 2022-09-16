using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.CommercialFishingRegister;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.Nomenclatures;
using IARA.Interfaces.Nomenclatures;

namespace IARA.Infrastructure.Services.Nomenclatures
{
    public class CommercialFishingNomenclaturesService : BaseService, ICommercialFishingNomenclaturesService
    {
        public CommercialFishingNomenclaturesService(IARADbContext dbContext)
            : base(dbContext)
        {
        }

        public List<QualifiedFisherNomenclatureDTO> GetQualifiedFishers()
        {
            DateTime now = DateTime.Now;

            List<QualifiedFisherNomenclatureDTO> qualifiedFishers = (from fisher in this.Db.FishermenRegisters
                                                                     join person in this.Db.Persons on fisher.PersonId equals person.Id
                                                                     where fisher.RecordType == nameof(RecordTypesEnum.Register)
                                                                     orderby person.FirstName, person.LastName
                                                                     select new QualifiedFisherNomenclatureDTO
                                                                     {
                                                                         Value = fisher.Id,
                                                                         DisplayName = $"{person.FirstName} {person.LastName}",
                                                                         RegistrationNumber = fisher.RegistrationNum,
                                                                         IsActive = fisher.IsActive
                                                                     }).ToList();

            return qualifiedFishers;
        }

        public List<NomenclatureDTO> GetWaterTypes()
        {
            return this.GetCodeNomenclature(this.Db.NwaterTypes);
        }

        public List<NomenclatureDTO> GetCommercialFishingPermitTypes()
        {
            DateTime now = DateTime.Now;

            List<NomenclatureDTO> types = (from permitType in this.Db.NcommercialFishingPermitTypes
                                           orderby permitType.ShortName
                                           select new NomenclatureDTO
                                           {
                                               Value = permitType.Id,
                                               DisplayName = permitType.ShortName,
                                               IsActive = permitType.ValidFrom <= now && permitType.ValidTo > now
                                           }).ToList();

            return types;
        }

        public List<NomenclatureDTO> GetCommercialFishingPermitLicenseTypes()
        {
            DateTime now = DateTime.Now;

            List<NomenclatureDTO> types = (from permitLicenseType in this.Db.NcommercialFishingPermitLicenseTypes
                                           orderby permitLicenseType.ShortName
                                           select new NomenclatureDTO
                                           {
                                               Value = permitLicenseType.Id,
                                               DisplayName = permitLicenseType.ShortName,
                                               IsActive = permitLicenseType.ValidFrom <= now && permitLicenseType.ValidTo > now
                                           }).ToList();

            return types;
        }

        public List<NomenclatureDTO> GetHolderGroundForUseTypes()
        {
            return this.GetCodeNomenclature(this.Db.NholderGroundsForUseTypes);
        }

        public List<PoundNetNomenclatureDTO> GetPoundNets()
        {
            DateTime now = DateTime.Now;

            HashSet<int> poundNetIdsWithPoundNetPermit = (from permit in this.Db.CommercialFishingPermitRegisters
                                                          join appl in this.Db.Applications on permit.ApplicationId equals appl.Id
                                                          where permit.PoundNetId.HasValue
                                                                && !permit.IsSuspended
                                                                && (permit.RecordType == nameof(RecordTypesEnum.Application)
                                                                     || (permit.PermitValidFrom <= now && (permit.IsPermitUnlimited.Value || permit.PermitValidTo > now))
                                                                   )
                                                                && permit.IsActive
                                                                && appl.IsActive
                                                          select permit.PoundNetId.Value).ToHashSet();

            List<PoundNetNomenclatureDTO> poundNets = (from poundNet in this.Db.PoundNetRegisters
                                                       join poundNetStatus in this.Db.NpoundNetStatuses on poundNet.StatusId equals poundNetStatus.Id
                                                       select new PoundNetNomenclatureDTO
                                                       {
                                                           Value = poundNet.Id,
                                                           DisplayName = poundNet.Name,
                                                           StatusCode = poundNetStatus.Code,
                                                           StatusName = poundNetStatus.Name,
                                                           HasPoundNetPermit = poundNetIdsWithPoundNetPermit.Contains(poundNet.Id),
                                                           Depth = poundNet.DepthFrom.HasValue && poundNet.DepthTo.HasValue
                                                                   ? $"{poundNet.DepthFrom.Value} - {poundNet.DepthTo.Value}"
                                                                   : poundNet.DepthFrom.HasValue
                                                                     ? $"{poundNet.DepthFrom.Value}"
                                                                     : poundNet.DepthTo.HasValue
                                                                         ? $"{poundNet.DepthTo.Value}"
                                                                         : "",
                                                           IsActive = poundNet.IsActive
                                                       }).ToList();



            return poundNets;
        }

        public List<SuspensionTypeNomenclatureDTO> GetSuspensionTypes()
        {
            DateTime now = DateTime.Now;

            List<SuspensionTypeNomenclatureDTO> suspensionTypes = (from suspensionType in this.Db.NsuspensionTypes
                                                                   select new SuspensionTypeNomenclatureDTO
                                                                   {
                                                                       Value = suspensionType.Id,
                                                                       Code = suspensionType.Code,
                                                                       DisplayName = suspensionType.Name,
                                                                       isPermit = suspensionType.IsPermit,
                                                                       IsActive = suspensionType.ValidFrom <= now && suspensionType.ValidTo > now
                                                                   }).ToList();
            return suspensionTypes;
        }

        public List<SuspensionReasonNomenclatureDTO> GetSuspensionReasons()
        {
            DateTime now = DateTime.Now;

            List<SuspensionReasonNomenclatureDTO> suspensionReasons = (from suspensionReason in this.Db.NsuspensionReasons
                                                                       select new SuspensionReasonNomenclatureDTO
                                                                       {
                                                                           Value = suspensionReason.Id,
                                                                           Code = suspensionReason.Code,
                                                                           DisplayName = suspensionReason.Name,
                                                                           SuspensionTypeId = suspensionReason.SuspensionTypeId,
                                                                           MonthsDuration = suspensionReason.DurationMonths
                                                                       }).ToList();

            return suspensionReasons;
        }

        public List<PermitNomenclatureDTO> GetShipPermits(int shipId, bool onlyPoundNet)
        {
            DateTime now = DateTime.Now;

            int shipUId = this.GetShipUId(shipId);
            List<int> shipIds = this.GetShipIds(shipUId);

            var registerPermitDataIds = (from permit in this.Db.CommercialFishingPermitRegisters
                                         join permitType in this.Db.NcommercialFishingPermitTypes on permit.PermitTypeId equals permitType.Id
                                         where shipIds.Contains(permit.ShipId)
                                               && permit.RecordType == nameof(RecordTypesEnum.Register)
                                               && ((!onlyPoundNet && permitType.Code != nameof(CommercialFishingTypesEnum.PoundNetPermit))
                                                    || permitType.Code == nameof(CommercialFishingTypesEnum.PoundNetPermit)
                                                  )
                                               && permit.PermitValidFrom <= now
                                               && (permit.IsPermitUnlimited.Value || permit.PermitValidTo.Value > now)
                                         select new
                                         {
                                             PermitId = permit.Id,
                                             ApplicationId = permit.ApplicationId
                                         }).ToList();

            HashSet<int> applicationIds = registerPermitDataIds.Select(x => x.ApplicationId).ToHashSet();

            HashSet<int> applicationPermitsWithoutRegisterIds = (from permit in this.Db.CommercialFishingPermitRegisters
                                                                 join permitType in this.Db.NcommercialFishingPermitTypes on permit.PermitTypeId equals permitType.Id
                                                                 join appl in this.Db.Applications on permit.ApplicationId equals appl.Id
                                                                 join applStatus in this.Db.NapplicationStatuses on appl.ApplicationStatusId equals applStatus.Id
                                                                 where shipIds.Contains(permit.ShipId)
                                                                       && permit.RecordType == nameof(RecordTypesEnum.Application)
                                                                       && !applicationIds.Contains(permit.ApplicationId)
                                                                       && ((!onlyPoundNet && permitType.Code != nameof(CommercialFishingTypesEnum.PoundNetPermit))
                                                                            || permitType.Code == nameof(CommercialFishingTypesEnum.PoundNetPermit)
                                                                          )
                                                                       && permit.IsActive
                                                                       && appl.IsActive
                                                                       && applStatus.Code != nameof(ApplicationStatusesEnum.CANCELED_APPL)
                                                                 select permit.Id).ToHashSet();

            HashSet<int> permitIds = registerPermitDataIds.Select(x => x.PermitId).Concat(applicationPermitsWithoutRegisterIds).ToHashSet();

            List<PermitNomenclatureDTO> shipPermits = (from permit in this.Db.CommercialFishingPermitRegisters
                                                       join permitType in this.Db.NcommercialFishingPermitTypes on permit.PermitTypeId equals permitType.Id
                                                       join captain in this.Db.FishermenRegisters on permit.QualifiedFisherId equals captain.Id
                                                       join captainPerson in this.Db.Persons on captain.PersonId equals captainPerson.Id
                                                       join submittedForPerson in this.Db.Persons on permit.SubmittedForPersonId equals submittedForPerson.Id into sbPerson
                                                       from submittedForPerson in sbPerson.DefaultIfEmpty()
                                                       join submittedForLegal in this.Db.Legals on permit.SubmittedForLegalId equals submittedForLegal.Id into sbLegal
                                                       from submittedForLegal in sbLegal.DefaultIfEmpty()
                                                       where permitIds.Contains(permit.Id)
                                                       select new PermitNomenclatureDTO
                                                       {
                                                           Value = permit.Id,
                                                           DisplayName = permit.RegistrationNum,
                                                           Type = Enum.Parse<CommercialFishingTypesEnum>(permitType.Code),
                                                           CaptainName = $"{captainPerson.FirstName} {captainPerson.LastName} ({captain.RegistrationNum})",
                                                           ShipOwnerName = submittedForPerson != null
                                                                           ? $"{submittedForPerson.FirstName} {submittedForPerson.LastName}"
                                                                           : $"{submittedForLegal.Name} ({submittedForLegal.Eik})",
                                                           ShipOwnerPersonId = permit.SubmittedForPersonId,
                                                           ShipOwnerLegalId = permit.SubmittedForLegalId,
                                                           IsActive = permit.IsActive && !permit.IsSuspended
                                                       }).ToList();

            return shipPermits;
        }

        public List<PermitNomenclatureDTO> GetShipPermitLicenses(int shipId)
        {
            DateTime now = DateTime.Now;

            int shipUId = this.GetShipUId(shipId);
            List<int> shipIds = this.GetShipIds(shipUId);

            List<int> registerPermitDataIds = (from permit in this.Db.CommercialFishingPermitLicensesRegisters
                                               where shipIds.Contains(permit.ShipId)
                                                     && permit.RecordType == nameof(RecordTypesEnum.Register)
                                                     && permit.PermitLicenseValidFrom <= now
                                                     && (permit.PermitLicenseValidTo.Value > now)
                                               select permit.Id).ToList();

            List<PermitNomenclatureDTO> shipPermits = (from permit in this.Db.CommercialFishingPermitLicensesRegisters
                                                       join permitType in this.Db.NcommercialFishingPermitLicenseTypes on permit.PermitLicenseTypeId equals permitType.Id
                                                       join captain in this.Db.FishermenRegisters on permit.QualifiedFisherId equals captain.Id
                                                       join captainPerson in this.Db.Persons on captain.PersonId equals captainPerson.Id
                                                       where registerPermitDataIds.Contains(permit.Id)
                                                       select new PermitNomenclatureDTO
                                                       {
                                                           Value = permit.Id,
                                                           DisplayName = permit.RegistrationNum,
                                                           Type = Enum.Parse<CommercialFishingTypesEnum>(permitType.Code),
                                                           CaptainName = $"{captainPerson.FirstName} {captainPerson.LastName} ({captain.RegistrationNum})",
                                                           CaptainId = permit.QualifiedFisherId,
                                                           IsActive = permit.IsActive && !permit.IsSuspended
                                                       }).ToList();

            return shipPermits;
        }

        private int GetShipUId(int shipId)
        {
            return (from ship in this.Db.ShipsRegister
                    where ship.Id == shipId
                    select ship.ShipUid).Single();
        }

        private List<int> GetShipIds(int shipUId)
        {
            return (from ship in this.Db.ShipsRegister
                    where ship.ShipUid == shipUId
                    select ship.Id).ToList();
        }
    }
}
