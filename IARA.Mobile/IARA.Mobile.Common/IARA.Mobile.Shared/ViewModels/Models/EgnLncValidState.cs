using System.Collections.Generic;
using IARA.Mobile.Application.DTObjects.Common;
using IARA.Mobile.Domain.Enums;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Shared.ViewModels.Models
{
    public class EgnLncValidState : ValidState<string>
    {
        private IdentifierTypeEnum _identifierType;

        public EgnLncValidState(List<TLValidator> validations, List<string> groups, IViewModelValidation validation)
            : base(validations, groups, validation)
        {
            Value = string.Empty;
        }

        public IdentifierTypeEnum IdentifierType
        {
            get => _identifierType;
            set
            {
                if (SetProperty(ref _identifierType, value))
                {
                    (this as IValidState).ForceValidation();
                }
            }
        }

        public static implicit operator EgnLncDto(EgnLncValidState state)
        {
            if (string.IsNullOrEmpty(state.Value))
            {
                return null;
            }

            return new EgnLncDto
            {
                EgnLnc = state.Value,
                IdentifierType = state.IdentifierType
            };
        }
    }
}
