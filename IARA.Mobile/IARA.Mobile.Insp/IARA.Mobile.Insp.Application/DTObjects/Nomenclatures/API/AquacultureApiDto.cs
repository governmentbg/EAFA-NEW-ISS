using IARA.Mobile.Domain.Interfaces;

namespace IARA.Mobile.Insp.Application.DTObjects.Nomenclatures.API
{
    public class AquacultureApiDto : IActive
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UrorNum { get; set; }
        public int LegalId { get; set; }
        public bool IsActive { get; set; }
    }
}
