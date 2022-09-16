using IARA.Mobile.Domain.Enums;

namespace IARA.Mobile.Application.DTObjects.Reports
{
    public class ExecutionParamDto
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public ReportParameterType Type { get; set; }
    }
}
