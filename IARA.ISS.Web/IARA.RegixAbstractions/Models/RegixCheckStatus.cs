using IARA.Common.Enums;

namespace IARA.RegixAbstractions.Models
{
    public class RegixCheckStatus
    {
        public RegixCheckStatus(RegixCheckStatusesEnum status, string errorDescription = null)
        {
            this.Status = status;
            this.ErrorDescription = errorDescription;
        }

        public RegixCheckStatusesEnum Status { get; set; }
        public string ErrorDescription { get; set; }
    }
}
