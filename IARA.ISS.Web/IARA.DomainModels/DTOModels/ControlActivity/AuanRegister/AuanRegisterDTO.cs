using System;

namespace IARA.DomainModels.DTOModels.ControlActivity.AuanRegister
{
    public class AuanRegisterDTO
    {
        public int Id { get; set; }

        public int InspectionId { get; set; }

        public string AuanNum { get; set; }

        public string InspectedEntity { get; set; }

        public string Drafter { get; set; }

        public DateTime DraftDate { get; set; }

        public bool IsActive { get; set; }
    }
}
