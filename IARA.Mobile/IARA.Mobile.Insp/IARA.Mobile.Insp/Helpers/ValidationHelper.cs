using System.Collections.Generic;
using System.Linq;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Interfaces;

namespace IARA.Mobile.Insp.Helpers
{
    public static class ValidationHelper
    {
        public static void AddFakeValidation<T>(this IValidState<T> validState)
        {
            validState.ForceValidation = () =>
            {
                IReadOnlyList<string> errors = validState
                    .GetErrors("Validation")
                    .Select(f => f.ErrorMessage)
                    .ToList();

                validState.IsValid = errors.Count == 0;

                return errors.Count == 0 ? null : errors;
            };
        }
    }
}
