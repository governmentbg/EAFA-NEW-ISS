using System;
using IARA.Common.Enums;
using IARA.Interfaces;
using IARA.Security;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Mobile.Administrative
{
    [AreaRoute(AreaType.MobileAdministrative)]
    public class TranslationsController : BaseController
    {
        private readonly ITranslationService translationService;

        public TranslationsController(ITranslationService translationService, IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.translationService = translationService ?? throw new ArgumentNullException(nameof(translationService));
        }

        [HttpGet]
        [CustomAuthorize]
        public IActionResult GetAll()
        {
            return this.Ok(this.translationService.GetMobileTranslationsWhenUpdated(ResourceTranslationEnum.MOBILE_INSP));
        }
    }
}
