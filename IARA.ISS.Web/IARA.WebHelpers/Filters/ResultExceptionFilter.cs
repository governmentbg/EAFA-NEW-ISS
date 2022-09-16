using System.Collections.Generic;
using System.Threading;
using IARA.Common.Exceptions;
using IARA.Logging.Abstractions.Interfaces;
using IARA.WebHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IARA.WebFilters
{
    public class ResultExceptionFilter : IExceptionFilter
    {
        private readonly IExtendedLogger logger;

        public ResultExceptionFilter(IExtendedLogger logger)
        {
            this.logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            if (!context.ExceptionHandled)
            {
                context.ExceptionHandled = true;
                Thread.CurrentPrincipal = context.HttpContext.User;
                int id = logger.LogException(context.Exception, context.HttpContext.Request?.Path.ToString());

                if (context.Exception is ApplicationFileInvalidException afie)
                {
                    context.Result = new BadRequestObjectResult(new ErrorModel
                    {
                        Messages = afie.Errors.ConvertAll(f => f + $" ({id})"),
                        Type = ErrorType.Handled
                    });
                }
                else
                {
                    context.Result = new BadRequestObjectResult(new ErrorModel
                    {
                        Messages = new List<string> { context.Exception.Message + $" ({id})" },
                        Type = ErrorType.Unhandled
                    });
                }
            }
        }
    }
}
