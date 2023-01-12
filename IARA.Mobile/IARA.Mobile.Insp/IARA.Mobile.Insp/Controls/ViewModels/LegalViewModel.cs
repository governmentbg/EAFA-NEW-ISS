using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Shared.ViewModels.Models;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class LegalViewModel : ViewModel
    {
        private List<SelectNomenclatureDto> _nationalities;
        private bool _isEnabled = true;

        public LegalViewModel(InspectionPageViewModel inspection, InspectedPersonType personType)
        {
            Inspection = inspection;
            PersonType = personType;

            this.AddValidation();
        }

        public int? Id { get; set; }

        public InspectionPageViewModel Inspection { get; }
        public InspectedPersonType PersonType { get; }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        [Required]
        [MaxLength(200)]
        public ValidState Name { get; set; }

        [Required]
        [MaxLength(20)]
        public EgnLncValidState EIK { get; set; }

        [Required]
        [MaxLength(500)]
        public ValidState Address { get; set; }

        [Required]
        public ValidStateSelect<SelectNomenclatureDto> Nationality { get; set; }

        public List<SelectNomenclatureDto> Nationalities
        {
            get => _nationalities;
            private set => SetProperty(ref _nationalities, value);
        }

        public void Init(List<SelectNomenclatureDto> nationalities)
        {
            Nationalities = nationalities;

            Nationality.Value = nationalities.Find(f => f.Code == CommonConstants.NomenclatureBulgaria);
        }

        public void OnEdit(List<InspectionSubjectPersonnelDto> personnel)
        {
            InspectionSubjectPersonnelDto subject = personnel?.Find(f => f.Type == PersonType);

            OnEdit(subject);
        }

        public void OnEdit(InspectionSubjectPersonnelDto subject)
        {
            if (subject != null)
            {
                Id = subject.Id;

                Name.AssignFrom(subject.FirstName);
                Address.AssignFrom(subject.Address);
                Nationality.AssignFrom(subject.CitizenshipId, Nationalities);
                EIK.AssignFrom(subject.Eik);
            }
        }

        public static implicit operator InspectionSubjectPersonnelDto(LegalViewModel viewModel)
        {
            return new InspectionSubjectPersonnelDto
            {
                Id = viewModel.Id,
                Type = viewModel.PersonType,
                Address = viewModel.Address,
                CitizenshipId = viewModel.Nationality.Value,
                FirstName = viewModel.Name,
                Eik = viewModel.EIK,
                IsLegal = true,
            };
        }
    }
}
