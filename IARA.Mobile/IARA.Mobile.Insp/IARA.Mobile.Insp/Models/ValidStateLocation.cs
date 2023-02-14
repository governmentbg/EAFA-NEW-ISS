using System;
using System.Collections.Generic;
using IARA.Mobile.Application.DTObjects.Common;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.ViewModels.Models
{
    public class ValidStateLocation : ValidState<LocationDto>
    {
        public ValidStateLocation(List<TLValidator> validations, List<string> groups, IViewModelValidation validation)
            : base(validations, groups, validation) { }

        public static implicit operator LocationDto(ValidStateLocation validState)
        {
            return validState.Value ?? throw new ArgumentException($"Cannot convert {nameof(ValidStateLocation)} to {nameof(LocationDto)} because the Value is null.");
        }
    }
}
