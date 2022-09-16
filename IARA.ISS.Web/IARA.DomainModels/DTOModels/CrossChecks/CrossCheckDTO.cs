using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.CrossChecks
{
    public class CrossCheckDTO
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string GroupName { get; set; }

        public string Purpose { get; set; }

        public short ErrorLevel { get; set; }

        public string DataSource { get; set; }

        public string DataSourceColumns { get; set; }

        public string CheckSource { get; set; }

        public string CheckSourceColumns { get; set; }

        public string CheckTableName { get; set; }

        public CrossChecksAutoExecFrequencyEnum AutoExecFrequency { get; set; }

        public bool IsActive { get; set; }
    }
}
