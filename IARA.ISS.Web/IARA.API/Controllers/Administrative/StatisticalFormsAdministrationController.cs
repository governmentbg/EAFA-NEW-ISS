using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.ApplicationsRegister;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.StatisticalForms;
using IARA.DomainModels.DTOModels.StatisticalForms.AquaFarms;
using IARA.DomainModels.DTOModels.StatisticalForms.FishVessels;
using IARA.DomainModels.DTOModels.StatisticalForms.Reworks;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces;
using IARA.Interfaces.CommercialFishing;
using IARA.Interfaces.Nomenclatures;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Enums;
using IARA.WebHelpers.Models;
using Microsoft.AspNetCore.Mvc;

namespace IARA.WebAPI.Controllers.Administrative
{
    [AreaRoute(AreaType.Administrative)]
    public class StatisticalFormsAdministrationController : BaseAuditController
    {
        private static readonly PageCodeEnum[] PAGE_CODES = new PageCodeEnum[]
        {
            PageCodeEnum.StatFormAquaFarm,
            PageCodeEnum.StatFormRework,
            PageCodeEnum.StatFormFishVessel
        };

        private readonly IStatisticalFormsService service;
        private readonly IStatisticalFormsNomenclaturesService nomenclaturesService;
        private readonly IApplicationService applicationService;
        private readonly IApplicationsRegisterService applicationsRegisterService;
        private readonly IFileService fileService;
        private readonly IFishingGearsService fishingGearsService;
        private readonly IUserService userService;

        public StatisticalFormsAdministrationController(IStatisticalFormsService service,
                                                        IStatisticalFormsNomenclaturesService nomenclaturesService,
                                                        IApplicationService applicationService,
                                                        IApplicationsRegisterService applicationsRegisterService,
                                                        IFileService fileService,
                                                        IFishingGearsService fishingGearsService,
                                                        IPermissionsService permissionsService,
                                                        IUserService userService)
            : base(permissionsService)
        {
            this.service = service;
            this.nomenclaturesService = nomenclaturesService;
            this.applicationService = applicationService;
            this.applicationsRegisterService = applicationsRegisterService;
            this.fishingGearsService = fishingGearsService;
            this.fileService = fileService;
            this.userService = userService;
        }

