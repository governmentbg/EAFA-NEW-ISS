using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IARA.DomainModels.DTOModels.Address;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.ControlActivity.Inspections;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.Nomenclatures;
using IARA.Infrastructure.Services;
using IARA.Interfaces.Nomenclatures;
using IARA.Security;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using Microsoft.AspNetCore.Mvc;
using TL.Caching.Interfaces;
using TL.Caching.Models;

namespace IARA.Web.Controllers
{
    [AreaRoute(AreaType.Nomenclatures)]
    public class NomenclaturesController : BaseController
    {
        private readonly ICommonNomenclaturesService nomenclaturesService;
        private readonly SystemPropertiesService systemPropertiesService;
        private readonly IMemoryCacheService memoryCacheService;

        public NomenclaturesController(ICommonNomenclaturesService nomenclaturesService,
                                       IPermissionsService permissionsService,
                                       SystemPropertiesService systemPropertiesService,
                                       IMemoryCacheService memoryCacheService)
            : base(permissionsService)
        {
            this.memoryCacheService = memoryCacheService;
            this.nomenclaturesService = nomenclaturesService;
            this.systemPropertiesService = systemPropertiesService;
        }

        [HttpGet]
        [CustomAuthorize]
        public IActionResult GetSystemProperties()
        {
            return this.Ok(this.systemPropertiesService.SystemProperties);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetCountries()
        {
            List<NomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetCountries), (service) => service.GetCountries());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetMuncipalities()
        {
            List<MunicipalityNomenclatureExtendedDTO> result = await this.GetCachedNomenclature(nameof(GetMuncipalities), (service) => service.GetMuncipalities());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetDistricts()
        {
            List<NomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetDistricts), (service) => service.GetDistricts());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetPopulatedAreas()
        {
            List<PopulatedAreaNomenclatureExtendedDTO> result = await this.GetCachedNomenclature(nameof(GetPopulatedAreas), (service) => service.GetPopulatedAreas());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetSectors()
        {
            List<NomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetSectors), (service) => service.GetSectors());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetDepartments()
        {
            List<NomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetDepartments), (service) => service.GetDepartments());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetTerritoryUnits()
        {
            List<NomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetTerritoryUnits), (service) => service.GetTerritoryUnits());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetDocumentTypes()
        {
            List<NomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetDocumentTypes), (service) => service.GetDocumentTypes());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetFishTypes()
        {
            List<FishNomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetFishTypes), (service) => service.GetFishes());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public IActionResult GetAddressNomenclatures()
        {
            AddressNomenclaturesDTO result = this.nomenclaturesService.GetAddressNomenclatures();
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetPermissions()
        {
            List<NomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetPermissions), (service) => service.GetPermissions());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public IActionResult GetUserNames()
        {
            List<NomenclatureDTO> result = this.nomenclaturesService.GetUserNames();
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetFileTypes()
        {
            List<NomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetFileTypes), (service) => service.GetFileTypes());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetOfflinePaymentTypes()
        {
            List<NomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetOfflinePaymentTypes), (service) => service.GetOfflinePaymentTypes());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetOnlinePaymentTypes()
        {
            List<NomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetOnlinePaymentTypes), (service) => service.GetOnlinePaymentTypes());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetGenders()
        {
            List<NomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetGenders), (service) => service.GetGenders());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetSubmittedByRoles()
        {
            List<SubmittedByRoleNomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetSubmittedByRoles), (service) => service.GetSubmittedByRoles());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetCancellationReasons()
        {
            List<CancellationReasonDTO> result = await this.GetCachedNomenclature(nameof(GetCancellationReasons), (service) => service.GetCancellationReasons());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetPermittedFileTypes()
        {
            List<PermittedFileTypeDTO> result = await this.GetCachedNomenclature(nameof(GetPermittedFileTypes), (service) => service.GetPermittedFileTypes());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetDeliveryTypes()
        {
            List<ApplicationDeliveryTypeDTO> result = await this.GetCachedNomenclature(nameof(GetDeliveryTypes), (service) => service.GetDeliveryTypes());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetChangeOfCircumstancesTypes()
        {
            List<ChangeOfCircumstancesTypeDTO> result = await this.GetCachedNomenclature(nameof(GetChangeOfCircumstancesTypes), (service) => service.GetChangeOfCircumstancesTypes());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetFishingGear()
        {
            List<FishingGearNomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetFishingGear), (service) => service.GetFishingGear());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetInstitutions()
        {
            List<NomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetInstitutions), (service) => service.GetInstitutions());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetVesselTypes()
        {
            List<NomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetVesselTypes), (service) => service.GetVesselTypes());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetPatrolVehicleTypes()
        {
            List<PatrolVehicleTypeNomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetPatrolVehicleTypes), (service) => service.GetPatrolVehicleTypes());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetCatchCheckTypes()
        {
            List<NomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetCatchCheckTypes), (service) => service.GetCatchCheckTypes());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetObservationTools()
        {
            List<InspectionObservationToolNomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetObservationTools), (service) => service.GetObservationTools());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetPorts()
        {
            List<NomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetPorts), (service) => service.GetPorts());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetVesselActivities()
        {
            List<InspectionVesselActivityNomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetVesselActivities), (service) => service.GetVesselActivities());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetUsageDocumentTypes()
        {
            List<NomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetUsageDocumentTypes), (service) => service.GetUsageDocumentTypes());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetCatchZones()
        {
            List<CatchZoneNomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetCatchZones), (service) => service.GetCatchZones());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetFishingGearMarkStatuses()
        {
            List<NomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetFishingGearMarkStatuses), (service) => service.GetFishingGearMarkStatuses());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetFishingGearPingerStatuses()
        {
            List<NomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetFishingGearPingerStatuses), (service) => service.GetFishingGearPingerStatuses());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetShips()
        {
            List<ShipNomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetShips), (service) => service.GetShips().ToList());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetLogBookTypes()
        {
            List<NomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetLogBookTypes), (service) => service.GetLogBookTypes());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetLogBookStatuses()
        {
            List<NomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetLogBookStatuses), (service) => service.GetLogBookStatuses());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetInspectionTypes()
        {
            List<NomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetInspectionTypes), (service) => service.GetInspectionTypes());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetPaymentTariffs()
        {
            List<TariffNomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetPaymentTariffs), (service) => service.GetPaymentTariffs());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetShipAssociations()
        {
            List<NomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetShipAssociations), (service) => service.GetShipAssociations());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetCatchInspectionTypes()
        {
            List<NomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetCatchInspectionTypes), (service) => service.GetCatchInspectionTypes());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetTransportVehicleTypes()
        {
            List<NomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetTransportVehicleTypes), (service) => service.GetTransportVehicleTypes());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetFishSex()
        {
            List<NomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetFishSex), (service) => service.GetFishSex());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetWaterBodyTypes()
        {
            List<NomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetWaterBodyTypes), (service) => service.GetWaterBodyTypes());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetCatchPresentations()
        {
            List<NomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetCatchPresentations), (service) => service.GetCatchPresentations());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetMarkReasons()
        {
            List<NomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetMarkReasons), (service) => service.GetMarkReasons());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetRemarkReasons()
        {
            List<NomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetRemarkReasons), (service) => service.GetRemarkReasons());
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> GetPoundNets()
        {
            List<NomenclatureDTO> result = await this.GetCachedNomenclature(nameof(GetPoundNets), (service) => service.GetPoundNets());
            return this.Ok(result);
        }

        private Task<List<NomenclatureType>> GetCachedNomenclature<NomenclatureType>(
            string key,
            Func<ICommonNomenclaturesService, List<NomenclatureType>> refreshAction
        )
            where NomenclatureType : NomenclatureDTO
        {
            CachingSettings<ICommonNomenclaturesService, List<NomenclatureType>> settings = new CachingSettings<ICommonNomenclaturesService, List<NomenclatureType>>(key, refreshAction)
            {
                MinutesToRefresh = 60
            };

            return this.memoryCacheService.GetCachedObject(settings);
        }
    }
}
