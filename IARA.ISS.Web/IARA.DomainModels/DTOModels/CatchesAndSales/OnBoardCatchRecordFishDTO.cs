using System;

namespace IARA.DomainModels.DTOModels.CatchesAndSales
{
    public class OnBoardCatchRecordFishDTO : CatchRecordFishDTO
    {
        public int ShipLogBookPageId { get; set; }

        public string ShipLogBookPageNumber { get; set; }

        public DateTime TripStartDateTime { get; set; }

        public DateTime TripEndDateTime { get; set; }

        /// <summary>
        /// Set and used only in UI
        /// </summary>
        public bool isChecked { get; set; }
    }
}
