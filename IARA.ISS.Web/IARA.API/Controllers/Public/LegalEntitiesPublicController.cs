using IARA.Common.Exceptions;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.LegalEntities;
using IARA.Interfaces;
using IARA.Interfaces.Legals;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Administrative
{
    [AreaRoute(AreaType.Public)]
    public class LegalEntitiesPublicController : BaseController
    {
        private readonly ILegalEntitiesService service;
        private readonly IFileService fileService;
        private readonly IApplicationService applicationService;

        public LegalEntitiesPublicController(ILegalEntitiesService service,
                                             IFileService fileService,
                                             IPermissionsService permissionsService,
                                             IApplicationService applicationService)
            : base(permissionsService)
        {
            this.service = service;
            this.fileService = fileService;
            this.applicationService = applicationService;
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult DownloadFile(int id)
        {
            if (service.HasUserAccessToLegalEntityFile(CurrentUser.ID, id))
            {
                DownloadableFileDTO file = fileService.GetFileForDownload(id);
                return File(file.Bytes, file.MimeType, file.FileName);
            }
            return NotFound();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsAddRecords)]
        public IActionResult GetCurrentUserAsAuthorizedPerson()
        {
            AuthorizedPersonDTO person = service.GetAuthorizedPersonFromUserId(CurrentUser.ID);
            return Ok(person);
        }

        // applications
        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead)]
        public IActionResult GetLegalEntityApplication([FromQuery] int id)
        {
            if (applicationService.IsApplicationSubmittedByUserOrPerson(CurrentUser.ID, id))
            {
                LegalEntityApplicationEditDTO result = service.GetLegalEntityApplication(id);
                return Ok(result);
            }
            return NotFound();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetRegisterByApplicationId([FromQuery] int applicationId)
        {
            LegalEntityEditDTO permit = service.GetRegisterByApplicationId(applicationId);
            return Ok(permit);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsAddRecords)]
        public IActionResult AddLegalEntityApplication([FromForm] LegalEntityApplicationEditDTO legalEntity)
        {
            try
            {
                int id = service.AddLegalEntityApplication(legalEntity, null);
                return Ok(id);
            }
            catch (ObjectException error)
            {
                return BadRequest(error.Object);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsEditRecords)]
        public IActionResult EditLegalEntityApplication([FromForm] LegalEntityApplicationEditDTO legalEntity)
        {
            try
            {
                if (legalEntity.ApplicationId.HasValue && applicationService.IsApplicationSubmittedByUser(CurrentUser.ID, legalEntity.ApplicationId.Value))
                {
                    service.EditLegalEntityApplication(legalEntity);
                    return Ok();
                }

                return NotFound();
            }
            catch (ObjectException error)
            {
                return BadRequest(error.Object);
            }
        }
    }
}
