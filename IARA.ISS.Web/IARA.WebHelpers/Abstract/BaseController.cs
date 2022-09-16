using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using IARA.Common.GridModels;
using IARA.Common.Resources;
using IARA.Logging.Abstractions.Models;
using IARA.Security;
using IARA.Security.Models;
using IARA.WebHelpers.Attributes;
using IARA.WebHelpers.Enums;
using IARA.WebHelpers.Filters;
using IARA.WebHelpers.Models;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TL.SysToSysSecCom;
using TL.SysToSysSecCom.Interfaces;

namespace IARA.WebHelpers.Abstract
{
    public abstract class BaseController : ControllerBase, IActionFilter
    {
        protected const string IMPERSONATE_KEY = "Impersonate";

        protected UserSecurityModel CurrentUser { get; private set; }
        protected IPermissionsService permissionsService;

        public BaseController(IPermissionsService permissionsService)
        {
            this.permissionsService = permissionsService;
        }

        [NonAction]
        public void OnActionExecuting(ActionExecutingContext context)
        {
            bool? isAuthenticated = User?.Identity?.IsAuthenticated;

            if (isAuthenticated.HasValue && isAuthenticated.Value)
            {
                if (Request.Headers.ContainsKey(IMPERSONATE_KEY))
                {
                    ICryptoHelper helper = context.HttpContext.RequestServices.GetService(typeof(ICryptoHelper)) as ICryptoHelper;

                    string impersonationToken = Request.Headers[IMPERSONATE_KEY];

                    string payload = Encoding.UTF8.GetString(Convert.FromBase64String(impersonationToken));

                    SecurePayload securePayload = CommonUtils.JsonDeserialize<SecurePayload>(payload);

                    if (CommonUtils.TryUnwrapPayload(helper, securePayload, out ImpersonateSecurityModel securityModel) && securityModel.ValidTo > DateTime.UtcNow)
                    {
                        UserSecurityModel user = securityModel.User;

                        CurrentUser = new UserSecurityModel
                        {
                            ID = user.ID,
                            Username = user.Username,
                            LoginType = user.LoginType,
                            ClientId = user.ClientId,
                            Permissions = permissionsService.GetPermissionNamesByIds(user.Permissions.Select(x => int.Parse(x)).ToList())
                        };
                    }
                    else
                    {
                        context.HttpContext.Response.Cookies.Delete(IMPERSONATE_KEY);
                        FillCurrentUser();
                    }
                }
                else
                {
                    FillCurrentUser();
                }

                ClaimsIdentity claimsIdentity = User?.Identity as ClaimsIdentity;

                if (claimsIdentity != null)
                {
                    RequestData requestData = context.HttpContext.GetRequestContext();
                    claimsIdentity.AddIdentificationClaims(requestData);
                    Thread.CurrentPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claimsIdentity.Claims, claimsIdentity.AuthenticationType, JwtClaimTypes.Name, JwtClaimTypes.Role));
                    Request.HttpContext.User = Thread.CurrentPrincipal as ClaimsPrincipal;
                }
            }
            else
            {
                context.SetCurrentPrincipal(false, "ANONYMOUS");
            }

            if (!context.Filters.Any(x => x.GetType() == typeof(NoAutomaticValidationAttribute)) && !context.ModelState.IsValid)
            {
                context.Result = ValidationFailedResult();
            }
        }

        [NonAction]
        protected IActionResult PageResult<T>(IQueryable<T> query, BaseGridRequestModel request, bool applyDefaultSorting = true)
        {
            return Ok(new GridResultModel<T>(query, request, applyDefaultSorting));
        }

        [NonAction]
        protected IActionResult ExcelFile(Stream stream, string filename)
        {
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{filename}.xlsx");
        }

        [NonAction]
        protected IActionResult ValidationFailedResult(List<string> additionalErrors = null, ErrorCode? errorCode = null)
        {
            List<string> errors = new List<string>();

            if (additionalErrors != null)
            {
                errors.AddRange(additionalErrors);
            }

            foreach (KeyValuePair<string, ModelStateEntry> property in ModelState)
            {
                foreach (ModelError item in property.Value.Errors)
                {
                    if (!string.IsNullOrEmpty(item.ErrorMessage))
                    {
                        string value = string.Empty;
                        if (!string.IsNullOrEmpty(property.Value.AttemptedValue))
                        {
                            value = $"{ErrorResources.lblValue}:{property.Value.AttemptedValue}";
                        }

                        errors.Add($"{item.ErrorMessage} {value}");
                    }
                    else if (item.Exception != null)
                    {
                        string value = string.Empty;
                        if (!string.IsNullOrEmpty(property.Value.AttemptedValue))
                        {
                            value = $"{ErrorResources.lblValue}:{property.Value.AttemptedValue}";
                        }

                        errors.Add($"{item.Exception.Message} {value}");
                    }
                }
            }

            return UnprocessableEntity(new ErrorModel
            {
                Messages = errors,
                Type = ErrorType.Validation,
                Code = errorCode
            });
        }

        [NonAction]
        protected BadRequestObjectResult BadRequest(string message, ErrorType type = ErrorType.Handled, int? id = null)
        {
            return base.BadRequest(new ErrorModel
            {
                ID = id,
                Messages = new List<string> { message },
                Type = type
            });
        }

        [NonAction]
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        private void FillCurrentUser()
        {
            CurrentUser = new UserSecurityModel
            {
                ID = User.GetUserId().Value,
                Username = User.GetName(),
                LoginType = User.GetLoginType(),
                ClientId = User.GetClientId(),
                Permissions = permissionsService.GetPermissionNamesByIds(User.GetPermissionIds())
            };
        }
    }
}
