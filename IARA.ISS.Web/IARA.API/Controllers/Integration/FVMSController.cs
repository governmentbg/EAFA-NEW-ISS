using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using IARA.FVMSModels;
using IARA.FVMSModels.Common;
using IARA.FVMSModels.CrossChecks;
using IARA.FVMSModels.ExternalModels;
using IARA.FVMSModels.GeoZones;
using IARA.FVMSModels.NISS;
using IARA.Infrastructure.FluxIntegrations.Interfaces;
using IARA.Interfaces.FVMSIntegrations;
using IARA.Logging.Abstractions.Interfaces;
using IARA.WebHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TL.SysToSysSecCom.Controller;
using TL.SysToSysSecCom.Interfaces;
using TL.SysToSysSecCom.Utils;

namespace IARA.WebAPI.Controllers.Integration
{
    /// <summary>
    /// Fishing Vessels Monitoring System
    /// </summary>
    [AreaRoute(AreaType.Integration)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces("application/json")]
    public class FVMSController : BaseSecureController
    {
        private IFVMSInitiatorIntegrationService service;
        private IExtendedLogger logger;
        private IFluxFvmsRequestsService fluxFvmsRequestsService;

        public FVMSController(ICryptoHelper cryptoHelper, 
                              IFVMSInitiatorIntegrationService service, 
                              IFluxFvmsRequestsService fluxFvmsRequestsService,
                              IExtendedLogger logger)
            : base(cryptoHelper)
        {
            this.service = service;
            this.fluxFvmsRequestsService = fluxFvmsRequestsService;
            this.logger = logger;
        }

        /// <summary>
        /// Заявка за данни за разрешителни за риболов по CFR номер на РК
        /// </summary>
        /// <param name="query"></param>
        /// <returns>Списък с разрешителни</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult PermitsByCFRQuery([FromSecureBody] NISSQuery query)
        {
            query.Type = NISSQueryType.PermByCFR;

            var permits = service.GetPermitsByCFR(query).Select(x => new ApiPerm(x)).ToList();

            if (permits != null && permits.Count > 0)
            {
                return Ok(permits);
            }
            else
            {
                return NoContent();
            }
        }

        /// <summary>
        /// Заявка за данни за разрешителни за риболов по уникален номер
        /// </summary>
        /// <param name="query"></param>
        /// <returns>Разрешително притежаващо списък с удостоверения и уреди към тях.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult PermitByNumberQuery([FromSecureBody] NISSQuery query)
        {
            query.Type = NISSQueryType.PermByNumber;
            var permit = new ApiPerm(service.GetPermit(query));

            if (permit != null)
            {
                return Ok(permit);
            }
            else
            {
                return NoContent();
            }
        }

        /// <summary>
        /// Заявка за данни за удостоверения за риболов по CFR номер на РК
        /// </summary>
        /// <param name="query"></param>
        /// <returns>Списък с удостоверения</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult LicensesByCFRQuery([FromSecureBody] NISSQuery query)
        {
            query.Type = NISSQueryType.LicenseByCFR;

            var licenses = service.GetLicenses(query).Select(x => new ApiCerts(x)).ToList();

            if (licenses != null && licenses.Count > 0)
            {
                return Ok(licenses);
            }
            else
            {
                return NoContent();
            }
        }

        /// <summary>
        /// Заявка за данни за удостоверения за риболов по номер на разрешително за риболов
        /// </summary>
        /// <param name="query"></param>
        /// <returns>Списък с удостоверения</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult LicensesByPermitNumber([FromSecureBody] NISSQuery query)
        {
            query.Type = NISSQueryType.LicenseByPermNumber;
            var licenses = service.GetLicenses(query).Select(x => new ApiCerts(x)).ToList();

            if (licenses != null && licenses.Count > 0)
            {
                return Ok(licenses);
            }
            else
            {
                return NoContent();
            }
        }

        /// <summary>
        /// Заявка за данни за удостоверения за риболов по номер на удостоверение за риболов
        /// </summary>
        /// <param name="query"></param>
        /// <returns>Удостоверение</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult LicenseByNumberQuery([FromSecureBody] NISSQuery query)
        {
            query.Type = NISSQueryType.LicenseByNumber;

            var licence = new ApiCerts(service.GetLicense(query));

            if (licence != null)
            {
                return Ok(licence);
            }
            else
            {
                return NoContent();
            }
        }

        /// <summary>
        /// Рапортуване на телеметрични данни за риболовни кораби в СНРК на определен времеви интервал и/или по време на възникване на събитиети на РК
        /// </summary>
        /// <param name="telemetry"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReportVesselTelemetryData([FromSecureBody] List<TelemetryStatus> telemetry)
        {
            logger.LogWarning("Опит за запис на телеметрични данни");
            bool result = await service.ReceiveTelemetryData(telemetry);

            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }


        /// <summary>
        /// Заявка за данни за риболовни уреди по номер на удостоверение за риболов
        /// </summary>
        /// <param name="query"></param>
        /// <returns>Списък с уреди</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult FishingGearsQuery([FromSecureBody] NISSQuery query)
        {
            var gears = service.GetFishingGears(query);

            if (gears != null && gears.Count > 0)
            {
                return Ok(gears);
            }
            else
            {
                return NoContent();
            }
        }


        /// <summary>
        /// Заявка за данни за географски зони
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult GeoZonesQuery([FromSecureBody] GeoZoneQuery query)
        {
            GeoZoneReport report = service.GetGeoZoneReport(query);

            if (report != null)
            {
                return Ok(report);
            }
            else
            {
                return NoContent();
            }
        }

        /// <summary>
        /// Получаване на данни за проведени кръстосани проверки
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCCheckReport([FromSecureBody] CCheckReport report)
        {
            bool result = await service.ReceiveCrossCheckReport(report);

            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Получаване на заявка за данни от извършени кръстосани проверки
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CrossCheckQuery([FromSecureBody] CCheckQuery query)
        {
            CCheckReport report = service.ReceiveCCheckQuery(query);
            return Ok(report);
        }

        [HttpPost]
        public IActionResult AddFluxRequest([FromSecureBody] FluxFvmsRequest request)
        {
            fluxFvmsRequestsService.AddFluxFvmsRequest(request);
            return Ok();
        }

        [HttpPost]
        public IActionResult AddFluxResponse([FromSecureBody] FluxFvmsResponse response)
        {
            fluxFvmsRequestsService.AddFluxFvmsResponse(response);
            return Ok();
        }
    }
}
