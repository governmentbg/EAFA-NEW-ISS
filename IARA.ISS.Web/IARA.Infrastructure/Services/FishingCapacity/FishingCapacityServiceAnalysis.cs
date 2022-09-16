using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.Common.Exceptions;
using IARA.DomainModels.DTOModels.FishingCapacity;
using IARA.Interfaces;

namespace IARA.Infrastructure.Services
{
    public partial class FishingCapacityService : Service, IFishingCapacityService
    {
        public FishingCapacityStatisticsDTO GetFishingCapacityStatistics(int year, int month, int day)
        {
            DateTime date = new DateTime(year, month, day);

            FishingCapacityStatisticsDTO result = new FishingCapacityStatisticsDTO
            {
                MaximumFishingCapacity = GetMaximumFishingCapacity(date),
                TotalCapacityFromActiveCertificates = GetTotalCapacityFromActiveCertificates(date),
                TotalActiveShipFishingCapacity = GetTotalActiveShipFishingCapacity(date)
            };

            result.TotalUnusedFishingCapacity = GetTotalUnusedFishingCapacity(result.MaximumFishingCapacity,
                                                                              result.TotalCapacityFromActiveCertificates,
                                                                              result.TotalActiveShipFishingCapacity);

            return result;
        }

        /// <summary>
        /// Get the maximum fishing capacity up to a date or throw if no such entry exists
        /// </summary>
        /// <param name="date">The date up to which to query</param>
        /// <returns>The maximum fishing capacity</returns>
        private SimpleFishingCapacityDTO GetMaximumFishingCapacity(DateTime date)
        {
            SimpleFishingCapacityDTO result = (from cap in Db.CountryCapacityRegister
                                               where cap.RegulationDate <= date
                                               orderby cap.RegulationDate descending
                                               select new SimpleFishingCapacityDTO
                                               {
                                                   GrossTonnage = cap.GrossTonnage,
                                                   EnginePower = cap.EnginePower
                                               }).FirstOrDefault();

            return result ?? throw new NoMaximumFishingCapacityToDateException();
        }

        /// <summary>
        /// Get the total capacity from all active free capacity certificates up to a date
        /// </summary>
        /// <param name="date">The date up to which to query</param>
        /// <returns>The total capacity from all active free capacity certificates</returns>
        private SimpleFishingCapacityDTO GetTotalCapacityFromActiveCertificates(DateTime date)
        {
            List<SimpleFishingCapacityDTO> certificates = (from cert in Db.CapacityCertificatesRegister
                                                           where cert.RecordType == nameof(RecordTypesEnum.Register)
                                                                && cert.CertificateValidFrom <= date
                                                                && cert.CertificateValidTo >= date
                                                                && cert.IsActive
                                                           select new SimpleFishingCapacityDTO
                                                           {
                                                               GrossTonnage = cert.GrossTonnage,
                                                               EnginePower = cert.MainEnginePower
                                                           }).ToList();

            SimpleFishingCapacityDTO result = GetTotalSumOfCapacities(certificates);
            return result;
        }

        /// <summary>
        /// Get the total capacity that is currently used by active ships up to a date
        /// </summary>
        /// <param name="date">The date up to which to query</param>
        /// <returns>The total capacity that is currently used by active ships</returns>
        private SimpleFishingCapacityDTO GetTotalActiveShipFishingCapacity(DateTime date)
        {
            List<SimpleFishingCapacityDTO> capacities = (from cap in Db.ShipCapacityRegister
                                                         where cap.RecordType == nameof(RecordTypesEnum.Register)
                                                            && cap.ValidFrom <= date
                                                            && cap.ValidTo >= date
                                                         select new SimpleFishingCapacityDTO
                                                         {
                                                             GrossTonnage = cap.GrossTonnage,
                                                             EnginePower = cap.EnginePower
                                                         }).ToList();

            SimpleFishingCapacityDTO result = GetTotalSumOfCapacities(capacities);
            return result;
        }

        /// <summary>
        /// Calculate the unused fishing capacity given the maximum capacity, the capacity from all active certificates
        /// and the capacity currently used by ships
        /// </summary>
        /// <param name="maxCap">The maximum fishing capacity</param>
        /// <param name="certCap">The capacity from all active certificates</param>
        /// <param name="shipCap">The capacity currently used by ships</param>
        /// <returns>The unused fishing capacity</returns>
        private static SimpleFishingCapacityDTO GetTotalUnusedFishingCapacity(SimpleFishingCapacityDTO maxCap, SimpleFishingCapacityDTO certCap, SimpleFishingCapacityDTO shipCap)
        {
            SimpleFishingCapacityDTO result = new SimpleFishingCapacityDTO
            {
                GrossTonnage = maxCap.GrossTonnage - certCap.GrossTonnage - shipCap.GrossTonnage,
                EnginePower = maxCap.EnginePower - certCap.EnginePower - shipCap.EnginePower
            };

            return result;
        }

        private SimpleFishingCapacityDTO GetTotalSumOfCapacities(List<SimpleFishingCapacityDTO> capacities)
        {
            SimpleFishingCapacityDTO result = new SimpleFishingCapacityDTO
            {
                GrossTonnage = capacities.Sum(x => x.GrossTonnage),
                EnginePower = capacities.Sum(x => x.EnginePower)
            };

            return result;
        }
    }
}
