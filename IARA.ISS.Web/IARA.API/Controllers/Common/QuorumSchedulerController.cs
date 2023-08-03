using System.Net.Mime;
using IARA.WebHelpers;
using Microsoft.AspNetCore.Mvc;
using TL.BatchWorkers.Abstractions.Interfaces.Internal;
using TL.BatchWorkers.Quorum.Controllers;
using TL.SysToSysSecCom.Abstractions.Interfaces;

namespace IARA.WebAPI.Controllers.Common
{
    [AreaRoute(AreaType.Common)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces("application/json")]
    public class QuorumSchedulerController : BaseQuorumSchedulerController
    {
        public QuorumSchedulerController(ICryptoHelper cryptoHelper, IQuorumServerAPISchedulerService schedulerService)
            : base(cryptoHelper, schedulerService)
        { }
    }
}
