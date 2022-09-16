using System;
using System.Collections.Generic;
using System.Text;
using IARA.Common.Enums;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.Reports
{
    public class ReportParameterExecuteDTO
    {
        public ReportParameterTypeEnum DataType { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsActive { get; set; }
        public int Id { get; set; }
        public int ParameterId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string DefaultValue { get; set; }
        public string Pattern { get; set; }
        public string Description { get; set; }
        public string ErrorMessage { get; set; }
        public List<NomenclatureDTO> ParameterTypeNomenclatures { get; set; }
        public string NomenclatureSQL { get; set; }
    }
}
