using System.ComponentModel.DataAnnotations;
using IARA.Common.Enums;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.Common
{
    public class AddressRegistrationDTO
    {
        public AddressTypesEnum AddressType { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public int? DistrictId { get; set; }
        public string DistrictName { get; set; }
        public int? MunicipalityId { get; set; }
        public string MunicipalityName { get; set; }
        public int? PopulatedAreaId { get; set; }
        public string PopulatedAreaName { get; set; }

        [StringLength(200, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Region { get; set; }

        [StringLength(10, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string PostalCode { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [StringLength(200, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Street { get; set; }

        [StringLength(10, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string StreetNum { get; set; }

        [StringLength(10, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string BlockNum { get; set; }

        [StringLength(10, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string EntranceNum { get; set; }

        [StringLength(10, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string FloorNum { get; set; }

        [StringLength(10, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string ApartmentNum { get; set; }
    }
}
