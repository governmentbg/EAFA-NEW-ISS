using System;
using System.Collections.Generic;
using System.Text;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class InspectedEntityEmailDTO
    {
        public int? InspectionId { get; set; }
        public int? InspectedPersonId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public int? PersonId { get; set; }
        public int? LegalId { get; set; }
        public int? UnregisteredPersonId { get; set; }
        public bool SendEmail { get; set; }
        public string InspectedPersonType { get; set; }
    }
}
