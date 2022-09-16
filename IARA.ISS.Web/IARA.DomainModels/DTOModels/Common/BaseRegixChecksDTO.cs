using System.Collections.Generic;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Application;

namespace IARA.DomainModels.DTOModels.Common
{
    public abstract class BaseRegixChecksDTO
    {
        public virtual int? ApplicationId { get; set; }

        public virtual PageCodeEnum? PageCode { get; set; }

        public string StatusReason { get; set; }

        public List<ApplicationRegiXCheckDTO> ApplicationRegiXChecks { get; set; }
    }
}
