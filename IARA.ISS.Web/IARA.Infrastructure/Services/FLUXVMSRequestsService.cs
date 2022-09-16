using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Transactions;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.FLUXVMSRequests;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Flux.Models;
using IARA.FluxModels.Enums;
using IARA.Infrastructure.FluxIntegrations.Interfaces;
using IARA.Interfaces;
using IARA.Interfaces.FluxIntegrations.PermitsAndCertificates;

namespace IARA.Infrastructure.Services
{
    public class FLUXVMSRequestsService : Service, IFLUXVMSRequestsService
    {
        private readonly IFluxPermitsDomainService fluxPermitsDomainService;
        private readonly IFlapDomainMapper flapDomainMapper;

        public FLUXVMSRequestsService(IARADbContext db,
                                      IFluxPermitsDomainService fluxPermitsDomainService,
                                      IFlapDomainMapper flapDomainMapper)
            : base(db)
        {
            this.fluxPermitsDomainService = fluxPermitsDomainService;
            this.flapDomainMapper = flapDomainMapper;
        }

        public IQueryable<FLUXVMSRequestDTO> GetAll(FLUXVMSRequestFilters filters)
        {
            IQueryable<FLUXVMSRequestDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                bool showInactive = filters?.ShowInactiveRecords ?? false;

                result = GetAllRequests(showInactive);
            }
            else
            {
                result = filters.HasAnyFilters()
                    ? GetParametersFilteredRequests(filters)
                    : GetFreeTextFilteredRequests(filters.FreeTextSearch, filters.ShowInactiveRecords);
            }

            return result;
        }

        public FLUXVMSRequestEditDTO Get(int id)
        {
            FLUXVMSRequestEditDTO result = (from request in Db.Fluxfvmsrequests
                                            where request.Id == id
                                            select new FLUXVMSRequestEditDTO
                                            {
                                                IsOutgoing = request.IsOutgoing,
                                                DomainName = request.DomainName,
                                                WebServiceName = request.WebServiceName,
                                                RequestDateTime = request.RequestDateTime,
                                                RequestUUID = request.RequestUuid.ToString(),
                                                RequestContent = request.RequestContent,
                                                ResponseStatus = request.ResponseStatus,
                                                ResponseDateTime = request.ResponseDateTime,
                                                ResponseUUID = request.ResponseUuid.HasValue ? request.ResponseUuid.ToString() : null,
                                                ErrorDescription = request.ErrorDescription,
                                                ResponseContent = request.ResponseContent
                                            }).First();

            return result;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            SimpleAuditDTO audit = GetSimpleEntityAuditValues(Db.Fluxfvmsrequests, id);
            return audit;
        }

        public IQueryable<FluxFlapRequestDTO> GetAllFlapRequests(FluxFlapRequestFilters filters)
        {
            IQueryable<FluxFlapRequestDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                result = GetAllFlapRequests();
            }
            else
            {
                result = filters.HasAnyFilters()
                    ? GetParametersFilteredFlapRequests(filters)
                    : GetFreeTextFilteredFlapRequests(filters.FreeTextSearch);
            }

