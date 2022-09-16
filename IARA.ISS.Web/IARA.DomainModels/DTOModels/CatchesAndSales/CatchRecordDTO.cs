using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.CatchesAndSales
{
    public class CatchRecordDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? CatchOperationsCount { get; set; }

        public int? Depth { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? GearEntryTime { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? GearExitTime { get; set; }

        public int? TransboardedFromShipId { get; set; }

        /// <summary>
        /// Used in UI only
        /// </summary>
        public string TotalTime { get; set; }

        public List<CatchRecordFishDTO> CatchRecordFishes { get; set; }

        /// <summary>
        /// Used in UI only
        /// </summary>
        public string CatchRecordFishesSummary { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsActive { get; set; }
    }
}
