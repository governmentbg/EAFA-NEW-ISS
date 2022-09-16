using IARA.Mobile.Application.DTObjects.Common;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Controls.ViewModels;
using IARA.Mobile.Insp.Models;
using IARA.Mobile.Shared.ViewModels.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.Forms.Maps;

namespace IARA.Mobile.Insp.Helpers
{
    public static class InspectionEditHelper
    {
        public static void AssignFrom(this ValidStateValidatableTable<ToggleViewModel> toggles, List<InspectionCheckDto> checks)
        {
            if (checks?.Count > 0)
            {
                foreach (ToggleViewModel toggle in toggles)
                {
                    InspectionCheckDto check = checks.Find(f => f.CheckTypeId == toggle.CheckTypeId);

                    if (check != null)
                    {
                        toggle.Id = check.Id;
                        toggle.Description.Value = check.Description ?? string.Empty;
                        toggle.Value.Value = check.CheckValue.ToString();
                    }
                }
            }
        }

        public static void AssignFrom(this IValidState<string> validState, string value)
        {
            if (value != null)
            {
                validState.Value = value;
            }
        }

        public static void AssignFrom(this EgnLncValidState validState, EgnLncDto value)
        {
            if (value != null)
            {
                validState.Value = value.EgnLnc;
                validState.IdentifierType = value.IdentifierType;
            }
        }

        public static void AssignFrom(this IValidState<string> validState, decimal? value)
        {
            if (value.HasValue)
            {
                validState.Value = value.Value.ToString(CultureInfo.InvariantCulture);
            }
        }

        public static void AssignFrom(this IValidState<string> validState, int? value)
        {
            if (value.HasValue)
            {
                validState.Value = value.Value.ToString();
            }
        }

        public static void AssignFrom(this IValidState<bool> validState, bool? value)
        {
            if (value.HasValue)
            {
                validState.Value = value.Value;
            }
        }

        public static void AssignFrom(this IValidState<DateTime?> validState, DateTime? value)
        {
            if (value.HasValue)
            {
                validState.Value = value.Value;
            }
        }

        public static void AssignFrom(this IValidState<Position?> validState, LocationDto value)
        {
            if (value != null)
            {
                validState.Value = new Position(value.Latitude, value.Longitude);
            }
        }

        public static void AssignFrom<T>(this ValidStateSelect<T> validState, int? id, List<T> values)
            where T : SelectNomenclatureDto
        {
            if (id.HasValue && values != null)
            {
                validState.Value = values.Find(f => f.Id == id.Value);
            }
        }

        public static void AssignFrom<T>(this ValidStateSelect<T> validState, string code, List<T> values)
            where T : SelectNomenclatureDto
        {
            if (code != null && values != null)
            {
                validState.Value = values.Find(f => f.Code == code);
            }
        }

        public static void AssignFrom(this ValidStateObservation validState, List<InspectionObservationTextDto> values)
        {
            if (values?.Count > 0)
            {
                InspectionObservationTextDto find = values.Find(f => f.Category == validState.Category);

                if (find != null)
                {
                    validState.Id = find.Id;
                    validState.Value = find.Text;
                }
            }
        }

        public static string BuildAddress(this InspectionSubjectAddressDto address)
        {
            if (address == null)
            {
                return null;
            }

            const string group = nameof(GroupResourceEnum.Common);

            return string.Join(", ", new[]
            {
                address.PopulatedArea,
                address.Region,
                !string.IsNullOrWhiteSpace(address.Street) && !string.IsNullOrWhiteSpace(address.StreetNum)
                    ? (address.Street + " " + address.StreetNum)
                    : address.Street,
                !string.IsNullOrWhiteSpace(address.BlockNum)
                    ? (TranslateExtension.Translator[group + "/Block"] + " " + address.BlockNum)
                    : null,
                !string.IsNullOrWhiteSpace(address.EntranceNum)
                    ? (TranslateExtension.Translator[group + "/Entrance"] + " " + address.EntranceNum)
                    : null,
                !string.IsNullOrWhiteSpace(address.FloorNum)
                    ? (TranslateExtension.Translator[group + "/Floor"] + " " + address.FloorNum)
                    : null,
                !string.IsNullOrWhiteSpace(address.ApartmentNum)
                    ? (TranslateExtension.Translator[group + "/Apartment"] + " " + address.ApartmentNum)
                    : null,
            }.Where(f => !string.IsNullOrWhiteSpace(f)));
        }
    }
}
