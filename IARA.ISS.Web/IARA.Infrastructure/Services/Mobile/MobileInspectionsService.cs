using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.ControlActivity.Inspections;
using IARA.DomainModels.DTOModels.Mobile.Inspections;
using IARA.DomainModels.DTOModels.Mobile.Ships;
using IARA.DomainModels.Nomenclatures;
using IARA.Interfaces.Mobile;

namespace IARA.Infrastructure.Services.Mobile
{
    public class MobileInspectionsService : Service, IMobileInspectionsService
    {
        public MobileInspectionsService(IARADbContext dbContext)
            : base(dbContext)
        {
        }

        public bool IsDeviceAllowed(string imei)
        {
            bool result = (
                from umd in this.Db.UserMobileDevices
                where umd.Imei == imei
                    && umd.IsActive
                    && umd.AccessStatus == nameof(UserMobileDeviceAccessStatusEnum.Approved)
                select umd
            ).Any();

            return result;
        }

        public List<InspectorDTO> GetInspectors(DateTime? afterDate)
        {
            DateTime now = DateTime.Now;

            List<InspectorDTO> registeredInspectors = (
                from inspector in this.Db.Inspectors
                join user in this.Db.Users on inspector.UserId equals user.Id
                join person in this.Db.Persons on user.PersonId equals person.Id
                where (afterDate.HasValue
                        ? ((inspector.IsActive && inspector.CreatedOn > afterDate.Value)
                            || inspector.UpdatedOn > afterDate.Value)
                        : inspector.IsActive)
                    && (inspector.UserId != null || inspector.UnregisteredPersonId != null)
                select new InspectorDTO
                {
                    InspectorId = inspector.Id,
                    UserId = user.Id,
                    IsNotRegistered = true,
                    InstitutionId = inspector.InstitutionId,
                    FirstName = person.FirstName,
                    MiddleName = person.MiddleName,
                    LastName = person.LastName,
                    CardNum = inspector.InspectorCardNum,
                    CitizenshipId = person.CitizenshipCountryId,
                    IsActive = inspector.IsActive,
                }
            ).ToList();

            List<InspectorDTO> unregisteredInspectors = (
                from inspector in this.Db.Inspectors
                join unregisteredPerson in this.Db.UnregisteredPersons on inspector.UnregisteredPersonId equals unregisteredPerson.Id
                where (afterDate.HasValue
                        ? ((inspector.IsActive && inspector.CreatedOn > afterDate.Value)
                            || inspector.UpdatedOn > afterDate.Value)
                        : inspector.IsActive)
                    && (inspector.UserId != null || inspector.UnregisteredPersonId != null)
                select new InspectorDTO
                {
                    InspectorId = inspector.Id,
                    UnregisteredPersonId = unregisteredPerson.Id,
                    IsNotRegistered = false,
                    InstitutionId = inspector.InstitutionId,
                    FirstName = unregisteredPerson.FirstName,
                    MiddleName = unregisteredPerson.MiddleName,
                    LastName = unregisteredPerson.LastName,
                    CardNum = inspector.InspectorCardNum,
                    CitizenshipId = unregisteredPerson.CitizenshipCountryId,
                    IsActive = inspector.IsActive,
                }).ToList();

            return registeredInspectors.Concat(unregisteredInspectors).ToList();
        }

        public List<NomenclatureDTO> GetPoundNets(DateTime? afterDate)
        {
            List<NomenclatureDTO> poundNets = (
                from poundNet in this.Db.PoundNetRegisters
                join status in this.Db.NpoundNetStatuses on poundNet.StatusId equals status.Id
                where !afterDate.HasValue
                    ? (poundNet.IsActive && status.Code == nameof(PoundNetStatusesEnum.Active))
                    : ((poundNet.IsActive && status.Code == nameof(PoundNetStatusesEnum.Active) && poundNet.CreatedOn > afterDate.Value)
                        || poundNet.UpdatedOn > afterDate.Value)
                select new NomenclatureDTO
                {
                    Value = poundNet.Id,
                    DisplayName = poundNet.Name,
                    IsActive = poundNet.IsActive,
                }).ToList();

            return poundNets;
        }

