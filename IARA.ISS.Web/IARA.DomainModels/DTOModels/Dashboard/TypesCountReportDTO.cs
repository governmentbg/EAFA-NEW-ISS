using System;
using System.Collections.Generic;
using System.Text;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.Dashboard
{
    public class TypesCountReportDTO
    {
        public string Icon { get; set; }
        public string Name { get; set; }
        public PageCodeEnum PageCode { get; set; }
        public int Count { get; set; }
    }
}
