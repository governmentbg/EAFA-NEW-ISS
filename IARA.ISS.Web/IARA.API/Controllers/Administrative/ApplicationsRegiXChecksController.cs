using System.Linq;
using IARA.DomainModels.DTOModels.ApplicationRegiXChecks;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Models;
using Microsoft.AspNetCore.Mvc;

namespace IARA.WebAPI.Controllers.Administrative
{
    [AreaRoute(AreaType.Administrative)]
    public class ApplicationsRegiXChecksController : BaseController
    {
        private readonly IApplicationRegiXChecksService service;

        public ApplicationsRegiXChecksController(IApplicationRegiXChecksService service,
                                                 IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.service = service;
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ApplicationRegiXChecksRead)]
        public IActionResult GetAll([FromBody] GridRequestModel<ApplicationRegiXChecksFilters> regixChecks)
        {
            IQueryable<ApplicationRegixCheckRequestDTO> result = service.GetAll(regixChecks.Filters);
            return PageResult(result, regixChecks, false);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ApplicationRegiXChecksRead)]
        public IActionResult Get([FromQuery] int id)
        {
            ApplicationRegixCheckRequestEditDTO result = service.Get(id);
            return Ok(result);
        }
    }
}
