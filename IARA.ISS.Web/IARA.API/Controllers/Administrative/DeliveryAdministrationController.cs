using System.Collections.Generic;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Application;
using IARA.Interfaces;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Administrative
{
    [AreaRoute(AreaType.Administrative)]
    public class DeliveryAdministrationController : BaseAuditController
    {
        private readonly IDeliveryService service;

        public DeliveryAdministrationController(IPermissionsService permissionsService, IDeliveryService deliveryService)
            : base(permissionsService)
        {
            this.service = deliveryService;
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ApplicationRegisterDeliveryRead)]
        public IActionResult GetDeliveryData([FromQuery] int deliveryId)
        {
            ApplicationDeliveryDTO result = service.GetDeliveryData(deliveryId);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ApplicationRegisterDeliveryRead)]
        public override IActionResult GetAuditInfo(int id)
        {
            return Ok(service.GetSimpleAudit(id));
        }

        [HttpPut]
        [CustomAuthorize(Permissions.ApplicationRegisterDeliveryEditRecords)]
        public async Task<IActionResult> UpdateDeliveryData([FromQuery] int deliveryId, [FromQuery] bool sendEDelivery, [FromBody] ApplicationDeliveryDTO deliveryData)
        {
            bool hasEDelivery = await service.HasSubmittedForEDelivery(deliveryId, deliveryData.DeliveryTypeId);

            if (hasEDelivery == false)
            {
                return ValidationFailedResult(new List<string> { nameof(ApplicationValidationErrorsEnum.NoEDeliveryRegistration) });
            }

            int result = await service.UpdateDeliveryData(deliveryData, deliveryId, sendEDelivery);
            return Ok(result);
        }
    }
}
