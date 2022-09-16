using IARA.Common.Enums;

namespace IARA.RegixAbstractions.Models.ApplicationIds
{
    public class BaseRegixApplicationDataIds
    {
        public int ApplicationId { get; set; }

        public PageCodeEnum PageCode { get; set; }

        public int SubmittedByPersonId { get; set; }

        public int SubmittedByPersonRoleId { get; set; }

        public int SubmittedForPersonId { get; set; }

        public int SubmittedForLegalId { get; set; }
    }
}
