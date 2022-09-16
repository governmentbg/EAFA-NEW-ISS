using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.ApplicationsRegister;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.QualifiedFrishersRegister;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Enums;
using IARA.WebHelpers.Models;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers
{
    [AreaRoute(AreaType.Administrative)]
    public class QualifiedFishersController : BaseAuditController
    {
        private readonly IQualifiedFishersService service;
        private readonly IFileService fileService;
        private readonly IApplicationService applicationService;
        private readonly IApplicationsRegisterService applicationsRegisterService;
        private readonly IUserService userService;
        private readonly IDeliveryService deliveryService;

        public QualifiedFishersController(IQualifiedFishersService service,
                                          IFileService fileService,
                                          IPermissionsService permissionsService,
                                          IApplicationService applicationService,
                                          IApplicationsRegisterService applicationsRegisterService,
                                          IUserService userService,
                                          IDeliveryService deliveryService)
            : base(permissionsService)
        {
            this.service = service;
            this.fileService = fileService;
            this.applicationService = applicationService;
            this.applicationsRegisterService = applicationsRegisterService;
            this.userService = userService;
            this.deliveryService = deliveryService;
        }

        [HttpPost]
        [CustomAuthorize(Permissions.QualifiedFishersAddRecords)]
        public IActionResult Add([FromForm] QualifiedFisherEditDTO fisher)
        {
            if (!service.PersonIsQualifiedFisherCheck(fisher.SubmittedForRegixData.EgnLnc, fisher.Id))
            {
                return Ok(service.AddRegisterEntry(fisher));
            }
            else
            {
                return ValidationFailedResult(errorCode: ErrorCode.QualifiedFisherAlreadyExists);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.QualifiedFishersAddRecords)]
        public async Task<IActionResult> AddAndDownloadRegister([FromForm] QualifiedFisherEditDTO fisher)
        {
            if (!service.PersonIsQualifiedFisherCheck(fisher.SubmittedForRegixData.EgnLnc, fisher.Id))
            {
                int registerId = service.AddRegisterEntry(fisher);

                DownloadableFileDTO file = await service.GetRegisterFileForDownload(registerId);
                return File(file.Bytes, file.MimeType, file.FileName);
            }
            else
            {
                return ValidationFailedResult(errorCode: ErrorCode.QualifiedFisherAlreadyExists);
            }
        }

        [HttpGet]
        [CustomAuthorize(Permissions.QualifiedFishersReadAll, Permissions.QualifiedFishersRead)]
        public IActionResult Get([FromQuery] int id)
        {
            QualifiedFisherEditDTO result = service.GetRegisterEntry(id);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.QualifiedFishersReadAll, Permissions.QualifiedFishersRead)]
        public IActionResult DownloadFile(int id)
        {
            DownloadableFileDTO file = fileService.GetFileForDownload(id);
            return File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.QualifiedFishersReadAll, Permissions.QualifiedFishersRead)]
        public IActionResult GetAll([FromBody] GridRequestModel<QualifiedFishersFilters> request)
        {
            if (!CurrentUser.Permissions.Contains(Permissions.QualifiedFishersReadAll))
            {
                int? territoryUnitId = userService.GetUserTerritoryUnitId(CurrentUser.ID);

                if (territoryUnitId.HasValue)
                {
                    if (request.Filters == null)
                    {
                        request.Filters = new QualifiedFishersFilters
                        {
                            ShowInactiveRecords = false
                        };
                    }

                    request.Filters.TerritoryUnitId = territoryUnitId.Value;
                }
                else
                {
                    return Ok(Enumerable.Empty<QualifiedFisherDTO>());
                }
            }

            IQueryable<QualifiedFisherDTO> result = service.GetAll(request.Filters);
            return PageResult(result, request, false);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.QualifiedFishersEditRecords)]
        public IActionResult Edit([FromForm] QualifiedFisherEditDTO fisher)
        {
            if (!service.PersonIsQualifiedFisherCheck(fisher.SubmittedForRegixData.EgnLnc, fisher.Id))
            {
                return Ok(service.EditRegisterEntry(fisher));
            }
            else
            {
                return ValidationFailedResult(errorCode: ErrorCode.QualifiedFisherAlreadyExists);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.QualifiedFishersEditRecords)]
        public async Task<IActionResult> EditAndDownloadRegister([FromForm] QualifiedFisherEditDTO fisher)
        {
            if (!service.PersonIsQualifiedFisherCheck(fisher.SubmittedForRegixData.EgnLnc, fisher.Id))
            {
                service.EditRegisterEntry(fisher);

                DownloadableFileDTO file = await service.GetRegisterFileForDownload(fisher.Id!.Value);
                return File(file.Bytes, file.MimeType, file.FileName);
            }
            else
            {
                return ValidationFailedResult(errorCode: ErrorCode.QualifiedFisherAlreadyExists);
            }
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.QualifiedFishersDeleteRecords)]
        public IActionResult Delete([FromQuery] int id)
        {
            service.Delete(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.QualifiedFishersRestoreRecords)]
        public IActionResult UndoDelete([FromQuery] int id)
        {
            service.UndoDelete(id);
            return Ok();
        }

        [HttpGet]
        public override IActionResult GetAuditInfo([FromQuery] int id)
        {
            return Ok(service.GetSimpleAudit(id));
        }

        // Applications

        [HttpPost]
        [CustomAuthorize(Permissions.QualifiedFishersApplicationsReadAll, Permissions.QualifiedFishersApplicationsRead)]
        public IActionResult GetAllApplications([FromBody] GridRequestModel<ApplicationsRegisterFilters> request)
        {
            if (!CurrentUser.Permissions.Contains(Permissions.QualifiedFishersApplicationsReadAll))
            {
                int? territoryUnitId = userService.GetUserTerritoryUnitId(CurrentUser.ID);

                if (territoryUnitId.HasValue)
                {
                    if (request.Filters == null)
                    {
                        request.Filters = new ApplicationsRegisterFilters
                        {
                            ShowInactiveRecords = false
                        };
                    }

                    request.Filters.TerritoryUnitId = territoryUnitId.Value;
                }
                else
                {
                    return Ok(Enumerable.Empty<ApplicationRegisterDTO>());
                }
            }

            IQueryable<ApplicationRegisterDTO> applications = applicationsRegisterService.GetAllApplications(request.Filters, null,
                new PageCodeEnum[] {
                    PageCodeEnum.QualiFi,
                    PageCodeEnum.CommFishLicense,
                    PageCodeEnum.CompetencyDup
                });
            return PageResult(applications, request, false);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.QualifiedFishersApplicationsEditRecords)]
        public IActionResult AssignApplicationViaAccessCode([FromQuery] string accessCode)
        {
            try
            {
                AssignedApplicationInfoDTO applicationData = applicationService.AssignApplicationViaAccessCode(accessCode, CurrentUser.ID,
                    new PageCodeEnum[]
                    {
                        PageCodeEnum.CompetencyDup,
                        PageCodeEnum.CommFishLicense
                    });
                return Ok(applicationData);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException)
            {
                return ValidationFailedResult(errorCode: ErrorCode.InvalidStateMachineTransitionOperation);
            }
        }

        [HttpGet]
        [CustomAuthorize(Permissions.QualifiedFishersApplicationsReadAll, Permissions.QualifiedFishersApplicationsRead)]
        public IActionResult GetApplication([FromQuery] int id)
        {
            QualifiedFisherApplicationEditDTO result = service.GetApplicationEntry(id);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.QualifiedFishersReadAll, Permissions.QualifiedFishersRead)]
        public IActionResult GetApplicationDataForRegister([FromQuery] int applicationId)
        {
            QualifiedFisherEditDTO result = service.GetEntryByApplicationId(applicationId);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.QualifiedFishersReadAll, Permissions.QualifiedFishersRead)]
        public IActionResult GetRegisterByApplicationId([FromQuery] int applicationId)
        {
            QualifiedFisherEditDTO permit = service.GetRegisterByApplicationId(applicationId);
            return Ok(permit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.QualifiedFishersApplicationsInspectAndCorrectRegiXData)]
        public IActionResult GetRegixData([FromQuery] int applicationId)
        {
            RegixChecksWrapperDTO<QualifiedFisherRegixDataDTO> result = service.GetApplicationRegixData(applicationId);
            return Ok(result);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.QualifiedFishersApplicationsAddRecords)]
        public async Task<IActionResult> AddApplication([FromForm] QualifiedFisherApplicationEditDTO fisher)
        {
            EgnLncDTO fisherEgnLnc;
            RegixPersonDataDTO person;
            List<AddressRegistrationDTO> addresses;

            if (fisher.SubmittedByRole == SubmittedByRolesEnum.Personal)
            {
                fisherEgnLnc = fisher.SubmittedByRegixData.EgnLnc;
                person = fisher.SubmittedByRegixData;
                addresses = fisher.SubmittedByAddresses;
            }
            else
            {
                fisherEgnLnc = fisher.SubmittedForRegixData.EgnLnc;
                person = fisher.SubmittedForRegixData;
                addresses = fisher.SubmittedForAddresses;
            }

            if (!service.PersonIsQualifiedFisherCheck(fisherEgnLnc, fisher.Id))
            {
                bool hasEDelivery = await deliveryService.HasSubmittedForEDelivery(fisher.DeliveryData.DeliveryTypeId,
                                                                                  new ApplicationSubmittedForRegixDataDTO
                                                                                  {
                                                                                      Person = person,
                                                                                      Addresses = addresses,
                                                                                      SubmittedByRole = fisher.SubmittedByRole
                                                                                  },
                                                                                  new ApplicationSubmittedByRegixDataDTO
                                                                                  {
                                                                                      Person = fisher.SubmittedByRegixData,
                                                                                      Addresses = fisher.SubmittedByAddresses
                                                                                  });

                if (hasEDelivery)
                {
                    int id = service.AddApplicationEntry(fisher, ApplicationStatusesEnum.EXT_CHK_STARTED);
                    return Ok(id);
                }
                else
                {
                    return ValidationFailedResult(errorCode: ErrorCode.NoEDeliveryRegistration);
                }
            }
            else
            {
                return ValidationFailedResult(errorCode: ErrorCode.QualifiedFisherAlreadyExists);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.QualifiedFishersApplicationsEditRecords)]
        public async Task<IActionResult> EditApplication([FromQuery] bool fromSaveAsDraft, [FromForm] QualifiedFisherApplicationEditDTO fisher)
        {
            EgnLncDTO fisherEgnLnc;
            RegixPersonDataDTO person;
            List<AddressRegistrationDTO> addresses;

            if (fisher.SubmittedByRole == SubmittedByRolesEnum.Personal)
            {
                fisherEgnLnc = fisher.SubmittedByRegixData.EgnLnc;
                person = fisher.SubmittedByRegixData;
                addresses = fisher.SubmittedByAddresses;
            }
            else
            {
                fisherEgnLnc = fisher.SubmittedForRegixData.EgnLnc;
                person = fisher.SubmittedForRegixData;
                addresses = fisher.SubmittedForAddresses;
            }

            if (!service.PersonIsQualifiedFisherCheck(fisherEgnLnc, fisher.Id))
            {
                bool hasEDelivery = await deliveryService.HasSubmittedForEDelivery(fisher.DeliveryData.DeliveryTypeId,
                                                                                  new ApplicationSubmittedForRegixDataDTO
                                                                                  {
                                                                                      SubmittedByRole = fisher.SubmittedByRole,
                                                                                      Person = person,
                                                                                      Addresses = addresses
                                                                                  },
                                                                                  new ApplicationSubmittedByRegixDataDTO
                                                                                  {
                                                                                      Person = fisher.SubmittedByRegixData,
                                                                                      Addresses = fisher.SubmittedByAddresses
                                                                                  });

                if (hasEDelivery)
                {
                    int id;

                    if (fromSaveAsDraft)
                    {
                        id = service.EditApplicationEntry(fisher);
                    }
                    else
                    {
                        id = service.EditApplicationEntry(fisher, ApplicationStatusesEnum.EXT_CHK_STARTED);
                    }

                    return Ok(id);
                }
                else
                {
                    return ValidationFailedResult(errorCode: ErrorCode.NoEDeliveryRegistration);
                }
            }
            else
            {
                return ValidationFailedResult(errorCode: ErrorCode.QualifiedFisherAlreadyExists);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.QualifiedFishersApplicationsInspectAndCorrectRegiXData)]
        public IActionResult EditApplicationAndStartRegixChecks([FromForm] QualifiedFisherApplicationEditDTO application)
        {
            service.EditApplicationRegixData(application);
            return Ok();
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.QualifiedFishersApplicationsDeleteRecords)]
        public IActionResult DeleteApplication([FromQuery] int id)
        {
            applicationsRegisterService.DeleteApplication(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.QualifiedFishersApplicationsRestoreRecords)]
        public IActionResult UndoDeleteApplication([FromQuery] int id)
        {
            applicationsRegisterService.UndoDeleteApplication(id);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.QualifiedFishersApplicationsReadAll, Permissions.QualifiedFishersApplicationsRead)]
        public IActionResult GetApplicationStatuses()
        {
            List<NomenclatureDTO> statuses = applicationsRegisterService.GetApplicationStatuses(ApplicationHierarchyTypesEnum.Online, ApplicationHierarchyTypesEnum.OnPaper);
            return Ok(statuses);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.QualifiedFishersApplicationsReadAll, Permissions.QualifiedFishersApplicationsRead)]
        public IActionResult GetApplicationTypes()
        {
            List<NomenclatureDTO> types = applicationsRegisterService.GetApplicationTypes(PageCodeEnum.QualiFi);
            return Ok(types);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.QualifiedFishersApplicationsReadAll, Permissions.QualifiedFishersApplicationsRead)]
        public IActionResult GetApplicationSources()
        {
            List<NomenclatureDTO> sources = applicationsRegisterService.GetApplicationSources(ApplicationHierarchyTypesEnum.Online, ApplicationHierarchyTypesEnum.OnPaper);
            return Ok(sources);
        }
    }
}
