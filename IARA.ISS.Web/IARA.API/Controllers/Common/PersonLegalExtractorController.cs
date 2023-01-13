using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Common;
using IARA.Interfaces.Common;
using IARA.Security;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace IARA.WebAPI.Controllers.Common
{
    [AreaRoute(AreaType.Common)]
    public class PersonLegalExtractorController : BaseController
    {
        private readonly IPersonLegalExtractorService service;

        public PersonLegalExtractorController(IPersonLegalExtractorService service, IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.service = service;
        }

        [HttpGet]
        [CustomAuthorize]
        public IActionResult TryGetPerson([FromQuery] IdentifierTypeEnum identifierType, [FromQuery] string identifier)
        {
            PersonFullDataDTO result = service.TryGetPerson(identifierType, identifier);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public IActionResult TryGetLegal([FromQuery] string eik)
        {
            LegalFullDataDTO result = service.TryGetLegal(eik);
            return Ok(result);
        }
    }
}