        public List<PoundNetPermitLicenseDTO> GetPoundNetPermitLicenses(DateTime? afterDate)
        {
            DateTime now = DateTime.Now;

            List<PoundNetPermitLicenseDTO> result = (
                from poundNet in this.Db.PoundNetRegisters
                join status in this.Db.NpoundNetStatuses on poundNet.StatusId equals status.Id
                join plr in this.Db.CommercialFishingPermitLicensesRegisters on poundNet.Id equals plr.PoundNetId
                join pr in this.Db.CommercialFishingPermitRegisters on plr.PermitId equals pr.Id
                join fisher in this.Db.FishermenRegisters on plr.QualifiedFisherId equals fisher.Id
                where plr.RecordType == nameof(RecordTypesEnum.Register)
                    && (afterDate.HasValue ? ((
                            // Взимаме записите които са неактивни в момента, но са били активни на дадената дата
                            (plr.PermitLicenseValidFrom.Value <= afterDate.Value && plr.PermitLicenseValidTo.Value > afterDate.Value && plr.PermitLicenseValidTo.Value <= now)
                            // Взимаме записите които са станали активни след дадената дата и все още са активни
                            || (plr.PermitLicenseValidFrom.Value > afterDate.Value && plr.PermitLicenseValidTo.Value > now)
                        ) && (
                            (pr.IsActive && pr.CreatedOn > afterDate.Value)
                            || pr.UpdatedOn > afterDate.Value
                        ) && (
                            (plr.IsActive && plr.CreatedOn > afterDate.Value)
                            || plr.UpdatedOn > afterDate.Value
                        ))
                        : (plr.PermitLicenseValidFrom.Value <= now && plr.PermitLicenseValidTo.Value > now
                            && plr.IsActive
                            && pr.IsActive)
                    ) && poundNet.IsActive
                    && status.Code == nameof(PoundNetStatusesEnum.Active)
                select new PoundNetPermitLicenseDTO
                {
                    Id = plr.Id,
                    PoundNetId = poundNet.Id,
                    LegalId = plr.SubmittedForLegalId,
                    PersonId = plr.SubmittedForPersonId,
                    PermitNumber = pr.RegistrationNum,
                    LicenseNumber = plr.RegistrationNum,
                    TypeId = plr.PermitLicenseTypeId,
                    ValidFrom = plr.PermitLicenseValidFrom.Value,
                    ValidTo = plr.PermitLicenseValidTo.Value,
                    IsActive = plr.PermitLicenseValidFrom.Value <= now && plr.PermitLicenseValidTo.Value > now
                        && plr.IsActive
                        && pr.IsActive,
                }
            ).ToList();

            return result;
        }

        public List<FishingGearInspectionDTO> GetPoundNetFishingGears(DateTime? afterDate)
        {
            DateTime now = DateTime.Now;

            List<FishingGearInspectionDTO> result = (
                from poundNet in this.Db.PoundNetRegisters
                join status in this.Db.NpoundNetStatuses on poundNet.StatusId equals status.Id
                join permit in this.Db.CommercialFishingPermitLicensesRegisters on poundNet.Id equals permit.PoundNetId
                join fishingGear in this.Db.FishingGearRegisters on permit.Id equals fishingGear.PermitLicenseId
                where fishingGear.InspectionId == null
                    && (!afterDate.HasValue
                        ? fishingGear.IsActive
                        : ((fishingGear.CreatedOn > afterDate.Value && fishingGear.IsActive)
                            || fishingGear.UpdatedOn > afterDate.Value))
                    && permit.PermitLicenseValidFrom.Value <= now && permit.PermitLicenseValidTo.Value > now
                    && permit.IsActive
                    && poundNet.IsActive
                    && status.Code == nameof(PoundNetStatusesEnum.Active)
                select new FishingGearInspectionDTO
                {
                    Id = fishingGear.Id,
                    PermitId = permit.Id,
                    SubjectId = poundNet.Id,
                    Count = fishingGear.GearCount,
                    Height = fishingGear.Height,
                    Length = fishingGear.Length,
                    HookCount = fishingGear.HookCount,
                    CordThickness = fishingGear.CordThickness,
                    HouseLength = fishingGear.HouseLength,
                    HouseWidth = fishingGear.HouseWidth,
                    NetEyeSize = fishingGear.NetEyeSize,
                    TowelLength = fishingGear.TowelLength,
                    TypeId = fishingGear.FishingGearTypeId,
                    Description = fishingGear.Description,
                    IsActive = fishingGear.IsActive,
                }
            ).ToList();

            return result;
        }

