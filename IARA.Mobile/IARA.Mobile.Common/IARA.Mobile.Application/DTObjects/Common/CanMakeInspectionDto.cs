using System;
using System.Collections.Generic;
using System.Text;

namespace IARA.Mobile.Application.DTObjects.Common
{
    public class CanMakeInspectionDto
    {
        public bool CanMakeInspection { get; set; }
        public int UnresolvedCrossChecks { get; set; }
        public int LockDays { get; set; }
        public TimeSpan TimeRemainingUntilInspectorLock { get; set; }
        public List<CrossCheckResultDto> CrossChecks { get; set; } = new List<CrossCheckResultDto>();
    }
}
