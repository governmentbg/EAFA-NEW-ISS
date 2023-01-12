using System;
using System.Collections.Generic;
using IARA.Mobile.Application.DTObjects.Common;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.Forms.Maps;

namespace IARA.Mobile.Insp.ViewModels.Models
{
    public class ValidStateLocation : ValidState<Position?>
    {
        public ValidStateLocation(List<TLValidator> validations, List<string> groups, IViewModelValidation validation)
            : base(validations, groups, validation) { }

        public static implicit operator Position(ValidStateLocation validState)
        {
            return validState.Value ?? throw new ArgumentException($"Cannot convert {nameof(ValidStateLocation)} to {nameof(Position)} because the Value is null.");
        }

        public static implicit operator LocationDto(ValidStateLocation validState)
        {
            Position? pos = validState.Value;

            return pos.HasValue ? new LocationDto
            {
                Latitude = pos.Value.Latitude,
                Longitude = pos.Value.Longitude
            } : null;
        }
    }
}
