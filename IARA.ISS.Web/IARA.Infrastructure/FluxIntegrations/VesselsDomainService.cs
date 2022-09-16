using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.ShipsRegister;
using IARA.EntityModels.Entities;
using IARA.Flux.Models;
using IARA.FluxModels;
using IARA.FluxModels.Enums;
using IARA.Infrastructure.FluxIntegrations.Interfaces;
using IARA.Interfaces;
using IARA.Interfaces.Flux;
using IARA.Interfaces.FluxIntegrations.Ships;
using IARA.Interfaces.Legals;

namespace IARA.Infrastructure.FluxIntegrations
{
    public class VesselsDomainService : BaseService, IVesselsDomainService
    {
        private readonly IFluxVesselDomainReceiverService vesselDomainReceiverService;
        private readonly IVesselToFluxVesselReportMapper vesselToFluxVesselReportMapper;
        private readonly IPersonService personService;
        private readonly ILegalService legalService;

        public VesselsDomainService(IARADbContext dbContext,
                                    IFluxVesselDomainReceiverService vesselDomainReceiverService,
                                    IVesselToFluxVesselReportMapper vesselToFluxVesselReportMapper,
                                    IPersonService personService,
                                    ILegalService legalService)
            : base(dbContext)
        {
            this.vesselDomainReceiverService = vesselDomainReceiverService;
            this.vesselToFluxVesselReportMapper = vesselToFluxVesselReportMapper;
            this.personService = personService;
            this.legalService = legalService;
        }

        public Task<bool> ReportVesselChange(FLUXReportVesselInformationType vessel)
        {
            return vesselDomainReceiverService.ReportVesselChange(vessel);
        }

        public FLUXReportVesselInformationType FindVessel(FLUXVesselQueryMessageType queryMessage)
        {
            VesselQueryType query = queryMessage.VesselQuery;
            VesselQueryParams parameters = ParseVesselQueryParams(query);
            List<ShipRegisterEditDTO> ships = GetShipsByQueryParameters(parameters);

            FLUXReportVesselInformationType result = parameters.OnlyVcd
                ? vesselToFluxVesselReportMapper.MapVesselToFluxSubVcd(ships, ReportPurposeCodes.Original)
                : vesselToFluxVesselReportMapper.MapVesselToFluxSub(ships, ReportPurposeCodes.Original);

            // презаписваме типа на доклада
            result.FLUXReportDocument.TypeCode = parameters.QueryType switch
            {
                VesselQueryTypes.Q_SNAP_F => CodeType.CreateVesselReportType(VesselReportTypes.SNAP_F),
                VesselQueryTypes.Q_SNAP_L => CodeType.CreateVesselReportType(VesselReportTypes.SNAP_L),
                _ => throw new NotImplementedException(),
            };

            // референтният номер на заявката се вписва като референтен за доклада
            result.FLUXReportDocument.ReferencedID = IDType.CreateID(query.ID.schemeID, query.ID.Value);

            return result;
        }

        public void ProcessVessel(FLUXReportVesselInformationType vessel)
        {
            throw new NotImplementedException();
        }

        private static VesselQueryParams ParseVesselQueryParams(VesselQueryType query)
        {
            VesselQueryParams result = new VesselQueryParams
            {
                QueryType = query.TypeCode.Value,
                StartDate = (DateTime)query.SpecifiedDelimitedPeriod.StartDateTime,
                EndDate = (DateTime)query.SpecifiedDelimitedPeriod.EndDateTime
            };

            foreach (VesselQueryParameterType param in query.SimpleVesselQueryParameter)
            {
                switch (param.SearchTypeCode.Value)
                {
                    case VesselQueryParamTypes.DATA_VCD:
                        result.OnlyVcd = true;
                        break;
                    case VesselQueryParamTypes.DATA_ALL:
                        result.OnlyVcd = false;
                        break;
                    case VesselQueryParamTypes.HIST_YES:
                        result.HistData = true;
                        break;
                    case VesselQueryParamTypes.HIST_NO:
                        result.HistData = false;
                        break;
                    case VesselQueryParamTypes.VESSEL_ACTIVE:
                        result.OnlyActive = true;
                        break;
                    case VesselQueryParamTypes.VESSEL_ALL:
                        result.OnlyActive = false;
                        break;
                }
            }

            return result;
        }