        public List<FishingGearMarkInspectionDTO> GetPoundNetFishingGearsMarks(DateTime? afterDate)
        {
            DateTime now = DateTime.Now;

            List<FishingGearMarkInspectionDTO> result = (
                from poundNet in this.Db.PoundNetRegisters
                join status in this.Db.NpoundNetStatuses on poundNet.StatusId equals status.Id
                join permit in this.Db.CommercialFishingPermitLicensesRegisters on poundNet.Id equals permit.PoundNetId
                join fishingGear in this.Db.FishingGearRegisters on permit.Id equals fishingGear.PermitLicenseId
                join fishingGearMark in this.Db.FishingGearMarks on fishingGear.Id equals fishingGearMark.FishingGearId
                where (afterDate.HasValue
                        ? ((fishingGearMark.CreatedOn > afterDate.Value && fishingGearMark.IsActive)
                            || fishingGearMark.UpdatedOn > afterDate.Value)
                        : fishingGearMark.IsActive)
                    && fishingGear.InspectionId == null
                    && fishingGear.IsActive
                    && permit.PermitLicenseValidFrom.Value <= now && permit.PermitLicenseValidTo.Value > now
                    && permit.IsActive
                    && poundNet.IsActive
                    && status.Code == nameof(PoundNetStatusesEnum.Active)
                select new FishingGearMarkInspectionDTO
                {
                    Id = fishingGearMark.Id,
                    Number = fishingGearMark.MarkNum,
                    StatusId = fishingGearMark.MarkStatusId,
                    FishingGearId = fishingGearMark.FishingGearId,
                    IsActive = fishingGearMark.IsActive,
                }
            ).ToList();

            return result;
        }

        public List<FishingGearPingerInspectionDTO> GetPoundNetFishingGearsPingers(DateTime? afterDate)
        {
            DateTime now = DateTime.Now;

            List<FishingGearPingerInspectionDTO> result = (
                from poundNet in this.Db.PoundNetRegisters
                join status in this.Db.NpoundNetStatuses on poundNet.StatusId equals status.Id
                join permit in this.Db.CommercialFishingPermitLicensesRegisters on poundNet.Id equals permit.PoundNetId
                join fishingGear in this.Db.FishingGearRegisters on permit.Id equals fishingGear.PermitLicenseId
                join fishingGearPinger in this.Db.FishingGearPingers on fishingGear.Id equals fishingGearPinger.FishingGearId
                where (afterDate.HasValue
                        ? ((fishingGearPinger.CreatedOn > afterDate.Value && fishingGearPinger.IsActive)
                            || fishingGearPinger.UpdatedOn > afterDate.Value)
                        : fishingGearPinger.IsActive)
                    && fishingGear.InspectionId == null
                    && fishingGear.IsActive
                    && permit.PermitLicenseValidFrom.Value <= now && permit.PermitLicenseValidTo.Value > now
                    && permit.IsActive
                    && poundNet.IsActive
                    && status.Code == nameof(PoundNetStatusesEnum.Active)
                select new FishingGearPingerInspectionDTO
                {
                    Id = fishingGearPinger.Id,
                    Number = fishingGearPinger.PingerNum,
                    StatusId = fishingGearPinger.PingerStatusId,
                    FishingGearId = fishingGearPinger.FishingGearId,
                    IsActive = fishingGearPinger.IsActive,
                }
            ).ToList();

            return result;
        }

        public List<PatrolVehicleNomenclatureDTO> GetPatrolVehicles(DateTime? afterDate)
        {
            List<PatrolVehicleNomenclatureDTO> result = (
                from ship in this.Db.UnregisteredVessels
                join pvt in this.Db.NpatrolVehicleTypes on ship.PatrolVehicleTypeId equals pvt.Id
                where afterDate.HasValue
                    ? ((ship.IsActive && ship.CreatedOn > afterDate.Value)
                        || ship.UpdatedOn > afterDate.Value)
                    : ship.IsActive
                select new PatrolVehicleNomenclatureDTO
                {
                    Value = ship.Id,
                    RegistrationNumber = ship.Cfr,
                    DisplayName = ship.Name,
                    Code = ship.ExternalMark,
                    Description = ship.IrcscallSign,
                    InstitutionId = ship.InstitutionId,
                    FlagId = ship.FlagCountryId,
                    PatrolVehicleTypeId = ship.PatrolVehicleTypeId,
                    VehicleType = Enum.Parse<PatrolVehicleTypeEnum>(pvt.VehicleType),
                    IsActive = ship.IsActive
                }
            ).ToList();

            return result;
        }

