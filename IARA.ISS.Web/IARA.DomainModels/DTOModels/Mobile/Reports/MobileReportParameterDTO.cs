using System.Collections.Generic;
using IARA.Common.Enums;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.Mobile.Reports
{
    public class MobileReportParameterDTO
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsMandatory { get; set; }
        public ReportParameterTypeEnum DataType { get; set; }
        public string DefaultValue { get; set; }
        public string Pattern { get; set; }
        public string ErrorMessage { get; set; }
        public List<NomenclatureDTO> Nomenclatures { get; set; }
    }
}
