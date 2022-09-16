using IARA.Security;
using Microsoft.AspNetCore.Mvc;

namespace IARA.WebHelpers.Abstract
{
    public abstract class BaseAuditController : BaseController
    {
        public BaseAuditController(IPermissionsService permissionsService)
            : base(permissionsService)
        {
        }

        [HttpGet]
        public abstract IActionResult GetAuditInfo(int id);
    }
}
