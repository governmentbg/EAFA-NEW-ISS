using IARA.Mobile.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IARA.Mobile.Application.Extensions
{
    public static class HttpResultExtensions
    {
        public static Task<bool> IsSuccessfulResult(this Task<HttpResult> task)
        {
            return task.ContinueWith(f => f.Result.IsSuccessful);
        }

        public static Task<TDto> GetHttpContent<TDto>(this Task<HttpResult<TDto>> httpResult)
        {
            return httpResult.ContinueWith(task => task.Result.Content);
        }

        public static Task<HttpResult<List<TDto>>> GetGridRecords<TDto>(this Task<HttpResult<GridResult<TDto>>> httpResult)
        {
            return httpResult.ContinueWith(task => new HttpResult<List<TDto>>(task.Result.StatusCode, task.Result.Content?.Records));
        }
    }
}
