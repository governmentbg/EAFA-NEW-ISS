using System.Collections.Generic;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Domain.Enums;

namespace IARA.Mobile.Application.DTObjects.Reports
{
    public class ReportParameterDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsMandatory { get; set; }
        public ReportParameterType DataType { get; set; }
        public string DefaultValue { get; set; }
        public string Pattern { get; set; }
        public string ErrorMessage { get; set; }
        public List<NomenclatureDto> Nomenclatures { get; set; }
    }
}
