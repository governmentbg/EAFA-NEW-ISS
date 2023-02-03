using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.Application
{
    public class AddApplicationResultDTO
    {
        public int ApplicationId { get; set; }

        public string AccessCode { get; set; }

        public ApplicationHierarchyTypesEnum ApplicationHierarchyType { get; set; }
    }
}