        public List<ShipMobileDTO> GetShips(DateTime? afterDate)
        {
            DateTime now = DateTime.Now;

            List<ShipMobileDTO> ships = (
                from ship in this.Db.ShipsRegister
                where afterDate.HasValue ? ((
                        // Взимаме корабите които са неактивни в момента, но са били активни на дадената дата
                        (ship.ValidFrom <= afterDate.Value && ship.ValidTo > afterDate.Value && ship.ValidTo <= now)
                        // Взимаме корабите които са станали активни след дадената дата и все още са активни
                        || (ship.ValidFrom > afterDate.Value && ship.ValidTo > now)
                    ) && (
                        (ship.CancellationDetailsId == null && ship.CreatedOn > afterDate.Value)
                        || (ship.UpdatedOn > afterDate.Value)
                    ))
                    : (ship.ValidFrom <= now && ship.ValidTo > now
                        && ship.CancellationDetailsId == null)
                select new ShipMobileDTO
                {
                    Id = ship.Id,
                    Uid = ship.ShipUid,
                    AssociationId = ship.ShipAssociationId,
                    CFR = ship.Cfr,
                    ExtMarkings = ship.ExternalMark,
                    Name = ship.Name,
                    CallSign = ship.IrcscallSign,
                    FlagId = ship.FlagCountryId,
                    MMSI = ship.Mmsi,
                    ShipTypeId = ship.VesselTypeId,
                    UVI = ship.Uvi,
                    FleetTypeId = ship.FleetTypeId,
                    IsCancelled = ship.CancellationDetailsId != null,
                    IsActive = ship.ValidFrom <= now && ship.ValidTo > now
                }
            ).ToList();

            return ships;
        }

        public List<PermitLicenseMobileDTO> GetPermitLicenses(DateTime? afterDate)
        {
            DateTime now = DateTime.Now;

            List<PermitLicenseMobileDTO> result = (
                from rootShip in this.Db.ShipsRegister
                join ship in this.Db.ShipsRegister on rootShip.ShipUid equals ship.ShipUid
                join plr in this.Db.CommercialFishingPermitLicensesRegisters on ship.Id equals plr.ShipId
                join pr in this.Db.CommercialFishingPermitRegisters on plr.PermitId equals pr.Id
                join fisher in this.Db.FishermenRegisters on plr.QualifiedFisherId equals fisher.Id
                where plr.RecordType == nameof(RecordTypesEnum.Register)
                    && (afterDate.HasValue ? ((
                            // Взимаме записите които са неактивни в момента, но са били активни на дадената дата
                            (plr.PermitLicenseValidFrom.Value <= afterDate.Value && plr.PermitLicenseValidTo.Value > afterDate.Value && plr.PermitLicenseValidTo.Value <= now)
                            // Взимаме записите които са станали активни след дадената дата и все още са активни
                            || (plr.PermitLicenseValidFrom.Value > afterDate.Value && plr.PermitLicenseValidTo.Value > now)
                        ) && (
                            (pr.IsActive && pr.CreatedOn > afterDate.Value)
                            || pr.UpdatedOn > afterDate.Value
                        ) && (
                            (plr.IsActive && plr.CreatedOn > afterDate.Value)
                            || plr.UpdatedOn > afterDate.Value
                        ))
                        : (plr.PermitLicenseValidFrom.Value <= now && plr.PermitLicenseValidTo.Value > now
                            && plr.IsActive
                            && pr.IsActive)
                    ) && rootShip.ValidFrom <= now && rootShip.ValidTo > now
                    && rootShip.CancellationDetailsId == null
                select new PermitLicenseMobileDTO
                {
                    Id = plr.Id,
                    ShipUid = rootShip.ShipUid,
                    LegalId = plr.SubmittedForLegalId,
                    PersonId = plr.SubmittedForPersonId,
                    CaptainId = plr.QualifiedFisherId,
                    PersonCaptainId = fisher.PersonId,
                    PermitNumber = pr.RegistrationNum,
                    LicenseNumber = plr.RegistrationNum,
                    TypeId = plr.PermitLicenseTypeId,
                    ValidFrom = plr.PermitLicenseValidFrom.Value,
                    ValidTo = plr.PermitLicenseValidTo.Value,
                    IsActive = plr.PermitLicenseValidFrom.Value <= now && plr.PermitLicenseValidTo.Value > now
                        && plr.IsActive
                        && pr.IsActive,
                }
            ).ToList();

            return result;
        }

