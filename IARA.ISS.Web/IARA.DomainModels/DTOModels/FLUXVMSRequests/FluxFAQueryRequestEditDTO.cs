using System;
using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Enums;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.FLUXVMSRequests
{
    public class FluxFAQueryRequestEditDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public FluxFAQueryTypesEnum? QueryType { get; set; }

        [RequiredIf(nameof(QueryType), "msgRequired", typeof(ErrorResources), FluxFAQueryTypesEnum.VESSEL)]
        public DateTime? DateTimeFrom { get; set; }

        [RequiredIf(nameof(QueryType), "msgRequired", typeof(ErrorResources), FluxFAQueryTypesEnum.VESSEL)]
        public DateTime? DateTimeTo { get; set; }

        public string VesselCFR { get; set; }

        public string VesselIRCS { get; set; }

        [RequiredIf(nameof(QueryType), "msgRequired", typeof(ErrorResources), FluxFAQueryTypesEnum.TRIP)]
        public string TripIdentifier { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? Consolidated { get; set; }
    }
}
