using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Insp.Models;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class PermitLicensesViewModel : ViewModel
    {
        public PermitLicensesViewModel(InspectionPageViewModel inspection)
        {
            Inspection = inspection;

            Add = CommandBuilder.CreateFrom(OnAdd);
            Remove = CommandBuilder.CreateFrom<PermitLicenseModel>(OnRemove);

            this.AddValidation();
        }

        public InspectionPageViewModel Inspection { get; }

        public ValidStateValidatableTable<PermitLicenseModel> PermitLicenses { get; set; }

        public ICommand Add { get; }
        public ICommand Remove { get; }
        public ICommand ActionSelected { get; set; }

        public void OnEdit(List<InspectionPermitDto> permitLicenses)
        {
            if (permitLicenses == null || permitLicenses.Count == 0)
            {
                return;
            }

            PermitLicenses.Value.AddRange(permitLicenses.ConvertAll(f =>
            {
                PermitLicenseModel model = new PermitLicenseModel
                {
                    Dto = f,
                    AddedByInspector = f.PermitLicenseId == null
                };
                model.Corresponds.Value = f.CheckValue?.ToString();
                model.LicenseNumber.AssignFrom(f.LicenseNumber);
                model.Description.AssignFrom(f.Description);

                return model;
            }));
        }

        private void OnAdd()
        {
            PermitLicenseModel model = new PermitLicenseModel
            {
                AddedByInspector = true,
                Dto = new InspectionPermitDto(),
            };
            model.Corresponds.Value = nameof(CheckTypeEnum.N);
            PermitLicenses.Value.Add(model);
        }

        private async Task OnRemove(PermitLicenseModel model)
        {
            bool result = await App.Current.MainPage.DisplayAlert(null,
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/DeleteMessage"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Yes"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/No"]
            );

            if (result)
            {
                PermitLicenses.Value.Remove(model);
            }
        }

        public static implicit operator List<InspectionPermitDto>(PermitLicensesViewModel viewModel)
        {
            return viewModel == null
                ? new List<InspectionPermitDto>()
                : viewModel.PermitLicenses
                    .Select(f => new InspectionPermitDto
                    {
                        CheckValue = Enum.TryParse(f.Corresponds.Value, out CheckTypeEnum checkType)
                            ? (CheckTypeEnum?)checkType
                            : null,
                        Description = f.Description,
                        LicenseNumber = f.LicenseNumber,
                        From = f.Dto.From,
                        Id = f.Dto.Id,
                        PermitLicenseId = f.Dto.PermitLicenseId,
                        PermitNumber = f.Dto.PermitNumber,
                        To = f.Dto.To,
                        TypeId = f.Dto.TypeId,
                        TypeName = f.Dto.TypeName,
                    })
                    .ToList();
        }
    }
}
