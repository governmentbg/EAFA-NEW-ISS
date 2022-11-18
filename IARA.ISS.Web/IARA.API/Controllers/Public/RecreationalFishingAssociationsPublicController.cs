using IARA.Common.Exceptions;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.RecreationalFishing.Associations;
using IARA.Interfaces;
using IARA.Interfaces.Legals;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace IARA.WebAPI.Controllers.Public
{
    [AreaRoute(AreaType.Public)]
    public class RecreationalFishingAssociationsPublicController : BaseController
    {
        private readonly IRecreationalFishingAssociationService service;
        private readonly ILegalEntitiesService legalEntitiesService;
        private readonly IFileService fileService;
        private readonly IApplicationService applicationService;

        public RecreationalFishingAssociationsPublicController(IRecreationalFishingAssociationService service,
                                                               ILegalEntitiesService legalEntitiesService,
                                                               IFileService fileService,
                                                               IApplicationService applicationService,
                                                               IPermissionsService permissionsService) 
            : base(permissionsService)
        {
            this.service = service;
            this.legalEntitiesService = legalEntitiesService;
            this.fileService = fileService;
            this.applicationService = applicationService;
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult DownloadFile(int id)
        {
            if (legalEntitiesService.HasUserAccessToLegalEntityFile(CurrentUser.ID, id))
            {
                DownloadableFileDTO file = fileService.GetFileForDownload(id);
                return File(file.Bytes, file.MimeType, file.FileName);
            }
            return NotFound();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsAddRecords)]
        public IActionResult GetCurrentUserAsFishingAssociationPerson()
        {
            FishingAssociationPersonDTO person = service.GetFishingAssociationPersonFromUserId(CurrentUser.ID);
            return Ok(person);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead)]
        public IActionResult GetAssociationApplication([FromQuery] int id)
        {
            if (applicationService.IsApplicationSubmittedByUserOrPerson(CurrentUser.ID, id))
            {
                FishingAssociationApplicationEditDTO result = service.GetAssociationApplication(id, false);
                return Ok(result);
            }
            return NotFound();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetRegisterByApplicationId([FromQuery] int applicationId)
        {
            FishingAssociationEditDTO permit = service.GetRegisterByApplicationId(applicationId);
            return Ok(permit);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsAddRecords)]
        public IActionResult AddAssociationApplication([FromForm] FishingAssociationApplicationEditDTO association)
        {
            try
            {
                int id = service.AddAssociationApplication(association, null);
                return Ok(id);
            }
            catch (ObjectException error)
            {
                return BadRequest(error.Object);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsEditRecords)]
        public IActionResult EditAssociationApplication([FromForm] FishingAssociationApplicationEditDTO association)
        {
            try
            {
                if (association.ApplicationId.HasValue && applicationService.IsApplicationSubmittedByUser(CurrentUser.ID, association.ApplicationId.Value))
                {
                    service.EditAssociationApplication(association);
                    return Ok();
                }

                return NotFound();
            }
            catch (ObjectException error)
            {
                return BadRequest(error.Object);
            }
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetAssociationRoleName()
        {
            string role = service.GetAssociationRoleName();
            return Ok(role);
        }
    }
}