        // Register
        [HttpPost]
        [CustomAuthorize(Permissions.StatisticalFormsAquaFarmReadAll,
                         Permissions.StatisticalFormsAquaFarmRead,
                         Permissions.StatisticalFormsReworkReadAll,
                         Permissions.StatisticalFormsReworkRead,
                         Permissions.StatisticalFormsFishVesselReadAll,
                         Permissions.StatisticalFormsFishVesselRead)]
        public IActionResult GetAllStatisticalForms([FromBody] GridRequestModel<StatisticalFormsFilters> request)
        {
            List<StatisticalFormTypesEnum> statisticalFormTypes = GetAllowedStatisticalFormTypes();

            if (!CurrentUser.Permissions.Contains(Permissions.StatisticalFormsAquaFarmApplicationsReadAll)
                    || !CurrentUser.Permissions.Contains(Permissions.StatisticalFormsReworkApplicationsReadAll)
                    || !CurrentUser.Permissions.Contains(Permissions.StatisticalFormsFishVesselsApplicationsReadAll))
            {
                int? territoryUnitId = userService.GetUserTerritoryUnitId(CurrentUser.ID);

                if (territoryUnitId.HasValue)
                {
                    if (request.Filters == null)
                    {
                        request.Filters = new StatisticalFormsFilters
                        {
                            ShowInactiveRecords = false
                        };
                    }

                    request.Filters.TerritoryUnitId = territoryUnitId.Value;

                    if (!CurrentUser.Permissions.Contains(Permissions.StatisticalFormsAquaFarmApplicationsReadAll))
                    {
                        request.Filters.FilterAquaFarmTerritoryUnit = true;
                    }

                    if (!CurrentUser.Permissions.Contains(Permissions.StatisticalFormsReworkApplicationsReadAll))
                    {
                        request.Filters.FilterReworkTerritoryUnit = true;
                    }

                    if (!CurrentUser.Permissions.Contains(Permissions.StatisticalFormsFishVesselsApplicationsReadAll))
                    {
                        request.Filters.FilterFishVesselTerritoryUnit = true;
                    }
                }
                else
                {
                    return Ok(Enumerable.Empty<ApplicationRegisterDTO>());
                }
            }

            IQueryable<StatisticalFormDTO> result = service.GetAllStatisticalForms(request.Filters, statisticalFormTypes, userId: null);
            return PageResult(result, request, false);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.StatisticalFormsAquaFarmReadAll, Permissions.StatisticalFormsAquaFarmRead)]
        public IActionResult GetStatisticalFormAquaFarm([FromQuery] int id)
        {
            StatisticalFormAquaFarmEditDTO form = service.GetStatisticalFormAquaFarm(id);
            return Ok(form);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.StatisticalFormsReworkReadAll, Permissions.StatisticalFormsReworkRead)]
        public IActionResult GetStatisticalFormRework([FromQuery] int id)
        {
            StatisticalFormReworkEditDTO form = service.GetStatisticalFormRework(id);
            return Ok(form);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.StatisticalFormsFishVesselReadAll, Permissions.StatisticalFormsFishVesselRead)]
        public IActionResult GetStatisticalFormFishVessel([FromQuery] int id)
        {
            StatisticalFormFishVesselEditDTO form = service.GetStatisticalFormFishVessel(id);
            return Ok(form);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.StatisticalFormsAquaFarmReadAll, Permissions.StatisticalFormsAquaFarmRead)]
        public IActionResult GetAquaFarmRegisterByApplicationId([FromQuery] int applicationId)
        {
            StatisticalFormAquaFarmEditDTO permit = service.GetAquaFarmRegisterByApplicationId(applicationId);
            return Ok(permit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.StatisticalFormsFishVesselReadAll, Permissions.StatisticalFormsFishVesselRead)]
        public IActionResult GetFishVesselRegisterByApplicationId([FromQuery] int applicationId)
        {
            StatisticalFormFishVesselEditDTO permit = service.GetFishVesselRegisterByApplicationId(applicationId);
            return Ok(permit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.StatisticalFormsAquaFarmReadAll, Permissions.StatisticalFormsAquaFarmRead)]
        public IActionResult GetReworkRegisterByApplicationId([FromQuery] int applicationId)
        {
            StatisticalFormReworkEditDTO permit = service.GetReworkRegisterByApplicationId(applicationId);
            return Ok(permit);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.StatisticalFormsAquaFarmAddRecords)]
        public IActionResult AddStatisticalFormAquaFarm([FromForm] StatisticalFormAquaFarmEditDTO form)
        {
            int id = service.AddStatisticalFormAquaFarm(form);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.StatisticalFormsReworkEditRecords)]
        public IActionResult AddStatisticalFormRework([FromForm] StatisticalFormReworkEditDTO form)
        {
            int id = service.AddStatisticalFormRework(form);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.StatisticalFormsFishVesselEditRecords)]
        public IActionResult AddStatisticalFormFishVessel([FromForm] StatisticalFormFishVesselEditDTO form)
        {
            int id = service.AddStatisticalFormFishVessel(form);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.StatisticalFormsAquaFarmEditRecords)]
        public IActionResult EditStatisticalFormAquaFarm([FromForm] StatisticalFormAquaFarmEditDTO form)
        {
            service.EditStatisticalFormAquaFarm(form);
            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.StatisticalFormsReworkEditRecords)]
        public IActionResult EditStatisticalFormRework([FromForm] StatisticalFormReworkEditDTO form)
        {
            service.EditStatisticalFormRework(form);
            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.StatisticalFormsFishVesselEditRecords)]
        public IActionResult EditStatisticalFormFishVessel([FromForm] StatisticalFormFishVesselEditDTO form)
        {
            service.EditStatisticalFormFishVessel(form);
            return Ok();
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.StatisticalFormsAquaFarmDeleteRecords,
                         Permissions.StatisticalFormsReworkDeleteRecords,
                         Permissions.StatisticalFormsFishVesselDeleteRecords)]
        public IActionResult DeleteStatisticalForm([FromQuery] int id)
        {
            service.DeleteStatisticalForm(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.StatisticalFormsAquaFarmRestoreRecords,
                         Permissions.StatisticalFormsReworkRestoreRecords,
                         Permissions.StatisticalFormsFishVesselRestoreRecords)]
        public IActionResult UndoDeleteStatisticalForm([FromQuery] int id)
        {
            service.UndoDeleteStatisticalForm(id);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.StatisticalFormsAquaFarmReadAll,
                         Permissions.StatisticalFormsAquaFarmRead,
                         Permissions.StatisticalFormsReworkReadAll,
                         Permissions.StatisticalFormsReworkRead,
                         Permissions.StatisticalFormsFishVesselReadAll,
                         Permissions.StatisticalFormsFishVesselRead)]
        public IActionResult DownloadFile([FromQuery] int id)
        {
            DownloadableFileDTO file = fileService.GetFileForDownload(id);
            return File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.StatisticalFormsAquaFarmReadAll,
                         Permissions.StatisticalFormsAquaFarmRead,
                         Permissions.StatisticalFormsReworkReadAll,
                         Permissions.StatisticalFormsReworkRead,
                         Permissions.StatisticalFormsFishVesselReadAll,
                         Permissions.StatisticalFormsFishVesselRead)]
        public override IActionResult GetAuditInfo([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetSimpleAudit(id);
            return Ok(audit);
        }

        // Applications
        [HttpPost]
        [CustomAuthorize(Permissions.StatisticalFormsAquaFarmApplicationsReadAll,
                         Permissions.StatisticalFormsAquaFarmApplicationsRead,
                         Permissions.StatisticalFormsReworkApplicationsReadAll,
                         Permissions.StatisticalFormsReworkApplicationsRead,
                         Permissions.StatisticalFormsFishVesselsApplicationsReadAll,
                         Permissions.StatisticalFormsFishVesselsApplicationsRead)]
        public IActionResult GetAllApplications([FromBody] GridRequestModel<ApplicationsRegisterFilters> request)
        {
            List<PageCodeEnum> permittedPageCodesRead = null;
            if (!CurrentUser.Permissions.Contains(Permissions.StatisticalFormsAquaFarmApplicationsReadAll)
                    || !CurrentUser.Permissions.Contains(Permissions.StatisticalFormsReworkApplicationsReadAll)
                    || !CurrentUser.Permissions.Contains(Permissions.StatisticalFormsFishVesselsApplicationsReadAll))
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
                    permittedPageCodesRead = new List<PageCodeEnum>();

                    if (!CurrentUser.Permissions.Contains(Permissions.StatisticalFormsAquaFarmApplicationsReadAll)
                        && CurrentUser.Permissions.Contains(Permissions.StatisticalFormsAquaFarmApplicationsRead))
                    {
                        permittedPageCodesRead.Add(PageCodeEnum.StatFormAquaFarm);
                    }

                    if (!CurrentUser.Permissions.Contains(Permissions.StatisticalFormsReworkApplicationsReadAll)
                        && CurrentUser.Permissions.Contains(Permissions.StatisticalFormsReworkApplicationsRead))
                    {
                        permittedPageCodesRead.Add(PageCodeEnum.StatFormRework);
                    }

                    if (!CurrentUser.Permissions.Contains(Permissions.StatisticalFormsFishVesselsApplicationsReadAll)
                        && CurrentUser.Permissions.Contains(Permissions.StatisticalFormsFishVesselsApplicationsRead))
                    {
                        permittedPageCodesRead.Add(PageCodeEnum.StatFormFishVessel);
                    }
                }
                else
                {
                    return Ok(Enumerable.Empty<ApplicationRegisterDTO>());
                }
            }

            PageCodeEnum[] permittedPageCodesReadAll = GetAllowedPageCodesForRead().ToArray();

            IQueryable<ApplicationRegisterDTO> permits = applicationsRegisterService.GetAllApplications(request.Filters, null, permittedPageCodesReadAll, permittedPageCodesRead);
            return PageResult(permits, request, false);
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.StatisticalFormsAquaFarmApplicationsDeleteRecords,
                         Permissions.StatisticalFormsReworkApplicationsDeleteRecords,
                         Permissions.StatisticalFormsFishVesselsApplicationsDeleteRecords)]
        public IActionResult DeleteApplication([FromQuery] int id)
        {
            applicationsRegisterService.DeleteApplication(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.StatisticalFormsAquaFarmApplicationsRestoreRecords,
                         Permissions.StatisticalFormsReworkApplicationsRestoreRecords,
                         Permissions.StatisticalFormsFishVesselsApplicationsRestoreRecords)]
        public IActionResult UndoDeleteApplication([FromQuery] int id)
        {
            applicationsRegisterService.UndoDeleteApplication(id);
            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.StatisticalFormsAquaFarmApplicationsEditRecords,
                         Permissions.StatisticalFormsReworkApplicationsEditRecords,
                         Permissions.StatisticalFormsFishVesselsApplicationsEditRecords)]
        public IActionResult AssignApplicationViaAccessCode([FromQuery] string accessCode)
        {
            try
            {
                AssignedApplicationInfoDTO applicationData = applicationService.AssignApplicationViaAccessCode(accessCode, CurrentUser.ID, GetAllowedPageCodesForEdit().ToArray());
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
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsReadAll, Permissions.AquacultureFacilitiesApplicationsRead)]
        public IActionResult GetApplicationStatuses()
        {
            List<NomenclatureDTO> statuses = applicationsRegisterService.GetApplicationStatuses(ApplicationHierarchyTypesEnum.Online, ApplicationHierarchyTypesEnum.OnPaper);
            return Ok(statuses);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsReadAll, Permissions.AquacultureFacilitiesApplicationsRead)]
        public IActionResult GetApplicationTypes()
        {
            List<NomenclatureDTO> types = applicationsRegisterService.GetApplicationTypes(PAGE_CODES);
            return Ok(types);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsReadAll, Permissions.AquacultureFacilitiesApplicationsRead)]
        public IActionResult GetApplicationSources()
        {
            List<NomenclatureDTO> sources = applicationsRegisterService.GetApplicationSources(ApplicationHierarchyTypesEnum.Online, ApplicationHierarchyTypesEnum.OnPaper);
            return Ok(sources);
        }

        // Aquaculture farms
        [HttpGet]
        [CustomAuthorize(Permissions.StatisticalFormsAquaFarmApplicationsReadAll, Permissions.StatisticalFormsAquaFarmApplicationsRead)]
        public IActionResult GetStatisticalFormAquaFarmApplication([FromQuery] int id)
        {
            StatisticalFormAquaFarmApplicationEditDTO form = service.GetStatisticalFormAquaFarmApplication(id);
            return Ok(form);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.StatisticalFormsAquaFarmApplicationsInspectAndCorrectRegiXData)]
        public IActionResult GetStatisticalFormAquaFarmRegixData([FromQuery] int applicationId)
        {
            RegixChecksWrapperDTO<StatisticalFormAquaFarmRegixDataDTO> result = service.GetStatisticalFormAquaFarmRegixData(applicationId);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.StatisticalFormsAquaFarmReadAll, Permissions.StatisticalFormsAquaFarmRead)]
        public IActionResult GetApplicationAquaFarmDataForRegister([FromQuery] int applicationId)
        {
            StatisticalFormAquaFarmEditDTO form = service.GetApplicationAquaFarmDataForRegister(applicationId);
            return Ok(form);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.StatisticalFormsAquaFarmApplicationsAddRecords)]
        public IActionResult AddStatisticalFormAquaFarmApplication([FromForm] StatisticalFormAquaFarmApplicationEditDTO form)
        {
            int id = service.AddStatisticalFormAquaFarmApplication(form, ApplicationStatusesEnum.EXT_CHK_STARTED);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.StatisticalFormsAquaFarmApplicationsEditRecords)]
        public IActionResult EditStatisticalFormAquaFarmApplication([FromQuery] bool saveAsDraft, [FromForm] StatisticalFormAquaFarmApplicationEditDTO form)
        {
            if (saveAsDraft)
            {
                service.EditStatisticalFormAquaFarmApplication(form);
            }
            else
            {
                service.EditStatisticalFormAquaFarmApplication(form, ApplicationStatusesEnum.EXT_CHK_STARTED);
            }

            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.StatisticalFormsAquaFarmApplicationsEditRecords)]
        public IActionResult EditStatisticalFormAquaFarmApplicationRegixData([FromForm] StatisticalFormAquaFarmRegixDataDTO form)
        {
            service.EditStatisticalFormAquaFarmApplicationRegixData(form);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.StatisticalFormsAquaFarmApplicationsReadAll,
                         Permissions.StatisticalFormsAquaFarmApplicationsRead,
                         Permissions.StatisticalFormsAquaFarmReadAll,
                         Permissions.StatisticalFormsAquaFarmRead)]
        public IActionResult GetStatisticalFormAquaculture([FromQuery] int aquacultureId)
        {
            StatisticalFormAquacultureDTO result = service.GetStatisticalFormAquaculture(aquacultureId);
            return Ok(result);
        }

        // Reworks
        [HttpGet]
        [CustomAuthorize(Permissions.StatisticalFormsReworkApplicationsAddRecords)]
        public IActionResult GetStatisticalFormReworkApplication([FromQuery] int id)
        {
            StatisticalFormReworkApplicationEditDTO form = service.GetStatisticalFormReworkApplication(id);
            return Ok(form);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.StatisticalFormsReworkApplicationsInspectAndCorrectRegiXData)]
        public IActionResult GetStatisticalFormReworkRegixData([FromQuery] int applicationId)
        {
            RegixChecksWrapperDTO<StatisticalFormReworkRegixDataDTO> result = service.GetStatisticalFormReworkRegixData(applicationId);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.StatisticalFormsReworkReadAll, Permissions.StatisticalFormsReworkRead)]
        public IActionResult GetApplicationReworkDataForRegister([FromQuery] int applicationId)
        {
            StatisticalFormReworkEditDTO form = service.GetApplicationReworkDataForRegister(applicationId);
            return Ok(form);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.StatisticalFormsReworkApplicationsAddRecords)]
        public IActionResult AddStatisticalFormReworkApplication([FromForm] StatisticalFormReworkApplicationEditDTO form)
        {
            int id = service.AddStatisticalFormReworkApplication(form, ApplicationStatusesEnum.EXT_CHK_STARTED);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.StatisticalFormsReworkApplicationsEditRecords)]
        public IActionResult EditStatisticalFormReworkApplication([FromQuery] bool saveAsDraft, [FromForm] StatisticalFormReworkApplicationEditDTO form)
        {
            if (saveAsDraft)
            {
                service.EditStatisticalFormReworkApplication(form);
            }
            else
            {
                service.EditStatisticalFormReworkApplication(form, ApplicationStatusesEnum.EXT_CHK_STARTED);
            }

            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.StatisticalFormsReworkApplicationsEditRecords)]
        public IActionResult EditStatisticalFormReworkApplicationRegixData([FromForm] StatisticalFormReworkRegixDataDTO form)
        {
            service.EditStatisticalFormReworkApplicationRegixData(form);
            return Ok();
        }

        // Fishing vessel
        [HttpGet]
        [CustomAuthorize(Permissions.StatisticalFormsFishVesselsApplicationsReadAll, Permissions.StatisticalFormsFishVesselsApplicationsRead)]
        public IActionResult GetStatisticalFormFishVesselApplication([FromQuery] int id)
        {
            StatisticalFormFishVesselApplicationEditDTO form = service.GetStatisticalFormFishVesselApplication(id);
            return Ok(form);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.StatisticalFormsFishVesselsApplicationsInspectAndCorrectRegiXData)]
        public IActionResult GetStatisticalFormFishVesselRegixData([FromQuery] int applicationId)
        {
            RegixChecksWrapperDTO<StatisticalFormFishVesselRegixDataDTO> result = service.GetStatisticalFormFishVesselRegixData(applicationId);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.StatisticalFormsFishVesselReadAll, Permissions.StatisticalFormsFishVesselRead)]
        public IActionResult GetApplicationFishVesselDataForRegister([FromQuery] int applicationId)
        {
            StatisticalFormFishVesselEditDTO form = service.GetApplicationFishVesselDataForRegister(applicationId);
            return Ok(form);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.StatisticalFormsFishVesselsApplicationsAddRecords)]
        public IActionResult AddStatisticalFormFishVesselApplication([FromForm] StatisticalFormFishVesselApplicationEditDTO form)
        {
            int id = service.AddStatisticalFormFishVesselApplication(form, ApplicationStatusesEnum.EXT_CHK_STARTED);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.StatisticalFormsFishVesselsApplicationsEditRecords)]
        public IActionResult EditStatisticalFormFishVesselApplication([FromQuery] bool saveAsDraft, [FromForm] StatisticalFormFishVesselApplicationEditDTO form)
        {
            if (saveAsDraft)
            {
                service.EditStatisticalFormFishVesselApplication(form);
            }
            else
            {
                service.EditStatisticalFormFishVesselApplication(form, ApplicationStatusesEnum.EXT_CHK_STARTED);
            }

            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.StatisticalFormsFishVesselsApplicationsEditRecords)]
        public IActionResult EditStatisticalFormFishVesselApplicationRegixData([FromForm] StatisticalFormFishVesselRegixDataDTO form)
        {
            service.EditStatisticalFormFishVesselApplicationRegixData(form);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.StatisticalFormsFishVesselsApplicationsReadAll,
                         Permissions.StatisticalFormsFishVesselsApplicationsRead,
                         Permissions.StatisticalFormsFishVesselReadAll,
                         Permissions.StatisticalFormsFishVesselRead)]
        public IActionResult GetStatisticalFormShip([FromQuery] int shipId)
        {
            StatisticalFormShipDTO result = service.GetStatisticalFormShip(shipId);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.StatisticalFormsFishVesselsApplicationsReadAll,
                         Permissions.StatisticalFormsFishVesselsApplicationsRead,
                         Permissions.StatisticalFormsFishVesselReadAll,
                         Permissions.StatisticalFormsFishVesselRead)]
        public IActionResult GetShipFishingGearsForYear([FromQuery] int shipId, [FromQuery] int year)
        {
            List<NomenclatureDTO> result = fishingGearsService.GetShipFishingGearNomenclatures(shipId, year);
            return Ok(result);
        }

        //Nomenclatures
        [HttpGet]
        [CustomAuthorize(Permissions.StatisticalFormsFishVesselsApplicationsReadAll,
                         Permissions.StatisticalFormsFishVesselsApplicationsRead,
                         Permissions.StatisticalFormsFishVesselReadAll,
                         Permissions.StatisticalFormsFishVesselRead)]
        public IActionResult GetGrossTonnageIntervals()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetGrossTonnageIntervals();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.StatisticalFormsFishVesselsApplicationsReadAll,
                         Permissions.StatisticalFormsFishVesselsApplicationsRead,
                         Permissions.StatisticalFormsFishVesselReadAll,
                         Permissions.StatisticalFormsFishVesselRead)]
        public IActionResult GetVesselLengthIntervals()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetVesselLengthIntervals();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.StatisticalFormsFishVesselsApplicationsReadAll,
                         Permissions.StatisticalFormsFishVesselsApplicationsRead,
                         Permissions.StatisticalFormsFishVesselReadAll,
                         Permissions.StatisticalFormsFishVesselRead)]
        public IActionResult GetFuelTypes()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetFuelTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.StatisticalFormsReworkApplicationsReadAll,
                         Permissions.StatisticalFormsReworkApplicationsRead,
                         Permissions.StatisticalFormsReworkReadAll,
                         Permissions.StatisticalFormsReworkRead)]
        public IActionResult GetReworkProductTypes()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetReworkProductTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.StatisticalFormsAquaFarmApplicationsReadAll,
                         Permissions.StatisticalFormsAquaFarmApplicationsRead,
                         Permissions.StatisticalFormsAquaFarmReadAll,
                         Permissions.StatisticalFormsAquaFarmRead)]
        public IActionResult GetAllAquacultureNomenclatures()
        {
            List<StatisticalFormAquacultureNomenclatureDTO> result = nomenclaturesService.GetAllAquacultureNomenclatures();
            return Ok(result);
        }


        private List<StatisticalFormTypesEnum> GetAllowedStatisticalFormTypes()
        {
            List<StatisticalFormTypesEnum> statisticalFormTypes = new();

            if (CurrentUser.Permissions.Contains(Permissions.StatisticalFormsAquaFarmReadAll) || CurrentUser.Permissions.Contains(Permissions.StatisticalFormsAquaFarmRead))
            {
                statisticalFormTypes.Add(StatisticalFormTypesEnum.AquaFarm);
            }

            if (CurrentUser.Permissions.Contains(Permissions.StatisticalFormsReworkReadAll) || CurrentUser.Permissions.Contains(Permissions.StatisticalFormsReworkRead))
            {
                statisticalFormTypes.Add(StatisticalFormTypesEnum.Rework);
            }

            if (CurrentUser.Permissions.Contains(Permissions.StatisticalFormsFishVesselReadAll) || CurrentUser.Permissions.Contains(Permissions.StatisticalFormsFishVesselRead))
            {
                statisticalFormTypes.Add(StatisticalFormTypesEnum.FishVessel);
            }

            return statisticalFormTypes;
        }

        private List<PageCodeEnum> GetAllowedPageCodesForEdit()
        {
            List<PageCodeEnum> pageCodes = new();

            if (CurrentUser.Permissions.Contains(Permissions.StatisticalFormsAquaFarmReadAll) || CurrentUser.Permissions.Contains(Permissions.StatisticalFormsAquaFarmRead))
            {
                pageCodes.Add(PageCodeEnum.StatFormAquaFarm);
            }

            if (CurrentUser.Permissions.Contains(Permissions.StatisticalFormsReworkReadAll) || CurrentUser.Permissions.Contains(Permissions.StatisticalFormsReworkRead))
            {
                pageCodes.Add(PageCodeEnum.StatFormRework);
            }

            if (CurrentUser.Permissions.Contains(Permissions.StatisticalFormsFishVesselReadAll) || CurrentUser.Permissions.Contains(Permissions.StatisticalFormsFishVesselRead))
            {
                pageCodes.Add(PageCodeEnum.StatFormFishVessel);
            }

            return pageCodes;
        }

        private List<PageCodeEnum> GetAllowedPageCodesForRead()
        {
            List<PageCodeEnum> pageCodes = new();

            if (CurrentUser.Permissions.Contains(Permissions.StatisticalFormsAquaFarmEditRecords))
            {
                pageCodes.Add(PageCodeEnum.StatFormAquaFarm);
            }

            if (CurrentUser.Permissions.Contains(Permissions.StatisticalFormsReworkApplicationsEditRecords))
            {
                pageCodes.Add(PageCodeEnum.StatFormRework);
            }

            if (CurrentUser.Permissions.Contains(Permissions.StatisticalFormsFishVesselsApplicationsEditRecords))
            {
                pageCodes.Add(PageCodeEnum.StatFormFishVessel);
            }

            return pageCodes;
        }
    }
}
