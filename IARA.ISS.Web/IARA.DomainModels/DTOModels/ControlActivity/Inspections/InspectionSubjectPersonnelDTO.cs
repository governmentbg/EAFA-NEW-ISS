using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class InspectionSubjectPersonnelDTO : UnregisteredPersonDTO
    {
        public bool IsRegistered { get; set; }
        public int? EntryId { get; set; }
        public InspectedPersonTypeEnum Type { get; set; }
        public InspectionSubjectAddressDTO RegisteredAddress { get; set; }
    }
}
