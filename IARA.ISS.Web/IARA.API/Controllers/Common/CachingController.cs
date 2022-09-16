using IARA.Logging.Abstractions.Interfaces;
using IARA.WebHelpers;
using Microsoft.AspNetCore.Mvc;
using TL.Caching.Controllers;
using TL.Caching.Interfaces;
using TL.Caching.Models;
using TL.SysToSysSecCom.Interfaces;
using TL.SysToSysSecCom.Utils;

namespace IARA.WebAPI.Controllers.Common
{
    [AreaRoute(AreaType.Common)]
    public class CachingController : BaseCachingController
    {
        private IExtendedLogger logger;

        public CachingController(ICryptoHelper cryptoHelper, IMemoryCacheService memoryCacheService, IExtendedLogger logger)
            : base(cryptoHelper, memoryCacheService)
        {
            this.logger = logger;
        }

        public override IActionResult RefreshCache([FromSecureBody] CachingKeyModel key)
        {
            logger.LogInfo(key.ToString());
            return base.RefreshCache(key);
        }
    }
}
