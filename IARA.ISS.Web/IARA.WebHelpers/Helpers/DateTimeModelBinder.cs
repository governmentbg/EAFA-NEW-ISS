using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using IARA.Common.Constants;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IARA.WebHelpers
{
    public class DateTimeModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var modelName = GetModelName(bindingContext);

            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

            string dateString = valueProviderResult.FirstValue;

            // in case the date is passed as a query param
            // we need to decode it since the UTC format contains a plus (+) which is treated as a special symbol
            if (dateString.Contains('%'))
            {
                dateString = HttpUtility.UrlDecode(dateString);
            }

            if (string.IsNullOrEmpty(dateString))
            {
                return Task.CompletedTask;
            }

            DateTime result;

            if (DateTime.TryParseExact(dateString, DateTimeFormats.UTC_DATETIME_FORMAT, Thread.CurrentThread.CurrentCulture, DateTimeStyles.AssumeUniversal, out result))
            {
                bindingContext.Result = ModelBindingResult.Success(result);
            }
            else if (DateTime.TryParseExact(dateString, DateTimeFormats.ISO_DATETIME_FORMAT, Thread.CurrentThread.CurrentCulture, DateTimeStyles.AssumeUniversal, out result))
            {
                bindingContext.Result = ModelBindingResult.Success(result);
            }
            else if (DateTime.TryParseExact(dateString, DateTimeFormats.ISO_DATE_FORMAT, Thread.CurrentThread.CurrentCulture, DateTimeStyles.AssumeLocal, out result))
            {
                bindingContext.Result = ModelBindingResult.Success(result.Date);
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Failed();
            }

            return Task.CompletedTask;
        }

        private string GetModelName(ModelBindingContext bindingContext)
        {
            // The "Name" property of the ModelBinder attribute can be used to specify the
            // route parameter name when the action parameter name is different from the route parameter name.
            // For instance, when the route is /api/{birthDate} and the action parameter name is "date".
            // We can add this attribute with a Name property [DateTimeModelBinder(Name ="birthDate")]
            // Now bindingContext.BinderModelName will be "birthDate" and bindingContext.ModelName will be "date"
            if (!string.IsNullOrEmpty(bindingContext.BinderModelName))
            {
                return bindingContext.BinderModelName;
            }

            return bindingContext.ModelName;
        }
    }
}
