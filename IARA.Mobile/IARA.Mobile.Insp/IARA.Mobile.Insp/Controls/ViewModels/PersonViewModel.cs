using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Shared.Attributes;
using IARA.Mobile.Shared.ViewModels.Models;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class PersonViewModel : ViewModel
    {
        private List<SelectNomenclatureDto> _nationalities;

        public PersonViewModel(InspectionPageViewModel inspection, InspectedPersonType personType)
        {
            Inspection = inspection;
            PersonType = personType;

            this.AddValidation();
        }

        public int? Id { get; set; }

        public InspectionPageViewModel Inspection { get; }
        public InspectedPersonType PersonType { get; }

        [Required]
        [MaxLength(200)]
        public ValidState FirstName { get; set; }

        [MaxLength(200)]
        public ValidState MiddleName { get; set; }

        [Required]
        [MaxLength(200)]
        public ValidState LastName { get; set; }

        [Required]
        [MaxLength(20)]
        [EGN(nameof(EGN))]
        public EgnLncValidState EGN { get; set; }

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
            if (personnel == null)
            {
                return;
            }

            InspectionSubjectPersonnelDto person = personnel.Find(f => f.Type == PersonType);
            OnEdit(person);
        }

        public void OnEdit(InspectionSubjectPersonnelDto subject)
        {
            if (subject != null)
            {
                Id = subject.Id;
                EGN.IdentifierType = subject.EgnLnc?.IdentifierType ?? IdentifierTypeEnum.EGN;

                FirstName.AssignFrom(subject.FirstName);
                MiddleName.AssignFrom(subject.MiddleName);
                LastName.AssignFrom(subject.LastName);
                EGN.AssignFrom(subject.EgnLnc);
                Address.AssignFrom(subject.Address);
                Nationality.AssignFrom(subject.CitizenshipId, Nationalities);
            }
        }

        public static implicit operator InspectionSubjectPersonnelDto(PersonViewModel viewModel)
        {
            return new InspectionSubjectPersonnelDto
            {
                Id = viewModel.Id,
                Type = viewModel.PersonType,
                Address = viewModel.Address,
                CitizenshipId = viewModel.Nationality.Value,
                FirstName = viewModel.FirstName,
                MiddleName = viewModel.MiddleName,
                LastName = viewModel.LastName,
                EgnLnc = viewModel.EGN,
                IsLegal = false,
            };
        }
    }
}
