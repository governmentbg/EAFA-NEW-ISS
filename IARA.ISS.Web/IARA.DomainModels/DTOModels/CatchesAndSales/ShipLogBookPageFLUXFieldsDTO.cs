using System;
using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.CatchesAndSales
{
    public class ShipLogBookPageFLUXFieldsDTO
    {
        public int PageId { get; set; }

        /// <summary>
        /// Related to activity type: DEPARTURE
        /// </summary>
        public int? DeparturePortId { get; set; }

        /// <summary>
        /// Related to activity type: ARRIVAL (Declaration and Notification)
        /// </summary>
        public int? ArrivalPortId { get; set; }

        /// <summary>
        /// Related to activity type: DEPARTURE (FA OccurrenceDateTime)
        /// </summary>
        public DateTime? PageFillDate { get; set; }

        /// <summary>
        /// Related to activity type: DEPARTURE (the used fishing gear)
        /// and activity type: FISHING OPERATION (the used fishing gear for the operation)
        /// </summary>
        public int? FishingGearRegisterId { get; set; }

        /// <summary>
        /// Related to activity type: DEPARTURE (number of fishing gears on board)
        /// </summary>
        public int? FishingGearCount { get; set; }

        /// <summary>
        /// Related to activity type: DEPARTURE (FA OccurrenceDateTime)
        /// </summary>
        public DateTime? FishTripStartDateTime { get; set; }

        /// <summary>
        /// Related to activity type: ARRIVAL (Declaration and Notification) - FA OccurrenceDateTime
        /// </summary>
        public DateTime? FishTripEndDateTime { get; set; }

        /// <summary>
        /// Related to activity type: FISHING OPERATION
        /// </summary>
        public List<CatchRecordDTO> CatchRecords { get; set; }

        /// <summary>
        /// Related to activity type: LANDING
        /// Could be related to activity type: TRANSHIPMENT, if there is fish transboarded to another ship
        /// </summary>
        public List<OriginDeclarationFishDTO> OriginDeclarationFishes { get; set; }

        public bool? IsCancelled { get; set; }

        public bool? IsActive { get; set; }
    }
}
