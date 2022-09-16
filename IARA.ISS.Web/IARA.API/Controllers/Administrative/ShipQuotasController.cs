using System.Collections.Generic;
using System.IO;
using System.Linq;
using IARA.DomainModels.DTOModels.CatchQuotas;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.RequestModels;
using IARA.Excel.Tools.Models;
using IARA.Interfaces;
using IARA.Interfaces.Nomenclatures;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Enums;
using IARA.WebHelpers.Models;
using Microsoft.AspNetCore.Mvc;
using TL.Caching.Interfaces;

namespace IARA.Web.Controllers.Administrative
{
    [AreaRoute(AreaType.Administrative)]
    public class ShipQuotasController : BaseAuditController
    {
        private readonly IShipQuotasService service;
        private readonly IFileService fileService;
        private readonly IMemoryCacheService memoryCacheService;
        private readonly IYearlyQuotasService yearlyQuotasService;

        public ShipQuotasController(IShipQuotasService service,
                                    IPermissionsService permissionsService,
                                    IFileService fileService,
                                    IMemoryCacheService memoryCacheService,
                                    IYearlyQuotasService yearlyQuotasService)
            : base(permissionsService)
        {
            this.service = service;
            this.fileService = fileService;
            this.memoryCacheService = memoryCacheService;
            this.yearlyQuotasService = yearlyQuotasService;
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipQuotasRead)]
        public override IActionResult GetAuditInfo(int id)
        {
            return Ok(service.GetSimpleAudit(id));
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ShipQuotasAddRecords)]
        public IActionResult Add([FromBody] ShipQuotaEditDTO entry)
        {
            try
            {
                this.service.Add(entry);
                ForceRefreshShipsNomenclature();
                return Ok();
            }
            catch
            {
                return ValidationFailedResult(null, ErrorCode.AlreadySubmitted);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ShipQuotasRead)]
        public IActionResult GetAll([FromBody] GridRequestModel<ShipQuotasFilters> request)
        {
            var result = service.GetAll(request.Filters);

            return PageResult(result, request, false);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipQuotasRead)]
        public IActionResult Get([FromQuery] int id)
        {
            var result = service.Get(id);
            return Ok(result);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ShipQuotasRead)]
        public IActionResult GetChangeHistory([FromBody] IEnumerable<int> ids)
        {
            var result = service.GetHistoryForIds(ids);

            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipQuotasRead)]
        public IActionResult GetShipQuotasForList([FromQuery] int id)
        {
            var result = service.GetShipQuotasForList(id);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipQuotasRead)]
        public IActionResult GetYearlyQuotasForList()
        {
            var result = yearlyQuotasService.GetYearlyQuotasForList();
            return Ok(result);
        }

        [HttpPut]
        [CustomAuthorize(Permissions.ShipQuotasEditRecords)]
        public IActionResult Edit([FromBody] ShipQuotaEditDTO entry)
        {
            if (service.Edit(entry))
            {
                ForceRefreshShipsNomenclature();
            }

            return Ok();
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.ShipQuotasEditRecords)]
        public IActionResult Transfer([FromQuery] int newQuotaId, [FromQuery] int oldQuotaId, [FromQuery] int transferValue, [FromQuery] string basis)
        {
            service.Transfer(newQuotaId, oldQuotaId, transferValue, basis);
            return Ok();
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.ShipQuotasDeleteRecords)]
        public IActionResult Delete([FromQuery] int id)
        {
            service.Delete(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.ShipQuotasRestoreRecords)]
        public IActionResult Restore([FromQuery] int id)
        {
            service.Restore(id);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipQuotasRead)]
        public IActionResult DownloadFile(int id)
        {
            DownloadableFileDTO file = fileService.GetFileForDownload(id);
            return File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ShipQuotasRead)]
        public IActionResult DownloadShipQuotaExcel([FromBody] ExcelExporterRequestModel<ShipQuotasFilters> request)
        {
            Stream stream = service.DownloadShipQuotaExcel(request);
            return ExcelFile(stream, request.Filename);
        }

        private void ForceRefreshShipsNomenclature()
        {
            memoryCacheService.ForceRefresh<ICommonNomenclaturesService, List<ShipNomenclatureDTO>>("GetShips", (service) => service.GetShips().ToList());
        }
    }
}
