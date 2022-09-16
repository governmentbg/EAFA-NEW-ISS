using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Constants;
using IARA.Common.Utils;
using IARA.DomainModels.DTOModels.FishingCapacity;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Interfaces;

namespace IARA.Infrastructure.Services
{
    public partial class FishingCapacityService : Service, IFishingCapacityService
    {
        public IQueryable<MaximumFishingCapacityDTO> GetAllMaximumCapacities(MaximumFishingCapacityFilters filters)
        {
            IQueryable<MaximumFishingCapacityDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                result = GetAllMaximumCapacities();
            }
            else
            {
                result = filters.HasAnyFilters()
                    ? GetParametersFilteredMaximumCapacities(filters)
                    : GetFreeTextFilteredMaximumCapacities(filters.FreeTextSearch);
            }
            return result;
        }

        public MaximumFishingCapacityEditDTO GetMaximumCapacity(int id)
        {
            MaximumFishingCapacityEditDTO result = (from cap in Db.CountryCapacityRegister
                                                    where cap.Id == id
                                                    select new MaximumFishingCapacityEditDTO
                                                    {
                                                        Id = cap.Id,
                                                        Date = cap.RegulationDate,
                                                        Regulation = cap.Regulation,
                                                        GrossTonnage = cap.GrossTonnage,
                                                        Power = cap.EnginePower
                                                    }).First();

            return result;
        }

        public LatestMaximumCapacityDTO GetLatestMaximumCapacities()
        {
            List<CountryCapacityRegister> latest = Db.CountryCapacityRegister.OrderByDescending(x => x.ValidTo).Take(2).ToList();

            if (latest.Count == 0)
            {
                return null;
            }

            LatestMaximumCapacityDTO result = new LatestMaximumCapacityDTO
            {
                Id = latest[0].Id,
                Date = latest[0].RegulationDate,
                PrevDate = latest.Count > 1 ? latest[1].RegulationDate : default(DateTime?)
            };
            return result;
        }

        public int AddMaximumCapacity(MaximumFishingCapacityEditDTO capacity)
        {
            CountryCapacityRegister latest = Db.CountryCapacityRegister.OrderByDescending(x => x.ValidTo).FirstOrDefault();

            if (latest != null)
            {
                latest.ValidTo = capacity.Date.Value;
            }

            CountryCapacityRegister entry = new CountryCapacityRegister
            {
                RegulationDate = capacity.Date.Value,
                Regulation = capacity.Regulation,
                GrossTonnage = capacity.GrossTonnage.Value,
                EnginePower = capacity.Power.Value,
                ValidFrom = capacity.Date.Value,
                ValidTo = DefaultConstants.MAX_VALID_DATE
            };

            Db.CountryCapacityRegister.Add(entry);
            Db.SaveChanges();

            return entry.Id;
        }

        public void EditMaximumCapacity(MaximumFishingCapacityEditDTO capacity)
        {
            CountryCapacityRegister entry = (from cap in Db.CountryCapacityRegister
                                             where cap.Id == capacity.Id
                                             select cap).First();

            entry.RegulationDate = capacity.Date.Value;
            entry.Regulation = capacity.Regulation;
            entry.GrossTonnage = capacity.GrossTonnage.Value;
            entry.EnginePower = capacity.Power.Value;

            Db.SaveChanges();
        }

        public SimpleAuditDTO GetMaximumCapacitySimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.CountryCapacityRegister, id);
        }

        private IQueryable<MaximumFishingCapacityDTO> GetAllMaximumCapacities()
        {
            IQueryable<MaximumFishingCapacityDTO> result = from cap in Db.CountryCapacityRegister
                                                           orderby cap.ValidTo descending
                                                           select new MaximumFishingCapacityDTO
                                                           {
                                                               Id = cap.Id,
                                                               Date = cap.RegulationDate,
                                                               Regulation = cap.Regulation,
                                                               GrossTonnage = cap.GrossTonnage,
                                                               Power = cap.EnginePower,
                                                               IsActive = true
                                                           };
            return result;
        }

        private IQueryable<MaximumFishingCapacityDTO> GetParametersFilteredMaximumCapacities(MaximumFishingCapacityFilters filters)
        {
            IQueryable<MaximumFishingCapacityDTO> result = from cap in Db.CountryCapacityRegister
                                                           orderby cap.ValidTo descending
                                                           select new MaximumFishingCapacityDTO
                                                           {
                                                               Id = cap.Id,
                                                               Date = cap.RegulationDate,
                                                               Regulation = cap.Regulation,
                                                               GrossTonnage = cap.GrossTonnage,
                                                               Power = cap.EnginePower,
                                                               IsActive = true
                                                           };

            if (filters.DateFrom.HasValue)
            {
                result = result.Where(x => x.Date >= filters.DateFrom.Value);
            }

            if (filters.DateTo.HasValue)
            {
                result = result.Where(x => x.Date <= filters.DateTo.Value);
            }

            if (!string.IsNullOrEmpty(filters.Regulation))
            {
                result = result.Where(x => x.Regulation.ToLower().Contains(filters.Regulation.ToLower()));
            }

            return result;
        }

        private IQueryable<MaximumFishingCapacityDTO> GetFreeTextFilteredMaximumCapacities(string text)
        {
            text = text.ToLowerInvariant();
            DateTime? searchDate = DateTimeUtils.TryParseDate(text);

            IQueryable<MaximumFishingCapacityDTO> result = from cap in Db.CountryCapacityRegister
                                                           where cap.Regulation.ToLower().Contains(text)
                                                                || cap.GrossTonnage.ToString().Contains(text)
                                                                || cap.EnginePower.ToString().Contains(text)
                                                                || (searchDate.HasValue && cap.RegulationDate == searchDate.Value)
                                                           orderby cap.ValidTo descending
                                                           select new MaximumFishingCapacityDTO
                                                           {
                                                               Id = cap.Id,
                                                               Date = cap.RegulationDate,
                                                               Regulation = cap.Regulation,
                                                               GrossTonnage = cap.GrossTonnage,
                                                               Power = cap.EnginePower,
                                                               IsActive = true
                                                           };

            return result;
        }
    }
}
