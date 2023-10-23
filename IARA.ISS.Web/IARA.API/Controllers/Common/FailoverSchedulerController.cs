using System.Net.Mime;
using IARA.WebHelpers;
using Microsoft.AspNetCore.Mvc;
using TL.BatchWorkers.FailoverCluster.Controllers;
using TL.BatchWorkers.FailoverCluster.Models;
using TL.SysToSysSecCom.Abstractions.Interfaces;

namespace IARA.WebAPI.Controllers.Common
{
    [AreaRoute(AreaType.Common)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces("application/json")]
    public class FailoverSchedulerController : BaseFailoverController
    {
        public FailoverSchedulerController(ICryptoHelper cryptoHelper, IFailoverSchedulerService service)
            : base(cryptoHelper, service)
        { }
    }
}
