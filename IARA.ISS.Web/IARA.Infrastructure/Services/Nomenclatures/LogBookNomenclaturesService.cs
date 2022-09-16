using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.CatchesAndSales;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.Nomenclatures;
using IARA.Interfaces.CatchSales;

namespace IARA.Infrastructure.Services.Nomenclatures
{
    public class LogBookNomenclaturesService : BaseService, ILogBookNomenclaturesService
    {
        public LogBookNomenclaturesService(IARADbContext dbContext) : base(dbContext)
        {
        }

        public List<NomenclatureDTO> GetLogBookTypes()
        {
            List<NomenclatureDTO> logBookTypes = GetCodeNomenclature(Db.NlogBookTypes);
            return logBookTypes;
        }

        public List<NomenclatureDTO> GetPermits()
        {
            List<NomenclatureDTO> permits = (from permit in Db.CommercialFishingPermitRegisters
                                             join person in Db.Persons on permit.SubmittedForPersonId equals person.Id into persons
                                             from submittedForPerson in persons.DefaultIfEmpty()
                                             join legal in Db.Legals on permit.SubmittedForLegalId equals legal.Id into legals
                                             from submittedForLegal in legals.DefaultIfEmpty()
                                             where permit.RecordType == nameof(RecordTypesEnum.Register)
                                             select new NomenclatureDTO
                                             {
                                                 Value = permit.Id,
                                                 DisplayName = submittedForPerson != null ? submittedForPerson.FirstName + " " + submittedForPerson.LastName
                                                                                          : submittedForLegal.Name,
                                                 Description = "Номер на разрешително: " + permit.RegistrationNum, // TODO make this string a RESOURCE
                                                 IsActive = permit.IsActive && !permit.IsSuspended,
                                             }).ToList();

            return permits;
        }

        public List<PermitLicenseNomenclatureDTO> GetPermitLicenses()
        {
            DateTime now = DateTime.Now;
            List<PermitLicenseNomenclatureDTO> permitLicenses = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                                                 join permit in Db.CommercialFishingPermitRegisters on permitLicense.PermitId equals permit.Id
                                                                 join person in Db.Persons on permitLicense.SubmittedForPersonId equals person.Id into persons
                                                                 from submittedForPerson in persons.DefaultIfEmpty()
                                                                 join legal in Db.Legals on permitLicense.SubmittedForLegalId equals legal.Id into legals
                                                                 from submittedForLegal in legals.DefaultIfEmpty()
                                                                 where permitLicense.RecordType == nameof(RecordTypesEnum.Register)
                                                                 select new PermitLicenseNomenclatureDTO
                                                                 {
                                                                     Value = permitLicense.Id,
                                                                     DisplayName = submittedForPerson != null ? submittedForPerson.FirstName + " " + submittedForPerson.LastName
                                                                                                              : submittedForLegal.Name,
                                                                     Description = "Номер на разрешително: " + permit.RegistrationNum, // TODO make this string a RESOURCE
                                                                     IsActive = permitLicense.IsActive && !(from license in Db.CommercialFishingPermitLicensesRegisters
                                                                                                            join suspension in Db.CommercialFishingPermitLicenseSuspensionChangeHistories on license.Id equals suspension.PermitLicenseId
                                                                                                            where license.Id == permitLicense.Id
                                                                                                                  && suspension.SuspensionValidFrom <= now
                                                                                                                  && suspension.SuspensionValidTo > now
                                                                                                            select license.Id).Any(),
                                                                     ShipId = permitLicense.ShipId
                                                                 }).ToList();

            return permitLicenses;
        }

        public List<NomenclatureDTO> GetRegisteredBuyers()
        {
            List<NomenclatureDTO> buyers = (from buyer in Db.BuyerRegisters
                                            join legal in Db.Legals on buyer.SubmittedForLegalId equals legal.Id into l
                                            from legal in l.DefaultIfEmpty()
                                            join person in Db.Persons on buyer.SubmittedForPersonId equals person.Id into p
                                            from person in p.DefaultIfEmpty()
                                            where buyer.RecordType == nameof(RecordTypesEnum.Register)
                                            orderby buyer.RegistrationNum
                                            select new NomenclatureDTO
                                            {
                                                Value = buyer.Id,
                                                Code = buyer.RegistrationNum,
                                                DisplayName = legal != null
                                                              ? $"{legal.Name} ({buyer.RegistrationNum})"
                                                              : $"{person.FirstName} {person.LastName} ({buyer.RegistrationNum})",
                                                IsActive = buyer.IsActive
                                            }).ToList();

            return buyers;
        }

        public List<NomenclatureDTO> GetTurbotSizeGroups()
        {
            return GetCodeNomenclature(Db.NturbotSizeGroups);
        }

        public List<NomenclatureDTO> GetFishSizeCategories()
        {
            return GetCodeNomenclature(Db.NfishSizeCategories);
        }

        public List<NomenclatureDTO> GetCatchStates()
        {
            DateTime now = DateTime.Now;
            List<NomenclatureDTO> results = (from state in Db.NfishFreshnesses
                                             orderby state.Name descending
                                             select new NomenclatureDTO
                                             {
                                                 Value = state.Id,
                                                 Code = state.Code,
                                                 DisplayName = state.Name,
                                                 IsActive = state.ValidFrom <= now && state.ValidTo > now
                                             }).ToList();

            return results;
        }

        public List<NomenclatureDTO> GetUnloadTypes()
        {
            return GetCodeNomenclature(Db.NcatchFishUnloadTypes);
        }

        public List<NomenclatureDTO> GetFishPurposes()
        {
            return GetCodeNomenclature(Db.NfishSalePurposes);
        }

        public List<NomenclatureDTO> GetFishSizes()
        {
            return GetCodeNomenclature(Db.NfishSizes);
        }

        public List<NomenclatureDTO> GetCatchTypes()
        {
            return GetCodeNomenclature(Db.NcatchTypes);
        }

        public List<FishingGearRegisterNomenclatureDTO> GetFishingGearsRegister(int permitLicenseId)
        {
            DateTime now = DateTime.Now;

            List<FishingGearRegisterNomenclatureDTO> results = (from fishingGearRegister in Db.FishingGearRegisters
                                                                join fishingGear in Db.NfishingGears on fishingGearRegister.FishingGearTypeId equals fishingGear.Id
                                                                join fishingGearType in Db.NfishingGearTypes on fishingGear.GearTypeId equals fishingGearType.Id
                                                                where fishingGearRegister.PermitLicenseId.HasValue && fishingGearRegister.PermitLicenseId == permitLicenseId
                                                                select new FishingGearRegisterNomenclatureDTO
                                                                {
                                                                    Value = fishingGearRegister.Id,
                                                                    Code = fishingGear.Code,
                                                                    DisplayName = fishingGear.Name,
                                                                    FishingGearId = fishingGear.Id,
                                                                    Type = Enum.Parse<FishingGearParameterTypesEnum>(fishingGear.GearParametersType),
                                                                    IsForMutualFishing = fishingGear.IsForMutualFishing,
                                                                    HasHooks = fishingGearType.HasHooks,
                                                                    HooksCount = fishingGearRegister.HookCount,
                                                                    GearCount = fishingGearRegister.GearCount,
                                                                    NetEyeSize = fishingGearRegister.NetEyeSize,
                                                                    IsActive = fishingGearRegister.IsActive && fishingGear.ValidFrom <= now && fishingGear.ValidTo > now
                                                                }).ToList();

            return results;
        }
    }
}
