namespace IARA.DomainModels.DTOModels.FishingCapacity
{
    public class FishingCapacityStatisticsDTO
    {
        /// <summary>
        /// Пределен риболовен капацитет
        /// </summary>
        public SimpleFishingCapacityDTO MaximumFishingCapacity { get; set; }

        /// <summary>
        /// Общ капацитет от активни удостоверения за свободен капацитет
        /// </summary>
        public SimpleFishingCapacityDTO TotalCapacityFromActiveCertificates { get; set; }

        /// <summary>
        /// Общ капацитет, разпределен по кораби
        /// </summary>
        public SimpleFishingCapacityDTO TotalActiveShipFishingCapacity { get; set; }

        /// <summary>
        /// Общ неизползван капацитет (към ИАРА) - нито в удостоверение, нито на кораб
        /// </summary>
        public SimpleFishingCapacityDTO TotalUnusedFishingCapacity { get; set; }
    }
}
