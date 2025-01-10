using System;
using System.IO.Compression;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IARA.WebAPI.ActionFilters
{
    public class GzipCompressionAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var acceptEncoding = context.HttpContext.Request.Headers["Accept-Encoding"].ToString();

            if (acceptEncoding.Contains("gzip"))
            {
                var response = context.HttpContext.Response;
                response.Headers.Append("Content-Encoding", "gzip");
                response.Body = new GZipStream(response.Body, CompressionLevel.Fastest);
                await next();
            }
            else
            {
                await next();
            }
        }
    }
}
