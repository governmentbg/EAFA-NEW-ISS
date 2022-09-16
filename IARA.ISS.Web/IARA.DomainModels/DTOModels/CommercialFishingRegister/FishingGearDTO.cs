using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.CommercialFishingRegister
{
    public class FishingGearDTO
    {
        public int? Id { get; set; }

        public int TypeId { get; set; }

        public string Type { get; set; } // for display in table

        public int Count { get; set; }

        public string MarksNumbers { get; set; } // for display in table

        public decimal? Length { get; set; }

        public decimal? Height { get; set; }

        public decimal? NetEyeSize { get; set; }

        public int? HookCount { get; set; }

        [StringLength(4000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Description { get; set; }

        public int? TowelLength { get; set; }

        public int? HouseLength { get; set; }

        public int? HouseWidth { get; set; }

        public decimal? CordThickness { get; set; }

        public bool? HasPingers { get; set; }

        public int? PermitId { get; set; }

        public bool IsActive { get; set; }

        public List<FishingGearMarkDTO> Marks { get; set; }

        public List<FishingGearPingerDTO> Pingers { get; set; }
    }
}