        private List<ShipRegisterEditDTO> GetShipsByQueryParameters(VesselQueryParams parameters)
        {
            DateTime now = DateTime.Now;

            IQueryable<ShipRegister> query = from ship in Db.ShipsRegister
                                             where ship.RecordType == nameof(RecordTypesEnum.Register)
                                             select ship;

            // филтрираме събитията по начална и крайна дати
            query = from ship in query
                    where ship.EventDate >= parameters.StartDate
                        && ship.EventDate <= parameters.EndDate
                    select ship;

            // филтрираме само текущи събития
            if (!parameters.HistData)
            {
                query = from ship in query
                        where ship.ValidFrom <= now
                            && ship.ValidTo > now
                        select ship;
            }

            // филтрираме само активни кораби
            if (parameters.OnlyActive)
            {
                List<string> inactiveStatuses = new List<string>
                {
                    nameof(ShipEventTypeEnum.EXP),
                    nameof(ShipEventTypeEnum.RET),
                    nameof(ShipEventTypeEnum.DES)
                };

                List<int> inactiveShips = (from ship in Db.ShipsRegister
                                           join ev in Db.NeventTypes on ship.EventTypeId equals ev.Id
                                           where ship.ValidFrom <= now
                                                && ship.ValidTo > now
                                                && inactiveStatuses.Contains(ev.Code)
                                           select ship.ShipUid).ToList();

                query = from ship in query
                        where !inactiveShips.Contains(ship.ShipUid)
                        select ship;
            }

            List<int> shipIds = query.Select(x => x.Id).ToList();

            List<ShipRegisterEditDTO> result = GetShipsForFlux(shipIds);
            return result;
        }

        private List<ShipRegisterEditDTO> GetShipsForFlux(List<int> ids)
        {
            List<ShipRegister> dbShips = (from ship in Db.ShipsRegister
                                          where ids.Contains(ship.Id)
                                          orderby ship.EventDate
                                          select ship).ToList();

            List<ShipRegisterEditDTO> result = MapDbShipsToDTOForFlux(dbShips);
            return result;
        }

        private List<ShipRegisterEditDTO> MapDbShipsToDTOForFlux(List<ShipRegister> dbShips)
        {
            DateTime now = DateTime.Now;

            Dictionary<int, ShipEventTypeEnum> eventTypes = (from ev in Db.NeventTypes
                                                             where ev.ValidFrom <= now && ev.ValidTo > now
                                                             select new
                                                             {
                                                                 ev.Id,
                                                                 Code = Enum.Parse<ShipEventTypeEnum>(ev.Code)
                                                             }).ToDictionary(x => x.Id, y => y.Code);

            List<ShipRegisterEditDTO> result = (from dbShip in dbShips
                                                select new ShipRegisterEditDTO
                                                {
                                                    Id = dbShip.Id,
                                                    ShipUID = dbShip.ShipUid,
                                                    ApplicationId = dbShip.ApplicationId,
                                                    EventType = eventTypes[dbShip.EventTypeId],
                                                    EventDate = dbShip.EventDate,
                                                    CFR = dbShip.Cfr,
                                                    Name = dbShip.Name,
                                                    ExternalMark = dbShip.ExternalMark,
                                                    RegistrationNumber = dbShip.RegistrationNum,
                                                    RegistrationDate = dbShip.RegistrationDate,
                                                    FleetSegmentId = dbShip.FleetSegmentId,
                                                    IRCSCallSign = dbShip.IrcscallSign,
                                                    MMSI = dbShip.Mmsi,
                                                    UVI = dbShip.Uvi,
                                                    CountryFlagId = dbShip.FlagCountryId,
                                                    HasAIS = dbShip.HasAis,
                                                    HasERS = dbShip.HasErs,
                                                    HasVMS = dbShip.HasVms,
                                                    VesselTypeId = dbShip.VesselTypeId,
                                                    ExploitationStartDate = dbShip.ExploitationStartDate,
                                                    BuildYear = dbShip.BuildYear,
                                                    PublicAidTypeId = dbShip.PublicAidTypeId,
                                                    PortId = dbShip.PortId,
                                                    TotalLength = dbShip.TotalLength,
                                                    TotalWidth = dbShip.TotalWidth,
                                                    GrossTonnage = dbShip.GrossTonnage,
                                                    NetTonnage = dbShip.NetTonnage,
                                                    OtherTonnage = dbShip.OtherTonnage,
                                                    ShipDraught = dbShip.ShipDraught,
                                                    LengthBetweenPerpendiculars = dbShip.LengthBetweenPerpendiculars,
                                                    MainEnginePower = dbShip.MainEnginePower,
                                                    AuxiliaryEnginePower = dbShip.AuxiliaryEnginePower,
                                                    MainEngineModel = dbShip.MainEngineModel,
                                                    MainFishingGearId = dbShip.MainFishingGearId,
                                                    AdditionalFishingGearId = dbShip.AdditionalFishingGearId,
                                                    HullMaterialId = dbShip.HullMaterialId,
                                                    CrewCount = dbShip.CrewCount,
                                                    HasControlCard = dbShip.HasControlCard,
                                                    ImportCountryId = dbShip.ImportCountryId,
                                                    ExportCountryId = dbShip.ExportCountryId,
                                                    ExportType = dbShip.ExportType == null ? default(ShipExportTypeEnum?) : Enum.Parse<ShipExportTypeEnum>(dbShip.ExportType)
                                                }).ToList();

            List<int> shipIds = result.Select(x => x.Id.Value).ToList();
            Dictionary<int, List<ShipOwnerDTO>> owners = GetOwnersForFlux(shipIds);

            foreach (ShipRegisterEditDTO ship in result)
            {
                ship.Owners = owners[ship.Id.Value].ToList();
            }

            return result;
        }

