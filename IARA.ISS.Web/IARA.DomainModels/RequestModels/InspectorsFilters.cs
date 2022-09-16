using System;
using System.Collections.Generic;
using System.Text;

namespace IARA.DomainModels.RequestModels
{
    public class InspectorsFilters : BaseRequestModel
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UnregPersonName { get; set; }
        public string EgnLnc { get; set; }
        public string Institution { get; set; }
        public int? InspectionSequenceNum { get; set; }
        public int? UserId { get; set; }
        public int? InstitutionId { get; set; }
        public string InspectorCardNum { get; set; }
    }
}