        public List<ShipOwnerMobileDTO> GetShipsOwners(DateTime? afterDate)
        {
            DateTime now = DateTime.Now;

            List<ShipOwnerMobileDTO> result = (
                from ship in this.Db.ShipsRegister
                join owner in this.Db.ShipOwners on ship.Id equals owner.ShipRegisterId
                where (afterDate.HasValue ? (
                            (owner.IsActive && owner.CreatedOn > afterDate.Value)
                            || owner.UpdatedOn > afterDate.Value
                        )
                        : owner.IsActive)
                    && ship.ValidFrom <= now && ship.ValidTo > now
                    && ship.CancellationDetailsId == null
                select new ShipOwnerMobileDTO
                {
                    Id = owner.Id,
                    PersonId = owner.OwnerPersonId,
                    LegalId = owner.OwnerLegalId,
                    ShipUid = ship.ShipUid,
                    IsActive = owner.IsActive,
                }).ToList();

            return result;
        }

        public List<PersonMobileDTO> GetPersons(List<int> personIds)
        {
            DateTime now = DateTime.Now;

            List<PersonMobileDTO> result = (
                from person in this.Db.Persons
                join address in (
                    from personAddress in this.Db.PersonAddresses
                    join address in this.Db.Addresses on personAddress.AddressId equals address.Id
                    join addressType in this.Db.NaddressTypes on personAddress.AddressTypeId equals addressType.Id
                    where addressType.Code == nameof(AddressTypesEnum.PERMANENT)
                        && address.IsActive
                        && personAddress.IsActive
                    select new ShipPersonnelAddressMobileDTO
                    {
                        PersonLegalId = personAddress.PersonId,
                        ApartmentNum = address.ApartmentNum,
                        BlockNum = address.BlockNum,
                        CountryId = address.CountryId,
                        DistrictId = address.DistrictId,
                        EntranceNum = address.EntranceNum,
                        FloorNum = address.FloorNum,
                        MunicipalityId = address.MunicipalityId,
                        PopulatedAreaId = address.PopulatedAreaId,
                        Street = address.Street,
                        StreetNum = address.StreetNum,
                        PostalCode = address.PostCode,
                        Region = address.Region,
                    }) on person.Id equals address.PersonLegalId into agrp
                from address in agrp.DefaultIfEmpty()
                where personIds.Contains(person.Id)
                select new PersonMobileDTO
                {
                    Id = person.Id,
                    FirstName = person.FirstName,
                    MiddleName = person.MiddleName,
                    LastName = person.LastName,
                    EgnLnc = new EgnLncDTO
                    {
                        EgnLnc = person.EgnLnc,
                        IdentifierType = Enum.Parse<IdentifierTypeEnum>(person.IdentifierType)
                    },
                    Address = address.PersonLegalId.HasValue ? address : null,
                }).ToList();

            return result;
        }

        public List<LegalMobileDTO> GetLegals(List<int> legalIds)
        {
            DateTime now = DateTime.Now;

            List<LegalMobileDTO> result = (
                from legal in this.Db.Legals
                join address in (
                    from legalAddress in this.Db.LegalsAddresses
                    join address in this.Db.Addresses on legalAddress.AddressId equals address.Id
                    join addressType in this.Db.NaddressTypes on legalAddress.AddressTypeId equals addressType.Id
                    where addressType.Code == nameof(AddressTypesEnum.COMPANY_HEADQUARTERS)
                        && address.IsActive
                        && legalAddress.IsActive
                    select new ShipPersonnelAddressMobileDTO
                    {
                        PersonLegalId = legalAddress.LegalId,
                        ApartmentNum = address.ApartmentNum,
                        BlockNum = address.BlockNum,
                        CountryId = address.CountryId,
                        DistrictId = address.DistrictId,
                        EntranceNum = address.EntranceNum,
                        FloorNum = address.FloorNum,
                        MunicipalityId = address.MunicipalityId,
                        PopulatedAreaId = address.PopulatedAreaId,
                        Street = address.Street,
                        StreetNum = address.StreetNum,
                        PostalCode = address.PostCode,
                        Region = address.Region,
                    }) on legal.Id equals address.PersonLegalId into agrp
                from address in agrp.DefaultIfEmpty()
                where legalIds.Contains(legal.Id)
                select new LegalMobileDTO
                {
                    Id = legal.Id,
                    Name = legal.Name,
                    Eik = legal.Eik,
                    Address = address.PersonLegalId.HasValue ? address : null,
                }).ToList();

            return result;
        }