        private Dictionary<int, List<ShipOwnerDTO>> GetOwnersForFlux(List<int> shipIds)
        {
            var data = (from shipOwner in Db.ShipOwners
                        where shipIds.Contains(shipOwner.ShipRegisterId)
                            && shipOwner.IsActive
                        select new
                        {
                            shipOwner.ShipRegisterId,
                            Owner = new
                            {
                                shipOwner.Id,
                                IsOwnerPerson = shipOwner.OwnerIsPerson,
                                PersonId = shipOwner.OwnerPersonId,
                                LegalId = shipOwner.OwnerLegalId,
                                shipOwner.IsShipHolder
                            }
                        }).ToLookup(x => x.ShipRegisterId, y => y.Owner);

            List<int> personIds = data.SelectMany(x => x).Where(x => x.IsOwnerPerson).Select(x => x.PersonId.Value).ToList();
            List<int> legalIds = data.SelectMany(x => x).Where(x => !x.IsOwnerPerson).Select(x => x.LegalId.Value).ToList();

            Dictionary<int, RegixPersonDataDTO> persons = personService.GetRegixPersonsData(personIds);
            Dictionary<int, RegixLegalDataDTO> legals = legalService.GetRegixLegalsData(legalIds);
            ILookup<int, AddressRegistrationDTO> personAddresses = personService.GetAddressRegistrations(personIds);
            ILookup<int, AddressRegistrationDTO> legalAddresses = legalService.GetAddressRegistrations(personIds);

            Dictionary<int, List<ShipOwnerDTO>> result = new Dictionary<int, List<ShipOwnerDTO>>();

            foreach (var shipOwners in data)
            {
                List<ShipOwnerDTO> owners = new List<ShipOwnerDTO>();

                foreach (var shipOwner in shipOwners)
                {
                    ShipOwnerDTO owner = new ShipOwnerDTO
                    {
                        Id = shipOwner.Id,
                        IsOwnerPerson = shipOwner.IsOwnerPerson,
                        IsShipHolder = shipOwner.IsShipHolder,
                        IsActive = true
                    };

                    if (shipOwner.IsOwnerPerson)
                    {
                        owner.RegixPersonData = persons[shipOwner.PersonId.Value];
                        owner.AddressRegistrations = personAddresses[shipOwner.PersonId.Value].ToList();
                    }
                    else
                    {
                        owner.RegixLegalData = legals[shipOwner.LegalId.Value];
                        owner.AddressRegistrations = legalAddresses[shipOwner.LegalId.Value].ToList();
                    }

                    owners.Add(owner);
                }

                result.Add(shipOwners.Key, owners);
            }

            return result;
        }
    }

    internal class VesselQueryParams
    {
        public string QueryType { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; } = new DateTime(2100, 12, 31);

        public bool HistData { get; set; } = true;

        public bool OnlyActive { get; set; } = false;

        public bool OnlyVcd { get; set; } = true;
    }
}
