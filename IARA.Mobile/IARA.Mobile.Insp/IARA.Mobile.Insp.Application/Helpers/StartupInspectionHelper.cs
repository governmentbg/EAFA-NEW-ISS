using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Interfaces;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures.API;
using IARA.Mobile.Insp.Application.Interfaces.Database;
using IARA.Mobile.Insp.Application.Interfaces.Factories;
using IARA.Mobile.Insp.Application.Interfaces.Utilities;
using IARA.Mobile.Insp.Domain.Entities.Inspections;
using IARA.Mobile.Insp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IARA.Mobile.Insp.Application.Helpers
{
    internal static class StartupInspectionHelper
    {
        private interface IPullData
        {
            Task<bool> Pull(IRestClient restClient, INomenclatureDates nomenclatureDates, IDateTime dateTime, IAppDbContextBuilder contextBuilder);
        }

        private class PullData<TDto, TEntity> : IPullData
            where TDto : IActive
            where TEntity : IEntity, new()
        {
            private readonly NomenclatureEnum nomenclatureEnum;
            private readonly string url;
            private readonly Func<TDto, TEntity> convert;
            private readonly Func<TDto, int> selectId;
            private readonly Action<IAppDbContext, List<TDto>, List<int>, List<int>> before;

            public PullData(NomenclatureEnum nomenclatureEnum, string url, Func<TDto, TEntity> convert, Func<TDto, int> selectId, Action<IAppDbContext, List<TDto>, List<int>, List<int>> before = null)
            {
                this.nomenclatureEnum = nomenclatureEnum;
                this.url = url;
                this.convert = convert;
                this.selectId = selectId;
                this.before = before;
            }

            public async Task<bool> Pull(IRestClient restClient, INomenclatureDates nomenclatureDates, IDateTime dateTime, IAppDbContextBuilder contextBuilder)
            {
                DateTime? date = nomenclatureDates[nomenclatureEnum];
                DateTime now = dateTime.Now;

                HttpResult<List<TDto>> result = await restClient.GetAsync<List<TDto>>(url, new { afterDate = date }, alertOnException: false);

                if (result.IsSuccessful && result.Content?.Count > 0)
                {
                    List<int> inactive = result.Content
                        .Where(f => !f.IsActive)
                        .Select(selectId)
                        .ToList();

                    List<int> active = result.Content
                        .Where(f => f.IsActive)
                        .Select(selectId)
                        .ToList();

                    using (IAppDbContext context = contextBuilder.CreateContext())
                    {
                        before?.Invoke(context, result.Content, active, inactive);

                        TLTableQuery<TEntity> table = context.TLTable<TEntity>();

                        if (inactive.Count > 0)
                        {
                            if (inactive.Count > 1000)
                            {
                                int length = (int)Math.Ceiling(inactive.Count / 1000d);

                                for (int i = 0; i < length; i++)
                                {
                                    List<int> inactivePaged = active
                                        .Skip(i * 1000)
                                        .Take(1000)
                                        .ToList();

                                    table.Delete(f => inactivePaged.Contains(f.Id));
                                }
                            }
                            else
                            {
                                table.Delete(f => inactive.Contains(f.Id));
                            }
                        }

                        if (active.Count > 0)
                        {
                            if (active.Count > 1000)
                            {
                                int length = (int)Math.Ceiling(active.Count / 1000d);

                                for (int i = 0; i < length; i++)
                                {
                                    List<int> activePaged = active
                                        .Skip(i * 1000)
                                        .Take(1000)
                                        .ToList();

                                    table.Delete(f => activePaged.Contains(f.Id));
                                }
                            }
                            else
                            {
                                table.Delete(f => active.Contains(f.Id));
                            }
                        }

                        try
                        {
                            table.AddRange(result.Content
                                .Where(f => f.IsActive)
                                .Select(convert)
                                .ToList()
                            );
                        }
                        catch (Exception ex)
                        {

                            throw;
                        }
                    }

                    nomenclatureDates[nomenclatureEnum] = now;
                }

                return result.IsSuccessful;
            }
        }

        public static Task<bool> MapInspectionNomenclature(IRestClient restClient, INomenclatureDates nomenclatureDates, IDateTime dateTime, IAppDbContextBuilder contextBuilder, NomenclatureEnum nomenclatureEnum, HashSet<int> personsToPull, HashSet<int> legalsToPull, HashSet<int> personsToDelete, HashSet<int> legalsToDelete)
        {
            IPullData pullData;

            switch (nomenclatureEnum)
            {
                case NomenclatureEnum.Catch:
                    pullData = PullCatches();
                    break;
                case NomenclatureEnum.Inspector:
                    pullData = PullInspectors();
                    break;
                case NomenclatureEnum.PatrolVehicle:
                    pullData = PullPatrolVehicles();
                    break;
                case NomenclatureEnum.Ship:
                    pullData = PullShips(personsToDelete, legalsToDelete);
                    break;
                case NomenclatureEnum.ShipOwner:
                    pullData = PullShipsOwners(personsToPull, legalsToPull, personsToDelete, legalsToDelete);
                    break;
                case NomenclatureEnum.ShipFishingGear:
                    pullData = PullShipsFishingGears();
                    break;
                case NomenclatureEnum.FishingGearMark:
                    pullData = PullFishingGearsMarks();
                    break;
                case NomenclatureEnum.PoundNet:
                    pullData = PullPoundNets();
                    break;
                case NomenclatureEnum.PoundNetFishingGear:
                    pullData = PullPoundNetFishingGears();
                    break;
                case NomenclatureEnum.PermitLicense:
                    pullData = PullPermitLicenses(personsToPull, legalsToPull, personsToDelete, legalsToDelete);
                    break;
                case NomenclatureEnum.LogBook:
                    pullData = PullLogBooks();
                    break;
                case NomenclatureEnum.Buyer:
                    pullData = PullBuyers(personsToPull, legalsToPull, personsToDelete, legalsToDelete);
                    break;
                case NomenclatureEnum.Aquaculture:
                    pullData = PullAquacultures(legalsToPull, legalsToDelete);
                    break;
                case NomenclatureEnum.PoundNetPermitLicense:
                    pullData = PullPoundNetPermitLicenses(personsToPull, legalsToPull, personsToDelete, legalsToDelete);
                    break;
                case NomenclatureEnum.PoundNetFishingGearMark:
                    pullData = PullPoundNetFishingGearsMarks();
                    break;
                case NomenclatureEnum.PoundNetFishingGearPinger:
                    pullData = PullPoundNetFishingGearsPingers();
                    break;
                case NomenclatureEnum.FishingGearPinger:
                    pullData = PullFishingGearsPingers();
                    break;
                case NomenclatureEnum.Permit:
                    pullData = PullPermits();
                    break;
                default:
                    throw new NotImplementedException($"{nameof(nomenclatureEnum)} doesn't have specified {nameof(NomenclatureEnum)} implemented ({nomenclatureEnum})");
            }

            return pullData.Pull(restClient, nomenclatureDates, dateTime, contextBuilder);
        }

        private static IPullData PullCatches()
        {
            return new PullData<CatchDto, Catch>(
                NomenclatureEnum.Catch,
                "InspectionData/GetCatches",
                f => new Catch
                {
                    Id = f.Id,
                    ShipUid = f.ShipUid,
                    LogBookId = f.LogBookId,
                    PageNumber = f.PageNumber,
                    FishId = f.FishId,
                    CatchTypeId = f.CatchTypeId,
                    Quantity = f.Quantity,
                    UnloadedQuantity = f.UnloadedQuantity,
                    TurbotSizeGroupId = f.TurbotSizeGroupId,
                    CatchZoneId = f.CatchZoneId,
                },
                f => f.Id
            );
        }

        private static IPullData PullInspectors()
        {
            return new PullData<InspectorDto, Inspector>(
                NomenclatureEnum.Inspector,
                "InspectionData/GetInspectors",
                f => new Inspector
                {
                    Id = f.InspectorId.Value,
                    CitizenshipId = f.CitizenshipId,
                    CardNum = f.CardNum,
                    TerritoryCode = f.TerritoryCode,
                    InstitutionId = f.InstitutionId,
                    IsNotRegistered = false,
                    FirstName = f.FirstName,
                    MiddleName = f.MiddleName,
                    LastName = f.LastName,
                    UnregisteredPersonId = f.UnregisteredPersonId,
                    UserId = f.UserId,
                    NormalizedCardNum = f.CardNum?.ToLower(),
                    NormalizedName = $"{f.FirstName} {f.MiddleName ?? string.Empty} {f.LastName}".ToLower(),
                },
                f => f.InspectorId.Value
            );
        }

        private static IPullData PullPatrolVehicles()
        {
            return new PullData<PatrolVehicleApiDto, PatrolVehicle>(
                NomenclatureEnum.PatrolVehicle,
                "InspectionData/GetPatrolVehicles",
                f => new PatrolVehicle
                {
                    Id = f.Value,
                    Name = f.DisplayName,
                    ExternalMark = f.Code,
                    FlagId = f.FlagId,
                    PatrolVehicleTypeId = f.PatrolVehicleTypeId,
                    CallSign = f.Description,
                    RegistrationNumber = f.RegistrationNumber,
                    VehicleType = f.VehicleType,
                    InstitutionId = f.InstitutionId,
                    NormalizedCallSign = f.Description?.ToLower(),
                    NormalizedExternalMark = f.Code?.ToLower(),
                    NormalizedName = f.DisplayName?.ToLower(),
                },
                f => f.Value
            );
        }

        private static IPullData PullShips(HashSet<int> personsToDelete, HashSet<int> legalsToDelete)
        {
            return new PullData<ShipDto, Ship>(
                NomenclatureEnum.Ship,
                "InspectionData/GetShips",
                f => new Ship
                {
                    Id = f.Id,
                    Uid = f.Uid,
                    AssociationId = f.AssociationId,
                    CFR = f.CFR,
                    ExtMarkings = f.ExtMarkings,
                    Name = f.Name,
                    CallSign = f.CallSign,
                    FlagId = f.FlagId.Value,
                    MMSI = f.MMSI,
                    ShipTypeId = f.ShipTypeId,
                    UVI = f.UVI,
                    FleetTypeId = f.FleetTypeId,
                    NormalizedCFR = f.CFR?.ToLower(),
                    NormalizedExtMarkings = f.ExtMarkings?.ToLower(),
                    NormalizedShipName = f.Name?.ToLower(),
                },
                f => f.Id,
                (context, ships, _, __) =>
                {
                    List<int> cancelledShipIds = ships
                        .Where(f => f.IsCancelled)
                        .Select(f => f.Uid)
                        .Distinct()
                        .ToList();

                    if (cancelledShipIds.Count > 0)
                    {
                        List<ShipOwner> shipOwners = context.ShipOwners
                            .Where(f => cancelledShipIds.Contains(f.ShipUid))
                            .ToList();

                        if (shipOwners.Count > 0)
                        {
                            HandleShipOwnerDeletion(true, context, shipOwners, personsToDelete, legalsToDelete);
                        }

                        List<PermitLicense> permits = context.PermitLicenses
                            .Where(f => cancelledShipIds.Contains(f.ShipUid))
                            .ToList();

                        if (permits.Count > 0)
                        {
                            HandlePermitDeletion(true, context, permits, personsToDelete, legalsToDelete);
                        }

                        List<int> fishingGearIdsToDelete = context.FishingGears
                            .Where(f => cancelledShipIds.Contains(f.ShipUid))
                            .Select(f => f.Id)
                            .ToList();
                        if (fishingGearIdsToDelete.Count > 0)
                        {
                            context.FishingGears.Delete(f => fishingGearIdsToDelete.Contains(f.Id));
                            context.FishingGearMarks.Delete(f => fishingGearIdsToDelete.Contains(f.FishingGearId));
                        }

                        List<int> logBooksIdsToDelete = context.LogBooks
                            .Where(f => cancelledShipIds.Contains(f.ShipUid))
                            .Select(f => f.Id)
                            .ToList();
                        if (logBooksIdsToDelete.Count > 0)
                        {
                            context.LogBooks.Delete(f => logBooksIdsToDelete.Contains(f.Id));
                        }
                    }
                }
            );
        }

        private static IPullData PullPermitLicenses(HashSet<int> personsToPull, HashSet<int> legalsToPull, HashSet<int> personsToDelete, HashSet<int> legalsToDelete)
        {
            return new PullData<PermitLicenseApiDto, PermitLicense>(
                NomenclatureEnum.PermitLicense,
                "InspectionData/GetPermitLicenses",
                f => new PermitLicense
                {
                    Id = f.Id,
                    LegalId = f.LegalId,
                    PersonId = f.PersonId,
                    CaptainId = f.CaptainId,
                    PersonCaptainId = f.PersonCaptainId,
                    LicenseNumber = f.LicenseNumber,
                    PermitNumber = f.PermitNumber,
                    TypeId = f.TypeId,
                    ShipUid = f.ShipUid,
                    ValidFrom = f.ValidFrom,
                    ValidTo = f.ValidTo,
                    IsSuspended = f.IsSuspended,
                },
                f => f.Id,
                (context, permits, _, inactive) =>
                {
                    foreach (PermitLicenseApiDto permit in permits)
                    {
                        personsToPull.Add(permit.PersonCaptainId);

                        if (permit.PersonId.HasValue)
                        {
                            personsToPull.Add(permit.PersonId.Value);
                        }
                        else
                        {
                            legalsToPull.Add(permit.LegalId.Value);
                        }
                    }

                    if (inactive.Count > 0)
                    {
                        List<PermitLicense> permitEntities = context.PermitLicenses
                            .Where(f => inactive.Contains(f.Id))
                            .ToList();

                        if (permitEntities.Count > 0)
                        {
                            HandlePermitDeletion(false, context, permitEntities, personsToDelete, legalsToDelete);
                        }
                    }
                }
            );
        }

        private static IPullData PullShipsOwners(HashSet<int> personsToPull, HashSet<int> legalsToPull, HashSet<int> personsToDelete, HashSet<int> legalsToDelete)
        {
            return new PullData<ShipOwnerApiDto, ShipOwner>(
                NomenclatureEnum.ShipOwner,
                "InspectionData/GetShipsOwners",
                f => new ShipOwner
                {
                    Id = f.Id,
                    LegalId = f.LegalId,
                    PersonId = f.PersonId,
                    ShipUid = f.ShipUid,
                },
                f => f.Id,
                (context, shipOwners, _, inactive) =>
                {
                    foreach (ShipOwnerApiDto owner in shipOwners)
                    {
                        if (owner.PersonId.HasValue)
                        {
                            personsToPull.Add(owner.PersonId.Value);
                        }
                        else
                        {
                            legalsToPull.Add(owner.LegalId.Value);
                        }
                    }

                    if (inactive.Count > 0)
                    {
                        List<ShipOwner> shipOwnerEntities = context.ShipOwners
                            .Where(f => inactive.Contains(f.Id))
                            .ToList();

                        if (shipOwnerEntities.Count > 0)
                        {
                            HandleShipOwnerDeletion(false, context, shipOwnerEntities, personsToDelete, legalsToDelete);
                        }
                    }
                }
            );
        }

        private static IPullData PullShipsFishingGears()
        {
            return new PullData<FishingGearInspectionNomenclatureDto, FishingGear>(
                NomenclatureEnum.ShipFishingGear,
                "InspectionData/GetShipsFishingGears",
                f => new FishingGear
                {
                    Id = f.Id,
                    PermitId = f.PermitId,
                    Count = f.Count,
                    Description = f.Description,
                    Height = f.Height,
                    HookCount = f.HookCount,
                    HouseLength = f.HouseLength,
                    HouseWidth = f.HouseWidth,
                    Length = f.Length,
                    NetEyeSize = f.NetEyeSize,
                    TowelLength = f.TowelLength,
                    CordThickness = f.CordThickness,
                    TypeId = f.TypeId,
                    ShipUid = f.SubjectId,
                },
                f => f.Id,
                (context, _, __, inactive) =>
                {
                    if (inactive.Count > 0)
                    {
                        context.FishingGearMarks.Delete(f => inactive.Contains(f.FishingGearId));
                    }
                }
            );
        }

        private static IPullData PullFishingGearsMarks()
        {
            return new PullData<FishingGearMarkInspectionNomenclatureDto, FishingGearMark>(
                NomenclatureEnum.FishingGearMark,
                "InspectionData/GetFishingGearsMarks",
                f => new FishingGearMark
                {
                    Id = f.Id,
                    FishingGearId = f.FishingGearId,
                    Number = f.Number,
                    Prefix = f.Prefix,
                    CreatedOn = f.CreatedOn,
                    StatusId = f.StatusId,
                },
                f => f.Id
            );
        }

        private static IPullData PullFishingGearsPingers()
        {
            return new PullData<FishingGearPingerInspectionNomenclatureDto, FishingGearPinger>(
                NomenclatureEnum.FishingGearPinger,
                "InspectionData/GetFishingGearsPingers",
                f => new FishingGearPinger
                {
                    Id = f.Id,
                    FishingGearId = f.FishingGearId,
                    Number = f.Number,
                    StatusId = f.StatusId,
                },
                f => f.Id
            );
        }

        private static IPullData PullPermits()
        {
            return new PullData<PermitApiDto, Permit>(
                NomenclatureEnum.Permit,
                "InspectionData/GetPermits",
                f => new Permit
                {
                    Id = f.Id,
                    PermitNumber = f.PermitNumber,
                    ShipUid = f.ShipUid,
                    TypeId = f.TypeId,
                    ValidFrom = f.ValidFrom,
                    ValidTo = f.ValidTo,
                    IsSuspended = f.IsSuspended,
                },
                f => f.Id
            );
        }

        private static IPullData PullPoundNets()
        {
            return new PullData<NomenclatureDto, PoundNet>(
                NomenclatureEnum.PoundNet,
                "InspectionData/GetPoundNets",
                f => new PoundNet
                {
                    Id = f.Value,
                    Name = f.DisplayName,
                    NormalizedName = f.DisplayName?.ToLower(),
                },
                f => f.Value,
                (context, _, __, inactive) =>
                {
                    if (inactive.Count > 0)
                    {
                        List<int> fishingGearIdsToDelete = context.PoundNetFishingGears
                          .Where(f => inactive.Contains(f.PoundNetId))
                          .Select(f => f.Id)
                          .ToList();

                        if (fishingGearIdsToDelete.Count > 0)
                        {
                            context.PoundNetFishingGears.Delete(f => fishingGearIdsToDelete.Contains(f.Id));
                            context.PoundNetFishingGearMarks.Delete(f => fishingGearIdsToDelete.Contains(f.FishingGearId));
                        }
                    }
                }
            );
        }

        private static IPullData PullPoundNetPermitLicenses(HashSet<int> personsToPull, HashSet<int> legalsToPull, HashSet<int> personsToDelete, HashSet<int> legalsToDelete)
        {
            return new PullData<PoundNetPermitLicenseApiDto, PoundNetPermitLicense>(
                NomenclatureEnum.PoundNetPermitLicense,
                "InspectionData/GetPoundNetPermitLicenses",
                f => new PoundNetPermitLicense
                {
                    Id = f.Id,
                    PoundNetId = f.PoundNetId,
                    LegalId = f.LegalId,
                    PersonId = f.PersonId,
                    LicenseNumber = f.LicenseNumber,
                    PermitNumber = f.PermitNumber,
                    TypeId = f.TypeId,
                    ValidFrom = f.ValidFrom,
                    ValidTo = f.ValidTo,
                },
                f => f.Id,
                (context, permits, _, inactive) =>
                {
                    foreach (PoundNetPermitLicenseApiDto permit in permits)
                    {
                        if (permit.PersonId.HasValue)
                        {
                            personsToPull.Add(permit.PersonId.Value);
                        }
                        else
                        {
                            legalsToPull.Add(permit.LegalId.Value);
                        }
                    }

                    if (inactive.Count > 0)
                    {
                        List<PoundNetPermitLicense> permitEntities = context.PoundNetPermitLicenses
                            .Where(f => inactive.Contains(f.Id))
                            .ToList();

                        if (permitEntities.Count > 0)
                        {
                            foreach (PoundNetPermitLicense permit in permitEntities)
                            {
                                if (permit.PersonId.HasValue)
                                {
                                    personsToDelete.Add(permit.PersonId.Value);
                                }
                                else
                                {
                                    legalsToDelete.Add(permit.LegalId.Value);
                                }
                            }

                            List<int> permitIds = permits.ConvertAll(f => f.Id);

                            List<int> fishingGearIdsToDelete = context.FishingGears
                                .Where(f => permitIds.Contains(f.PermitId))
                                .Select(f => f.Id)
                                .ToList();

                            if (fishingGearIdsToDelete.Count > 0)
                            {
                                context.FishingGears.Delete(f => fishingGearIdsToDelete.Contains(f.Id));
                                context.FishingGearMarks.Delete(f => fishingGearIdsToDelete.Contains(f.FishingGearId));
                            }
                        }
                    }
                }
            );
        }

        private static IPullData PullAquacultures(HashSet<int> legalsToPull, HashSet<int> legalsToDelete)
        {
            return new PullData<AquacultureApiDto, Aquaculture>(
                NomenclatureEnum.Aquaculture,
                "InspectionData/GetAquacultures",
                f => new Aquaculture
                {
                    Id = f.Id,
                    Name = f.Name,
                    UrorNum = f.UrorNum,
                    LegalId = f.LegalId,
                    NormalizedName = f.Name?.ToLower(),
                },
                f => f.Id,
                (context, aquacultures, _, inactive) =>
                {
                    foreach (AquacultureApiDto aquaculture in aquacultures)
                    {
                        legalsToPull.Add(aquaculture.LegalId);
                    }

                    if (inactive.Count > 0)
                    {
                        List<Aquaculture> aquacultureEntities = context.Aquacultures
                            .Where(f => inactive.Contains(f.Id))
                            .ToList();

                        foreach (Aquaculture aquaculture in aquacultureEntities)
                        {
                            legalsToDelete.Add(aquaculture.LegalId);
                        }
                    }
                }
            );
        }

        private static IPullData PullPoundNetFishingGears()
        {
            return new PullData<FishingGearInspectionNomenclatureDto, PoundNetFishingGear>(
                NomenclatureEnum.ShipFishingGear,
                "InspectionData/GetPoundNetFishingGears",
                f => new PoundNetFishingGear
                {
                    Id = f.Id,
                    PermitId = f.PermitId,
                    Count = f.Count,
                    Description = f.Description,
                    Height = f.Height,
                    HookCount = f.HookCount,
                    HouseLength = f.HouseLength,
                    HouseWidth = f.HouseWidth,
                    Length = f.Length,
                    NetEyeSize = f.NetEyeSize,
                    TowelLength = f.TowelLength,
                    TypeId = f.TypeId,
                    PoundNetId = f.SubjectId,
                },
                f => f.Id,
                (context, _, __, inactive) =>
                {
                    context.PoundNetFishingGearMarks.Delete(f => inactive.Contains(f.FishingGearId));
                    context.PoundNetFishingGearPingers.Delete(f => inactive.Contains(f.FishingGearId));
                }
            );
        }

        private static IPullData PullPoundNetFishingGearsMarks()
        {
            return new PullData<FishingGearMarkInspectionNomenclatureDto, PoundNetFishingGearMark>(
                NomenclatureEnum.PoundNetFishingGearMark,
                "InspectionData/GetPoundNetFishingGearsMarks",
                f => new PoundNetFishingGearMark
                {
                    Id = f.Id,
                    FishingGearId = f.FishingGearId,
                    Number = f.Number,
                    Prefix = f.Prefix,
                    CreatedOn = f.CreatedOn,
                    StatusId = f.StatusId,
                },
                f => f.Id
            );
        }

        private static IPullData PullPoundNetFishingGearsPingers()
        {
            return new PullData<FishingGearPingerInspectionNomenclatureDto, PoundNetFishingGearPinger>(
                NomenclatureEnum.PoundNetFishingGearPinger,
                "InspectionData/GetPoundNetFishingGearsPingers",
                f => new PoundNetFishingGearPinger
                {
                    Id = f.Id,
                    FishingGearId = f.FishingGearId,
                    Number = f.Number,
                    StatusId = f.StatusId,
                },
                f => f.Id
            );
        }

        private static IPullData PullLogBooks()
        {
            return new PullData<LogBookApiDto, LogBook>(
                NomenclatureEnum.LogBook,
                "InspectionData/GetLogBooks",
                f => new LogBook
                {
                    Id = f.Id,
                    Number = f.Number,
                    IssuedOn = f.IssuedOn,
                    ShipUid = f.ShipUid,
                    EndPage = f.EndPage,
                    StartPage = f.StartPage,
                },
                f => f.Id
            );
        }

        private static IPullData PullBuyers(HashSet<int> personsToPull, HashSet<int> legalsToPull, HashSet<int> personsToDelete, HashSet<int> legalsToDelete)
        {
            return new PullData<BuyerApiDto, Buyer>(
                NomenclatureEnum.Buyer,
                "InspectionData/GetBuyers",
                f =>
                {
                    Buyer buyer = new Buyer
                    {
                        Id = f.Id,
                        LegalId = f.LegalId,
                        PersonId = f.PersonId,
                        HasUtility = f.HasUtility,
                        HasVehicle = f.HasVehicle,
                        UtilityName = f.UtilityName,
                        VehicleNumber = f.VehicleNumber,
                    };

                    if (f.UtilityAddress != null)
                    {
                        buyer.HasAddress = true;
                        buyer.CountryId = f.UtilityAddress.CountryId;
                        buyer.DistrictId = f.UtilityAddress.DistrictId;
                        buyer.MunicipalityId = f.UtilityAddress.MunicipalityId;
                        buyer.PopulatedAreaId = f.UtilityAddress.PopulatedAreaId;
                        buyer.Region = f.UtilityAddress.Region;
                        buyer.PostalCode = f.UtilityAddress.PostalCode;
                        buyer.Street = f.UtilityAddress.Street;
                        buyer.StreetNum = f.UtilityAddress.StreetNum;
                        buyer.BlockNum = f.UtilityAddress.BlockNum;
                        buyer.EntranceNum = f.UtilityAddress.EntranceNum;
                        buyer.FloorNum = f.UtilityAddress.FloorNum;
                        buyer.ApartmentNum = f.UtilityAddress.ApartmentNum;
                    }
                    else
                    {
                        buyer.HasAddress = false;
                    }

                    return buyer;
                },
                f => f.Id,
                (context, buyers, _, inactive) =>
                {
                    foreach (BuyerApiDto buyer in buyers)
                    {
                        if (buyer.PersonId.HasValue)
                        {
                            personsToPull.Add(buyer.PersonId.Value);
                        }
                        else
                        {
                            legalsToPull.Add(buyer.LegalId.Value);
                        }
                    }

                    if (inactive.Count > 0)
                    {
                        List<Buyer> buyerEntities = context.Buyers
                            .Where(f => inactive.Contains(f.Id))
                            .ToList();

                        if (buyerEntities.Count > 0)
                        {
                            foreach (Buyer buyer in buyerEntities)
                            {
                                if (buyer.PersonId.HasValue)
                                {
                                    personsToDelete.Add(buyer.PersonId.Value);
                                }
                                else
                                {
                                    legalsToDelete.Add(buyer.LegalId.Value);
                                }
                            }

                            List<int> shipOwnerIds = buyerEntities.ConvertAll(f => f.Id);
                            context.ShipOwners.Delete(f => shipOwnerIds.Contains(f.Id));
                        }
                    }
                }
            );
        }

        private static void HandleShipOwnerDeletion(bool deleteOwners, IAppDbContext context, List<ShipOwner> shipOwners, HashSet<int> personsToDelete, HashSet<int> legalsToDelete)
        {
            foreach (ShipOwner shipOwner in shipOwners)
            {
                if (shipOwner.PersonId.HasValue)
                {
                    personsToDelete.Add(shipOwner.PersonId.Value);
                }
                else
                {
                    legalsToDelete.Add(shipOwner.LegalId.Value);
                }
            }

            if (deleteOwners)
            {
                List<int> shipOwnerIds = shipOwners.ConvertAll(f => f.Id);
                context.ShipOwners.Delete(f => shipOwnerIds.Contains(f.Id));
            }
        }

        private static void HandlePermitDeletion(bool deletePermits, IAppDbContext context, List<PermitLicense> permits, HashSet<int> personsToDelete, HashSet<int> legalsToDelete)
        {
            foreach (PermitLicense permit in permits)
            {
                personsToDelete.Add(permit.PersonCaptainId);

                if (permit.PersonId.HasValue)
                {
                    personsToDelete.Add(permit.PersonId.Value);
                }
                else
                {
                    legalsToDelete.Add(permit.LegalId.Value);
                }
            }

            List<int> permitIds = permits.ConvertAll(f => f.Id);

            if (deletePermits)
            {
                context.PermitLicenses.Delete(f => permitIds.Contains(f.Id));
            }

            List<int> fishingGearIdsToDelete = context.FishingGears
                .Where(f => permitIds.Contains(f.PermitId))
                .Select(f => f.Id)
                .ToList();

            if (fishingGearIdsToDelete.Count > 0)
            {
                context.FishingGears.Delete(f => fishingGearIdsToDelete.Contains(f.Id));
                context.FishingGearMarks.Delete(f => fishingGearIdsToDelete.Contains(f.FishingGearId));
                context.FishingGearPingers.Delete(f => fishingGearIdsToDelete.Contains(f.FishingGearId));
            }
        }
    }
}
