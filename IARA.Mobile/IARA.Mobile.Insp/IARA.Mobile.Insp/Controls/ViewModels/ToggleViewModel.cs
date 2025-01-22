using System;
using System.ComponentModel.DataAnnotations;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class ToggleViewModel : ViewModel
    {
        public ToggleViewModel(bool isMandatory, bool hasDescription)
        {
            HasDescription = hasDescription;
            IsMandatory = isMandatory;

            this.AddValidation();

            if (isMandatory)
            {
                Value.HasAsterisk = true;
                Value.Validations.Add(new TLValidator(new RequiredAttribute(), nameof(RequiredAttribute)));
            }

            if (!hasDescription)
            {
                (Description as IValidState).ForceValidation = () =>
                {
                    Description.IsValid = true;
                    return null;
                };
            }
        }

        public int? Id { get; set; }
        public int CheckTypeId { get; set; }
        public ToggleTypeEnum Type { get; set; }
        public string Text { get; set; }
        public string DescriptionLabel { get; set; }

        public bool IsMandatory { get; }
        public bool HasDescription { get; }

        [MaxLength(100)]
        public ValidState Description { get; set; }

        public ValidStateMultiToggle Value { get; set; }

        public void Reset()
        {
            Description.Value = "";
            Value.Value = "";
        }

        public static implicit operator InspectionCheckDto(ToggleViewModel viewModel)
        {
            if (Enum.TryParse(viewModel.Value, out CheckTypeEnum checkType))
            {
                return new InspectionCheckDto
                {
                    Id = viewModel.Id,
                    CheckTypeId = viewModel.CheckTypeId,
                    CheckValue = checkType,
                    Description = viewModel.Description,
                };
            }
            else
            {
                return null;
            }
        }
    }
}
