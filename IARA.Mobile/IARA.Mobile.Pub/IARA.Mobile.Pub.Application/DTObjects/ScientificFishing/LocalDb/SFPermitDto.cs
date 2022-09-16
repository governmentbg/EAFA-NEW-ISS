using System;

namespace IARA.Mobile.Pub.Application.DTObjects.ScientificFishing.LocalDb
{
    public class SFPermitDto
    {
        public int Id { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string RequesterName { get; set; }
        public string ScientificOrganizationName { get; set; }
        public bool HasOutings { get; set; }
    }
}