        public List<FishingGearInspectionDTO> GetShipsFishingGears(DateTime? afterDate)
        {
            DateTime now = DateTime.Now;

            List<FishingGearInspectionDTO> result = (
                from rootShip in this.Db.ShipsRegister
                join ship in this.Db.ShipsRegister on rootShip.ShipUid equals ship.ShipUid
                join permit in this.Db.CommercialFishingPermitLicensesRegisters on ship.Id equals permit.ShipId
                join fishingGear in this.Db.FishingGearRegisters on permit.Id equals fishingGear.PermitLicenseId
                where fishingGear.InspectionId == null
                    && (afterDate.HasValue
                        ? ((fishingGear.IsActive && fishingGear.CreatedOn > afterDate.Value)
                            || fishingGear.UpdatedOn > afterDate.Value)
                        : fishingGear.IsActive)
                    && permit.PermitLicenseValidFrom.Value <= now && permit.PermitLicenseValidTo.Value > now
                    && permit.IsActive
                    && rootShip.ValidFrom <= now && rootShip.ValidTo > now
                    && rootShip.CancellationDetailsId == null
                select new FishingGearInspectionDTO
                {
                    Id = fishingGear.Id,
                    PermitId = permit.Id,
                    SubjectId = rootShip.ShipUid,
                    Count = fishingGear.GearCount,
                    Height = fishingGear.Height,
                    Length = fishingGear.Length,
                    HookCount = fishingGear.HookCount,
                    HouseLength = fishingGear.HouseLength,
                    HouseWidth = fishingGear.HouseWidth,
                    NetEyeSize = fishingGear.NetEyeSize,
                    TowelLength = fishingGear.TowelLength,
                    CordThickness = fishingGear.CordThickness,
                    TypeId = fishingGear.FishingGearTypeId,
                    Description = fishingGear.Description,
                    IsActive = fishingGear.IsActive,
                }
            ).ToList();

            return result;
        }

        public List<FishingGearMarkInspectionDTO> GetFishingGearsMarks(DateTime? afterDate)
        {
            DateTime now = DateTime.Now;

            List<FishingGearMarkInspectionDTO> result = (
                from rootShip in this.Db.ShipsRegister
                join ship in this.Db.ShipsRegister on rootShip.ShipUid equals ship.ShipUid
                join permit in this.Db.CommercialFishingPermitLicensesRegisters on ship.Id equals permit.ShipId
                join fishingGear in this.Db.FishingGearRegisters on permit.Id equals fishingGear.PermitLicenseId
                join fishingGearMark in this.Db.FishingGearMarks on fishingGear.Id equals fishingGearMark.FishingGearId
                where (afterDate.HasValue
                        ? ((fishingGearMark.CreatedOn > afterDate.Value && fishingGearMark.IsActive)
                            || fishingGearMark.UpdatedOn > afterDate.Value)
                        : fishingGearMark.IsActive)
                    && fishingGear.InspectionId == null
                    && fishingGear.IsActive
                    && permit.PermitLicenseValidFrom.Value <= now && permit.PermitLicenseValidTo.Value > now
                    && permit.IsActive
                    && rootShip.ValidFrom <= now && rootShip.ValidTo > now
                    && rootShip.CancellationDetailsId == null
                select new FishingGearMarkInspectionDTO
                {
                    Id = fishingGearMark.Id,
                    Number = fishingGearMark.MarkNum,
                    StatusId = fishingGearMark.MarkStatusId,
                    FishingGearId = fishingGearMark.FishingGearId,
                    IsActive = fishingGearMark.IsActive,
                }
            ).ToList();

            return result;
        }

        public List<FishingGearPingerInspectionDTO> GetFishingGearsPingers(DateTime? afterDate)
        {
            DateTime now = DateTime.Now;

            List<FishingGearPingerInspectionDTO> result = (
                from rootShip in this.Db.ShipsRegister
                join ship in this.Db.ShipsRegister on rootShip.ShipUid equals ship.ShipUid
                join permit in this.Db.CommercialFishingPermitLicensesRegisters on ship.Id equals permit.ShipId
                join fishingGear in this.Db.FishingGearRegisters on permit.Id equals fishingGear.PermitLicenseId
                join fishingGearPinger in this.Db.FishingGearPingers on fishingGear.Id equals fishingGearPinger.FishingGearId
                where (afterDate.HasValue
                        ? ((fishingGearPinger.CreatedOn > afterDate.Value && fishingGearPinger.IsActive)
                            || fishingGearPinger.UpdatedOn > afterDate.Value)
                        : fishingGearPinger.IsActive)
                    && fishingGear.InspectionId == null
                    && fishingGear.IsActive
                    && permit.PermitLicenseValidFrom.Value <= now && permit.PermitLicenseValidTo.Value > now
                    && permit.IsActive
                    && rootShip.ValidFrom <= now && rootShip.ValidTo > now
                    && rootShip.CancellationDetailsId == null
                select new FishingGearPingerInspectionDTO
                {
                    Id = fishingGearPinger.Id,
                    Number = fishingGearPinger.PingerNum,
                    StatusId = fishingGearPinger.PingerStatusId,
                    FishingGearId = fishingGearPinger.FishingGearId,
                    IsActive = fishingGearPinger.IsActive,
                }
            ).ToList();

            return result;
        }

