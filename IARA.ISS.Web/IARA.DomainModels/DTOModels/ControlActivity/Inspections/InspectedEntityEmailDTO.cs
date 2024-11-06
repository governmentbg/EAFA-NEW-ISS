using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class InspectedEntityEmailDTO
    {
        public int? InspectionId { get; set; }

        public string Name { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public int? PersonId { get; set; }

        public int? LegalId { get; set; }

        public InspectedPersonTypeEnum? Type { get; set; }
    }
}
