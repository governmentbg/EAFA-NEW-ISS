using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.Application
{
    public class ApplicationRegisterDataDTO
    {
        public RecordTypesEnum RecordType { get; set; }

        public int? ApplicationId { get; set; }
    }
}