        public List<LogBookMobileDTO> GetLogBooks(DateTime? afterDate)
        {
            DateTime now = DateTime.Now;

            List<LogBookMobileDTO> result = (
                from rootShip in this.Db.ShipsRegister
                join ship in this.Db.ShipsRegister on rootShip.ShipUid equals ship.ShipUid
                join logBook in this.Db.LogBooks on ship.Id equals logBook.ShipId
                join logBookStatus in this.Db.NlogBookStatuses on logBook.StatusId equals logBookStatus.Id
                where (afterDate.HasValue ? (
                            (logBook.IsActive && logBook.FinishDate == null && logBookStatus.Code != nameof(LogBookStatusesEnum.Finished) && logBook.CreatedOn > afterDate.Value)
                            || logBook.UpdatedOn > afterDate.Value
                        )
                        : (logBook.IsActive
                            && logBook.FinishDate == null
                            && logBookStatus.Code != nameof(LogBookStatusesEnum.Finished))
                    ) && rootShip.ValidFrom <= now && rootShip.ValidTo > now
                    && rootShip.CancellationDetailsId == null
                select new LogBookMobileDTO
                {
                    Id = logBook.Id,
                    Number = logBook.LogNum,
                    IssuedOn = logBook.IssueDate,
                    StartPage = logBook.StartPageNum,
                    EndPage = logBook.EndPageNum,
                    ShipUid = ship.ShipUid,
                    IsActive = logBook.IsActive
                        && logBook.FinishDate == null
                        && logBookStatus.Code != nameof(LogBookStatusesEnum.Finished),
                }
            ).ToList();

            return result;
        }

        public List<LogBookPageMobileDTO> GetLogBookPages(List<int> logBookIds)
        {
            DateTime now = DateTime.Now;

            List<LogBookPageMobileDTO> result = (
                from rootShip in this.Db.ShipsRegister
                join ship in this.Db.ShipsRegister on rootShip.ShipUid equals ship.ShipUid
                join logBook in this.Db.LogBooks on ship.Id equals logBook.ShipId
                join logBookPage in this.Db.ShipLogBookPages on logBook.Id equals logBookPage.LogBookId
                where logBookIds.Contains(logBook.Id)
                    && logBookPage.Status == nameof(LogBookPageStatusesEnum.Submitted)
                    && rootShip.ValidFrom <= now && rootShip.ValidTo > now
                select new LogBookPageMobileDTO
                {
                    Id = logBookPage.Id,
                    LogBookId = logBook.Id,
                    PageNum = logBookPage.PageNum,
                }
            ).ToList();

            return result;
        }

        public List<BuyerMobileDTO> GetBuyers(DateTime? afterDate)
        {
            List<BuyerMobileDTO> result = (
                from buyer in this.Db.BuyerRegisters
                join address in (
                    from legalAddress in this.Db.LegalsAddresses
                    join address in this.Db.Addresses on legalAddress.AddressId equals address.Id
                    join addressType in this.Db.NaddressTypes on legalAddress.AddressTypeId equals addressType.Id
                    where addressType.Code == nameof(AddressTypesEnum.COMPANY_HEADQUARTERS)
                        && address.IsActive
                        && legalAddress.IsActive
                    select new ShipPersonnelAddressMobileDTO
                    {
                        PersonLegalId = legalAddress.LegalId,
                        ApartmentNum = address.ApartmentNum,
                        BlockNum = address.BlockNum,
                        CountryId = address.CountryId,
                        DistrictId = address.DistrictId,
                        EntranceNum = address.EntranceNum,
                        FloorNum = address.FloorNum,
                        MunicipalityId = address.MunicipalityId,
                        PopulatedAreaId = address.PopulatedAreaId,
                        Street = address.Street,
                        StreetNum = address.StreetNum,
                        PostalCode = address.PostCode,
                        Region = address.Region,
                    }) on buyer.SubmittedForLegalId equals address.PersonLegalId into agrp
                from address in agrp.DefaultIfEmpty()
                where (afterDate.HasValue ? (
                        (buyer.IsActive && buyer.CreatedOn > afterDate.Value)
                            || buyer.UpdatedOn > afterDate.Value
                        )
                        : buyer.IsActive
                    ) && buyer.RecordType == nameof(RecordTypesEnum.Register)
                    && !(buyer.SubmittedForPersonId == null && buyer.SubmittedForLegalId == null)
                select new BuyerMobileDTO
                {
                    Id = buyer.Id,
                    LegalId = buyer.SubmittedForLegalId,
                    PersonId = buyer.SubmittedForPersonId,
                    HasUtility = buyer.HasUtility == true,
                    HasVehicle = buyer.HasVehicle == true,
                    UtilityName = buyer.UtilityName,
                    UtilityAddress = address.PersonLegalId.HasValue ? address : null,
                    VehicleNumber = buyer.VehicleNumber,
                    IsActive = buyer.IsActive
                }
            ).ToList();

            return result;
        }

