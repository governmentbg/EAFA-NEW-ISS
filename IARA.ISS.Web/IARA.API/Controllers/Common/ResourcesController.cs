using System.Collections.Generic;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.Interfaces;
using IARA.Security;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using Microsoft.AspNetCore.Mvc;
using TL.Caching.Interfaces;
using TL.Caching.Models;

namespace IARA.WebAPI.Controllers.Common
{
    [AreaRoute(AreaType.Common)]
    public class ResourcesController : BaseController
    {
        private IMemoryCacheService memoryCache;

        public ResourcesController(IPermissionsService permissionsService, IMemoryCacheService memoryCache)
            : base(permissionsService)
        {
            this.memoryCache = memoryCache;
        }

        [HttpGet]
        public async Task<IActionResult> GetWebResources([FromQuery] ResourceLanguageEnum language = ResourceLanguageEnum.BG)
        {
            string resourceName = $"resources{language}";

            var settings = new CachingSettings<ITranslationService, Dictionary<string, Dictionary<string, string>>>(resourceName, (service) =>
            {
                return service.GetWebResources(language);
            });

            var resources = memoryCache.GetCachedObject(settings);

            return Ok(resources);
        }
    }
}
