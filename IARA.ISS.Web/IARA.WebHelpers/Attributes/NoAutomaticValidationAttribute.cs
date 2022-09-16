using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IARA.WebHelpers.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class NoAutomaticValidationAttribute : ActionFilterAttribute, IFilterMetadata
    {
    }
}
