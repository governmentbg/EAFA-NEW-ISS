using System;
using System.Collections.Generic;
using System.Text;
using IARA.Common.Enums;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.Application
{
    public class ApplicationDeliveryTypeDTO : NomenclatureDTO
    {
        public PageCodeEnum PageCode { get; set; }
    }
}
