using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Insp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class PermitsViewModel : ViewModel
    {
        public PermitsViewModel(InspectionPageViewModel inspection)
        {
            Inspection = inspection;

            Add = CommandBuilder.CreateFrom(OnAdd);
            Remove = CommandBuilder.CreateFrom<PermitModel>(OnRemove);

            this.AddValidation();
        }

        public InspectionPageViewModel Inspection { get; }

        public ValidStateValidatableTable<PermitModel> Permits { get; set; }

        public ICommand Add { get; }
        public ICommand Remove { get; }
        public ICommand ActionSelected { get; set; }

        public void OnEdit(List<InspectionPermitDto> permitLicenses)
        {
            if (permitLicenses == null || permitLicenses.Count == 0)
            {
                return;
            }

            Permits.Value.AddRange(permitLicenses.ConvertAll(f =>
            {
                PermitModel model = new PermitModel
                {
                    Dto = f,
                    AddedByInspector = f.PermitLicenseId == null
                };

                model.Corresponds.Value = f.CheckValue?.ToString();
                model.Number.AssignFrom(f.LicenseNumber ?? "NotAdded");
                model.Description.AssignFrom(f.Description);

                return model;
            }));
        }

        public void Reset()
        {
            Permits.Value.Clear();
        }

        private void OnAdd()
        {
            PermitModel model = new PermitModel
            {
                AddedByInspector = true,
                Dto = new InspectionPermitDto(),
            };
            model.Corresponds.Value = nameof(CheckTypeEnum.N);
            Permits.Value.Add(model);
        }

        private async Task OnRemove(PermitModel model)
        {
            bool result = await App.Current.MainPage.DisplayAlert(null,
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/DeleteMessage"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Yes"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/No"]
            );

            if (result)
            {
                Permits.Value.Remove(model);
            }
        }

        public static implicit operator List<InspectionPermitDto>(PermitsViewModel viewModel)
        {
            return viewModel == null
                ? new List<InspectionPermitDto>()
                : viewModel.Permits
                    .Select(f => new InspectionPermitDto
                    {
                        CheckValue = Enum.TryParse(f.Corresponds.Value, out CheckTypeEnum checkType)
                            ? (CheckTypeEnum?)checkType
                            : null,
                        Description = f.Description,
                        LicenseNumber = f.Number.Value == "NotAdded" ? null : f.Number.Value,
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
