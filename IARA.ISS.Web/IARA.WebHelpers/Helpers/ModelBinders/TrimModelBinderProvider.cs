using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace IARA.WebHelpers.Helpers
{
    public class TrimModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(string))
            {
                return new BinderTypeModelBinder(typeof(TrimModelBinder));
            }
            else
            {
                return null;
            }
        }
    }
}
