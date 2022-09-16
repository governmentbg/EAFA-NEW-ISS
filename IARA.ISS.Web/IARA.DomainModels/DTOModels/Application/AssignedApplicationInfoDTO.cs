using System;
using System.Collections.Generic;
using System.Text;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.Application
{
    public class AssignedApplicationInfoDTO
    {
        public int Id { get; set; }

        public string StatusCode { get; set; }

        public PageCodeEnum PageCode { get; set; }
    }
}
