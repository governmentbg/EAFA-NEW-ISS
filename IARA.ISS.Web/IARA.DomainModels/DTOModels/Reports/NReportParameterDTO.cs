using System;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Enums;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.Reports
{
    public class NReportParameterDTO
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string DataType { get; set; }

        public string DefaultValue { get; set; }

        public string ErrorMessage { get; set; }

        public string Pattern { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public bool IsActive { get; set; }
    }
}
