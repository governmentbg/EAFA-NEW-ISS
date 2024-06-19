using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Common;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Shared.Attributes;
using IARA.Mobile.Shared.ViewModels.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class PersonViewModel : ViewModel
    {
        private List<SelectNomenclatureDto> _nationalities;

        public PersonViewModel(InspectionPageViewModel inspection, InspectedPersonType personType, bool requiresEgn = true)
        {
            Inspection = inspection;
            PersonType = personType;

            SearchPerson = CommandBuilder.CreateFrom(OnSearchPerson);

            this.AddValidation();

            if (!requiresEgn)
            {
                EGN.Validations.RemoveAt(EGN.Validations.FindIndex(f => f.Name == nameof(RequiredAttribute)));
                EGN.HasAsterisk = false;
                OnPropertyChanged(nameof(EGN));
            }
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

        [MaxLength(500)]
        public ValidState Address { get; set; }

        [Required]
        public ValidStateSelect<SelectNomenclatureDto> Nationality { get; set; }

        public List<SelectNomenclatureDto> Nationalities
        {
            get => _nationalities;
            private set => SetProperty(ref _nationalities, value);
        }

        public ICommand SearchPerson { get; }

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

        private async Task OnSearchPerson()
        {
            PersonFullDataDto data = await InspectionsTransaction.GetPersonFullData(EGN.IdentifierType, EGN.Value);

            if (data?.Person != null)
            {
                FirstName.Value = data.Person.FirstName;
                MiddleName.Value = data.Person.MiddleName;
                LastName.Value = data.Person.LastName;
                Address.Value = string.Empty;
                Nationality.AssignFrom(data.Person.CitizenshipCountryId, Nationalities);
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
