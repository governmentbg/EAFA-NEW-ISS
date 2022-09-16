using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.Reports
{
    public class ExecutionParamDTO
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public ReportParameterTypeEnum Type { get; set; }
        public string DefaultValue { get; set; }
        public string Pattern { get; set; }
        public bool IsMandatory { get; set; }
    }
}
