using System;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.CatchesAndSales
{
    public class OriginDeclarationFishDTO
    {
        public int? Id { get; set; }

        public int? OriginDeclarationId { get; set; }

        public int? CatchRecordFishId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? FishId { get; set; }

        /// <summary>
        /// Only for UI in table
        /// </summary>
        public string FishName { get; set; } 

        /// <summary>
        /// Only for UI in table
        /// </summary>
        public string CatchZone { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? CatchQuadrantId { get; set; }

        /// <summary>
        /// Only for UI in table
        /// </summary>
        public string CatchQuadrant { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? CatchFishStateId { get; set; }

        /// <summary>
        /// Only for UI in table
        /// </summary>
        public string CatchFishStateName { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? CatchFishPresentationId { get; set; }

        /// <summary>
        /// Only for UI in table 
        /// </summary>
        public string CatchFishPresentationName { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsProcessedOnBoard { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public decimal? QuantityKg { get; set; }

        public decimal? UnloadedProcessedQuantityKg { get; set; }

        public DateTime? TransboradDateTime { get; set; }

        public int? TransboardShipId { get; set; }

        public int? TransboardTargetPortId { get; set; }

        [StringLength(4000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Comments { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsActive { get; set; }


        /// <summary>
        /// For UI puproses only
        /// </summary>
        public bool IsValid { get; set; }
    }
}
