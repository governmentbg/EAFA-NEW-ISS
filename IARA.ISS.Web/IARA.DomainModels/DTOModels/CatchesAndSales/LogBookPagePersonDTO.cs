using System.Collections.Generic;
using IARA.Common.CustomValidators;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.CatchesAndSales
{
    public class LogBookPagePersonDTO
    {
        public LogBookPagePersonTypesEnum PersonType { get; set; }

        [RequiredIf(nameof(PersonType), "msgRequired", typeof(ErrorResources), LogBookPagePersonTypesEnum.RegisteredBuyer)]
        public int? BuyerId { get; set; }

        public RegixPersonDataDTO Person { get; set; }

        public RegixLegalDataDTO PersonLegal { get; set; }

        public List<AddressRegistrationDTO> Addresses { get; set; }
    }
}
