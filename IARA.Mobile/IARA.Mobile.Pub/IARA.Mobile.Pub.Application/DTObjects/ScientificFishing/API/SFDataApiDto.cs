using System.Collections.Generic;

namespace IARA.Mobile.Pub.Application.DTObjects.ScientificFishing.API
{
    public class SFDataApiDto
    {
        public List<SFPermitApiDto> Permits { get; set; }
        public List<SFHolderApiDto> Holders { get; set; }
        public List<SFOutingApiDto> Outings { get; set; }
        public List<SFOutingCatchApiDto> Catches { get; set; }
        public List<SFPermitReasonApiDto> PermitReasons { get; set; }
    }
}
