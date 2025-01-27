using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IARA.DomainModels.DTOModels.CrossChecks;

namespace IARA.DomainModels.DTOModels.User
{
    public class CanMakeInspectionDTO
    {
        public bool CanMakeInspection { get; set; }
        public int UnresolvedCrossChecks { get; set; }
        public int LockDays { get; set; }
        public TimeSpan TimeRemainingUntilInspectorLock { get; set; }
        public List<CrossCheckResultDTO> CrossChecks { get; set; } = new List<CrossCheckResultDTO>();
    }
}