        public List<DeclarationLogBookPageDTO> GetDeclarationLogBookPages(DeclarationLogBookTypeEnum type, int shipUid)
        {
            HashSet<int> shipLogBookIds = (
                from ship in this.Db.ShipsRegister
                join logBook in this.Db.LogBooks on ship.Id equals logBook.ShipId
                where ship.ShipUid == shipUid
                select logBook.Id
            ).ToHashSet();

            List<DeclarationLogBookPageDTO> result = type switch
            {
                DeclarationLogBookTypeEnum.FirstSaleLogBook => (
                    from fslbp in this.Db.FirstSaleLogBookPages
                    where shipLogBookIds.Contains(fslbp.LogBookId)
                        && fslbp.Status == nameof(LogBookPageStatusesEnum.Submitted)
                    select new DeclarationLogBookPageDTO
                    {
                        Id = fslbp.Id,
                        Num = fslbp.PageNum.ToString(),
                        Date = fslbp.SaleDate.Value,
                    }
                ).ToList(),
                DeclarationLogBookTypeEnum.TransportationLogBook => (
                    from tlbp in this.Db.TransportationLogBookPages
                    where shipLogBookIds.Contains(tlbp.LogBookId)
                        && tlbp.Status == nameof(LogBookPageStatusesEnum.Submitted)
                    select new DeclarationLogBookPageDTO
                    {
                        Id = tlbp.Id,
                        Num = tlbp.PageNum.ToString(),
                        Date = tlbp.LoadingDate.Value,
                    }
                ).ToList(),
                DeclarationLogBookTypeEnum.AdmissionLogBook => (
                    from albp in this.Db.AdmissionLogBookPages
                    where shipLogBookIds.Contains(albp.LogBookId)
                        && albp.Status == nameof(LogBookPageStatusesEnum.Submitted)
                    select new DeclarationLogBookPageDTO
                    {
                        Id = albp.Id,
                        Num = albp.PageNum.ToString(),
                        Date = albp.HandoverDate.Value,
                    }
                ).ToList(),
                DeclarationLogBookTypeEnum.ShipLogBook => (
                    from slbp in this.Db.ShipLogBookPages
                    where shipLogBookIds.Contains(slbp.LogBookId)
                        && slbp.Status == nameof(LogBookPageStatusesEnum.Submitted)
                    select new DeclarationLogBookPageDTO
                    {
                        Id = slbp.Id,
                        Num = slbp.PageNum,
                        Date = slbp.PageFillDate.Value,
                    }
                ).ToList(),
                _ => throw new Exception($"{nameof(DeclarationLogBookTypeEnum)} provided to {nameof(GetDeclarationLogBookPages)} inside {nameof(MobileInspectionsService)} was of wrong type."),
            };

            return result;
        }

        public List<AquacultureMobileDTO> GetAquacultures(DateTime? afterDate)
        {
            List<AquacultureMobileDTO> result = (
                from aqua in this.Db.AquacultureFacilitiesRegister
                where (!afterDate.HasValue
                    ? aqua.IsActive
                    : ((aqua.IsActive && aqua.CreatedOn > afterDate.Value)
                        || aqua.UpdatedOn > afterDate.Value))
                    && aqua.RecordType == nameof(RecordTypesEnum.Register)
                    && aqua.SubmittedForLegalId != null
                select new AquacultureMobileDTO
                {
                    Id = aqua.Id,
                    UrorNum = aqua.UrorNum,
                    Name = aqua.Name,
                    IsActive = aqua.IsActive,
                    LegalId = aqua.SubmittedForLegalId.Value,
                }
            ).ToList();

            return result;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            throw new NotImplementedException();
        }
    }
}
