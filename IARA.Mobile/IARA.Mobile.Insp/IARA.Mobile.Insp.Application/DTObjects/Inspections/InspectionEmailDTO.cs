using System;
using System.Collections.Generic;
using System.Text;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class InspectionEmailDTO
    {
        public int InspectionId { get; set; }

        public List<InspectedEntityEmailDTO> InspectedEntityEmails { get; set; }
    }
}