            return result;
        }

        public FluxFlapRequestEditDTO GetFlapRequest(int id)
        {
            var data = (from req in Db.Fluxflaprequests
                        join flux in Db.Fluxfvmsrequests on req.FluxfvmsrequestId equals flux.Id
                        where req.Id == id
                        select new
                        {
                            req.Id,
                            flux.IsOutgoing,
                            req.RequestContent,
                            req.ResponseContent,
                            Ship = new FluxFlapRequestShipDTO
                            {
                                ShipId = req.ShipId,
                                ShipIdentifierType = req.ShipIdentifierType,
                                ShipIdentifier = req.ShipIdentifier,
                                ShipName = req.ShipName
                            }
                        }).First();

            FluxFlapRequestEditDTO request = JsonSerializer.Deserialize<FluxFlapRequestEditDTO>(data.RequestContent);
            request.Id = data.Id;
            request.IsOutgoing = data.IsOutgoing;
            request.Ship = data.Ship;

            return request;
        }

        public void AddFlapRequest(FluxFlapRequestEditDTO request)
        {
            FLUXFLAPRequestMessageType flap = flapDomainMapper.MapRequestToFlux(request, Guid.NewGuid(), ReportPurposeCodes.Original);
            fluxPermitsDomainService.SendFlapRequest(new(flap, request));
        }

        public SimpleAuditDTO GetFlapRequestAudit(int id)
        {
            SimpleAuditDTO audit = GetSimpleEntityAuditValues(Db.Fluxflaprequests, id);
            return audit;
        }

        public List<NomenclatureDTO> GetAgreementTypes()
        {
            DateTime now = DateTime.Now;

            List<NomenclatureDTO> result = (from type in Db.MdrAgreementTypes
                                            orderby type.Code, type.EnDescription
                                            select new NomenclatureDTO
                                            {
                                                Value = type.Id,
                                                Code = type.Code,
                                                DisplayName = type.EnDescription,
                                                IsActive = type.ValidFrom <= now && type.ValidTo > now
                                            }).ToList();
            return result;
        }

        public List<NomenclatureDTO> GetCoastalParties()
        {
            DateTime now = DateTime.Now;

            List<NomenclatureDTO> result = (from party in Db.MdrFlapCoastalParties
                                            orderby party.Code, party.EnDescription
                                            select new NomenclatureDTO
                                            {
                                                Value = party.Id,
                                                Code = party.Code,
                                                DisplayName = party.EnDescription,
                                                IsActive = party.ValidFrom <= now && party.ValidTo > now
                                            }).ToList();
            return result;
        }

        public List<NomenclatureDTO> GetRequestPurposes()
        {
            DateTime now = DateTime.Now;

            List<NomenclatureDTO> result = (from purpose in Db.MdrFlapRequestPurposes
                                            orderby purpose.Code, purpose.EnDescription
                                            select new NomenclatureDTO
                                            {
                                                Value = purpose.Id,
                                                Code = purpose.Code,
                                                DisplayName = purpose.EnDescription,
                                                IsActive = purpose.ValidFrom <= now && purpose.ValidTo > now
                                            }).ToList();
            return result;
        }

        public List<NomenclatureDTO> GetFishingCategories()
        {
            DateTime now = DateTime.Now;

            List<NomenclatureDTO> result = (from category in Db.MdrCrFishingCategories
                                            orderby category.Code, category.EnDescription
                                            select new NomenclatureDTO
                                            {
                                                Value = category.Id,
                                                Code = category.Code,
                                                DisplayName = category.EnDescription,
                                                IsActive = category.ValidFrom <= now && category.ValidTo > now
                                            }).ToList();
            return result;
        }

        public List<NomenclatureDTO> GetFlapQuotaTypes()
        {
            DateTime now = DateTime.Now;

            List<NomenclatureDTO> result = (from category in Db.MdrFlapQuotaTypes
                                            orderby category.Code, category.EnDescription
                                            select new NomenclatureDTO
                                            {
                                                Value = category.Id,
                                                Code = category.Code,
                                                DisplayName = category.EnDescription,
                                                IsActive = category.ValidFrom <= now && category.ValidTo > now
                                            }).ToList();
            return result;
        }

        private IQueryable<FLUXVMSRequestDTO> GetAllRequests(bool showInactive)
        {
            var result = from request in Db.Fluxfvmsrequests
                         where request.IsActive == !showInactive
                         orderby request.RequestDateTime descending
                         select new FLUXVMSRequestDTO
                         {
                             Id = request.Id,
                             IsOutgoing = request.IsOutgoing,
                             WebServiceName = request.DomainName + "/" + request.WebServiceName,
                             RequestDateTime = request.RequestDateTime,
                             RequestUUID = request.RequestUuid.ToString(),
                             ResponseStatus = request.ResponseStatus,
                             ResponseDateTime = request.ResponseDateTime,
                             ResponseUUID = request.ResponseUuid.HasValue ? request.ResponseUuid.ToString() : null,
                             ErrorDescription = request.ErrorDescription,
                             IsActive = request.IsActive
                         };

            return result;
        }

        private IQueryable<FLUXVMSRequestDTO> GetFreeTextFilteredRequests(string freeTextSearch, bool showInactive)
        {
            freeTextSearch = freeTextSearch.ToLowerInvariant();
            DateTime? searchDate = DateTimeUtils.TryParseDate(freeTextSearch);

            var result = from request in Db.Fluxfvmsrequests
                         where request.IsActive == !showInactive
                         && ((request.DomainName + "/" + request.WebServiceName).ToLower().Contains(freeTextSearch)
                              || (!string.IsNullOrEmpty(request.ResponseStatus) && request.ResponseStatus.ToLower().Contains(freeTextSearch))
                              || request.RequestUuid.ToString().ToLower().Contains(freeTextSearch)
                              || (request.ResponseUuid.HasValue && request.ResponseUuid.ToString().ToLower().Contains(freeTextSearch))
                              || (searchDate.HasValue && request.RequestDateTime.Date == searchDate)
                              || (request.ResponseDateTime.HasValue && searchDate.HasValue && request.ResponseDateTime.Value.Date == searchDate)
                              || request.ErrorDescription.ToLower().Contains(freeTextSearch))
                         orderby request.RequestDateTime descending
                         select new FLUXVMSRequestDTO
                         {
                             Id = request.Id,
                             IsOutgoing = request.IsOutgoing,
                             WebServiceName = request.DomainName + "/" + request.WebServiceName,
                             RequestDateTime = request.RequestDateTime,
                             RequestUUID = request.RequestUuid.ToString(),
                             ResponseStatus = request.ResponseStatus,
                             ResponseDateTime = request.ResponseDateTime,
                             ResponseUUID = request.ResponseUuid.HasValue ? request.ResponseUuid.ToString() : null,
                             ErrorDescription = request.ErrorDescription,
                             IsActive = request.IsActive
                         };

            return result;
        }

        private IQueryable<FLUXVMSRequestDTO> GetParametersFilteredRequests(FLUXVMSRequestFilters filters)
        {
            var query = from request in Db.Fluxfvmsrequests
                        where request.IsActive == !filters.ShowInactiveRecords
                        select new
                        {
                            request.Id,
                            request.IsOutgoing,
                            request.DomainName,
                            WebServiceName = request.DomainName + "/" + request.WebServiceName,
                            request.RequestDateTime,
                            RequestUuid = request.RequestUuid.ToString(),
                            ResponseUuid = request.ResponseUuid.HasValue ? request.ResponseUuid.ToString() : null,
                            request.ResponseStatus,
                            request.ResponseDateTime,
                            request.ErrorDescription,
                            request.IsActive
                        };

            if (!string.IsNullOrEmpty(filters.WebServiceName))
            {
                query = query.Where(x => x.WebServiceName.ToLower().Contains(filters.WebServiceName.ToLower()));
            }

            if (filters.RequestDateFrom.HasValue)
            {
                query = query.Where(x => x.RequestDateTime.Date >= filters.RequestDateFrom);
            }

            if (filters.RequestDateTo.HasValue)
            {
                query = query.Where(x => x.RequestDateTime.Date <= filters.RequestDateTo);
            }

            if (filters.ResponseDateFrom.HasValue)
            {
                query = query.Where(x => x.ResponseDateTime.HasValue && x.ResponseDateTime.Value.Date >= filters.ResponseDateFrom);
            }

            if (filters.ResponseDateTo.HasValue)
            {
                query = query.Where(x => x.ResponseDateTime.HasValue && x.ResponseDateTime.Value.Date <= filters.ResponseDateTo);
            }

            if (!string.IsNullOrEmpty(filters.RequestUUID))
            {
                query = query.Where(x => x.RequestUuid.ToLower().Contains(filters.RequestUUID.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.ResponseUUID))
            {
                query = query.Where(x => !string.IsNullOrEmpty(x.ResponseUuid)
                                      && x.ResponseUuid.ToLower().Contains(filters.ResponseUUID.ToLower()));
            }

            if (filters.ResponseStatuses != null && filters.ResponseStatuses.Count > 0)
            {
                query = query.Where(x => filters.ResponseStatuses.Contains(x.ResponseStatus));
            }

            if (filters.DomainNames != null && filters.DomainNames.Count > 0)
            {
                query = query.Where(x => filters.DomainNames.Contains(x.DomainName));
            }

            IQueryable<FLUXVMSRequestDTO> result = from request in query
                                                   orderby request.RequestDateTime descending
                                                   select new FLUXVMSRequestDTO
                                                   {
                                                       Id = request.Id,
                                                       IsOutgoing = request.IsOutgoing,
                                                       WebServiceName =  request.WebServiceName,
                                                       RequestDateTime = request.RequestDateTime,
                                                       RequestUUID = request.RequestUuid,
                                                       ResponseStatus = request.ResponseStatus,
                                                       ResponseDateTime = request.ResponseDateTime,
                                                       ResponseUUID = request.ResponseUuid,
                                                       ErrorDescription = request.ErrorDescription,
                                                       IsActive = request.IsActive
                                                   };

            return result;
        }

        private IQueryable<FluxFlapRequestDTO> GetAllFlapRequests()
        {
            DateTime now = DateTime.Now;

            IQueryable<FluxFlapRequestDTO> query = from flap in Db.Fluxflaprequests
                                                   join flux in Db.Fluxfvmsrequests on flap.FluxfvmsrequestId equals flux.Id
                                                   where flap.ValidFrom <= now
                                                        && flap.ValidTo > now
                                                   orderby flap.ValidFrom descending
                                                   select new FluxFlapRequestDTO
                                                   {
                                                       Id = flap.Id,
                                                       IsOutgoing = flux.IsOutgoing,
                                                       Ship = flap.ShipName + " | " + flap.ShipIdentifierType + ": " + flap.ShipIdentifier,
                                                       RequestUuid = flux.RequestUuid.ToString(),
                                                       RequestDate = flux.RequestDateTime,
                                                       ResponseUuid = flux.ResponseUuid != null ? flux.ResponseUuid.ToString() : null,
                                                       ResponseDate = flux.ResponseDateTime
                                                   };

            return query;
        }

        private IQueryable<FluxFlapRequestDTO> GetFreeTextFilteredFlapRequests(string text)
        {
            DateTime now = DateTime.Now;
            text = text.ToLower();
            DateTime? searchDate = DateTimeUtils.TryParseDate(text);

            IQueryable<FluxFlapRequestDTO> query = from flap in Db.Fluxflaprequests
                                                   join flux in Db.Fluxfvmsrequests on flap.FluxfvmsrequestId equals flux.Id
                                                   where flap.ValidFrom <= now
                                                        && flap.ValidTo > now
                                                        && ((flap.ShipName + " | " + flap.ShipIdentifierType + ": " + flap.ShipIdentifier).ToLower().Contains(text)
                                                            || flux.RequestUuid.ToString().ToLower().Contains(text)
                                                            || (searchDate.HasValue && searchDate.Value == flux.RequestDateTime.Date)
                                                            || (!flux.ResponseUuid.HasValue && flux.ResponseUuid.ToString().ToLower().Contains(text))
                                                            || (searchDate.HasValue && flux.ResponseDateTime.HasValue && searchDate.Value == flux.ResponseDateTime.Value.Date))
                                                   orderby flap.ValidFrom descending
                                                   select new FluxFlapRequestDTO
                                                   {
                                                       Id = flap.Id,
                                                       IsOutgoing = flux.IsOutgoing,
                                                       Ship = flap.ShipName + " | " + flap.ShipIdentifierType + ": " + flap.ShipIdentifier,
                                                       RequestUuid = flux.RequestUuid.ToString(),
                                                       RequestDate = flux.RequestDateTime,
                                                       ResponseUuid = flux.ResponseUuid != null ? flux.ResponseUuid.ToString() : null,
                                                       ResponseDate = flux.ResponseDateTime
                                                   };

            return query;
        }

        private IQueryable<FluxFlapRequestDTO> GetParametersFilteredFlapRequests(FluxFlapRequestFilters filters)
        {
            DateTime now = DateTime.Now;

            var query = from flap in Db.Fluxflaprequests
                        join flux in Db.Fluxfvmsrequests on flap.FluxfvmsrequestId equals flux.Id
                        where flap.ValidFrom <= now
                             && flap.ValidTo > now
                        select new
                        {
                            flap.Id,
                            flux.IsOutgoing,
                            flap.ShipId,
                            flap.ShipIdentifierType,
                            flap.ShipIdentifier,
                            flap.ShipName,
                            flap.ValidFrom,
                            RequestUuid = flux.RequestUuid.ToString(),
                            RequestDate = flux.RequestDateTime,
                            ResponseUuid = flux.ResponseUuid != null ? flux.ResponseUuid.ToString() : null,
                            ResponseDate = flux.ResponseDateTime
                        };

            if (filters.ShipId.HasValue)
            {
                query = query.Where(x => x.ShipId.HasValue && x.ShipId.Value == filters.ShipId.Value);
            }

            if (!string.IsNullOrEmpty(filters.ShipIdentifier))
            {
                query = query.Where(x => x.ShipIdentifier.ToLower().Contains(filters.ShipIdentifier.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.ShipName))
            {
                query = query.Where(x => x.ShipName.ToLower().Contains(filters.ShipName.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.RequestUuid))
            {
                query = query.Where(x => x.RequestUuid.ToLower().Contains(filters.RequestUuid.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.ResponseUuid))
            {
                query = query.Where(x => x.ResponseUuid.ToLower().Contains(filters.ResponseUuid.ToLower()));
            }

            if (filters.RequestDateFrom.HasValue)
            {
                query = query.Where(x => x.RequestDate >= filters.RequestDateFrom.Value);
            }

            if (filters.RequestDateTo.HasValue)
            {
                query = query.Where(x => x.RequestDate <= filters.RequestDateTo.Value);
            }

            IQueryable<FluxFlapRequestDTO> result = from flap in query
                                                    orderby flap.ValidFrom descending
                                                    select new FluxFlapRequestDTO
                                                    {
                                                        Id = flap.Id,
                                                        IsOutgoing = flap.IsOutgoing,
                                                        Ship = flap.ShipName + " | " + flap.ShipIdentifierType + ": " + flap.ShipIdentifier,
                                                        RequestUuid = flap.RequestUuid,
                                                        RequestDate = flap.RequestDate,
                                                        ResponseUuid = flap.ResponseUuid,
                                                        ResponseDate = flap.ResponseDate
                                                    };

            return result;
        }
    }
}
