using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IARA.WebHelpers
{
    public class DateModelBinder : IModelBinder
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

            if (string.IsNullOrEmpty(dateString))
            {
                return Task.CompletedTask;
            }

            var dateTime = Date.Parse(dateString, Thread.CurrentThread.CurrentCulture, DateTimeStyles.AssumeLocal);

            bindingContext.Result = ModelBindingResult.Success(dateTime);

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
