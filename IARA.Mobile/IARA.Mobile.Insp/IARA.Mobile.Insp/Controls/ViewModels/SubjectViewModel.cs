using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Common;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Attributes;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Shared.Attributes;
using IARA.Mobile.Shared.ViewModels.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Attributes;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class SubjectViewModel : ViewModel
    {
        private SelectNomenclatureDto _action;
        private List<SelectNomenclatureDto> _nationalities;
        public SubjectViewModel(InspectionPageViewModel inspection, InspectedPersonType personType, InspectedPersonType legalType, bool isRequired = true, bool requiresEgn = true)
        {
            Inspection = inspection;
            PersonType = personType;
            LegalType = legalType;

            SearchPerson = CommandBuilder.CreateFrom(OnSearchPerson);
            SearchLegal = CommandBuilder.CreateFrom(OnSearchLegal);

            this.AddValidation(groups: new Dictionary<string, Func<bool>>(){
                {nameof(SubjectType.Legal), () => Action.Code == nameof(SubjectType.Legal)},
                {nameof(SubjectType.Person), () => Action.Code == nameof(SubjectType.Person)}
            });

            IsRequired.AddFakeValidation();

            IsRequired.Value = isRequired;

            if (isRequired)
            {
                FirstName.HasAsterisk = true;
                LastName.HasAsterisk = true;
                EGN.HasAsterisk = true;
                EIK.HasAsterisk = true;
                Nationality.HasAsterisk = true;
            }

            if (!requiresEgn)
            {
                EGN.Validations.RemoveAt(EGN.Validations.FindIndex(f => f.Name == nameof(RequiredIfBooleanEqualsAttribute)));
                EGN.HasAsterisk = false;
                OnPropertyChanged(nameof(EGN));
            }

            Actions = new List<SelectNomenclatureDto>
            {
                new SelectNomenclatureDto
                {
                    Id = 1,
                    Code = nameof(SubjectType.Person),
                    Name = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Person"],
                },
                new SelectNomenclatureDto
                {
                    Id = 2,
                    Code = nameof(SubjectType.Legal),
                    Name = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Legal"],
                }
            };

            _action = Actions[0];
        }

        public int? Id { get; set; }

        public InspectionPageViewModel Inspection { get; }
        public InspectedPersonType PersonType { get; }
        public InspectedPersonType LegalType { get; }

        public SelectNomenclatureDto Action
        {
            get => _action;
            set => SetProperty(ref _action, value);
        }

        public ValidStateBool IsRequired { get; set; }

        [RequiredIfBooleanEquals(nameof(IsRequired), false, ErrorMessageResourceName = "Required")]
        [MaxLength(200)]
        public ValidState FirstName { get; set; }

        [MaxLength(200)]
        public ValidState MiddleName { get; set; }

        [RequiredIfBooleanEquals(nameof(IsRequired), false, ErrorMessageResourceName = "Required")]
        [MaxLength(200)]
        [ValidGroup(nameof(SubjectType.Person))]
        public ValidState LastName { get; set; }

        [MaxLength(20)]
        [EGN(nameof(EGN))]
        [RequiredIfBooleanEquals(nameof(IsRequired), false, ErrorMessageResourceName = "Required")]
        [ValidGroup(nameof(SubjectType.Person))]
        public EgnLncValidState EGN { get; set; }

        [EIKValidation(ErrorMessageResourceName = "EIKNotValid")]
        [RequiredIfBooleanEquals(nameof(IsRequired), false, ErrorMessageResourceName = "Required")]
        [ValidGroup(nameof(SubjectType.Legal))]
        public ValidState EIK { get; set; }


        [MaxLength(500)]
        public ValidState Address { get; set; }

        [RequiredIfBooleanEquals(nameof(IsRequired), false, ErrorMessageResourceName = "Required")]
        public ValidStateSelect<SelectNomenclatureDto> Nationality { get; set; }

        public List<SelectNomenclatureDto> Actions { get; }

        public List<SelectNomenclatureDto> Nationalities
        {
            get => _nationalities;
            private set => SetProperty(ref _nationalities, value);
        }

        public ICommand SearchPerson { get; }
        public ICommand SearchLegal { get; }
        public ICommand SubjectChosen { get; set; }

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

            InspectionSubjectPersonnelDto subject = personnel.Find(f => f.Type == PersonType || f.Type == LegalType);
            OnEdit(subject);
        }

        public void OnEdit(InspectionSubjectPersonnelDto subject)
        {
            if (subject != null)
            {
                Id = subject.Id;

                if (subject.IsLegal)
                {
                    Action = Actions.Find(f => f.Code == nameof(SubjectType.Legal));
                }
                else if (subject.EgnLnc != null)
                {
                    Action = Actions.Find(f => f.Code == nameof(SubjectType.Person));
                    EGN.IdentifierType = subject.EgnLnc.IdentifierType;
                }

                FirstName.AssignFrom(subject.FirstName);
                MiddleName.AssignFrom(subject.MiddleName);
                LastName.AssignFrom(subject.LastName);
                Address.AssignFrom(subject.Address);
                Nationality.AssignFrom(subject.CitizenshipId, Nationalities);

                if (subject.IsLegal)
                {
                    EIK.AssignFrom(subject.Eik);
                }
                else
                {
                    EGN.AssignFrom(subject.EgnLnc);
                }
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

        private async Task OnSearchLegal()
        {
            LegalFullDataDto data = await InspectionsTransaction.GetLegalFullData(EIK.Value);

            if (data?.Legal != null)
            {
                FirstName.Value = data.Legal.Name;
                EGN.Value = data.Legal.EIK;
                Address.Value = string.Empty;
                Nationality.Value = Nationalities.Find(f => f.Code == CommonConstants.NomenclatureBulgaria);
            }
        }

        public static implicit operator InspectionSubjectPersonnelDto(SubjectViewModel viewModel)
        {
            bool isLegal = viewModel.Action?.Code == nameof(SubjectType.Legal);

            if (isLegal)
            {
                return new InspectionSubjectPersonnelDto
                {
                    IsRegistered = false,
                    Type = viewModel.LegalType,
                    FirstName = viewModel.FirstName,
                    Eik = viewModel.EIK.Value,
                    IsLegal = true,
                    Address = viewModel.Address,
                    CitizenshipId = viewModel.Nationality.Value,
                };
            }
            else
            {
                return new InspectionSubjectPersonnelDto
                {
                    IsRegistered = false,
                    Type = viewModel.PersonType,
                    FirstName = viewModel.FirstName,
                    MiddleName = viewModel.MiddleName,
                    LastName = viewModel.LastName,
                    EgnLnc = !string.IsNullOrWhiteSpace(viewModel.EGN.Value)
                        ? viewModel.EGN
                        : null,
                    IsLegal = false,
                    Address = viewModel.Address,
                    CitizenshipId = viewModel.Nationality.Value,
                };
            }
        }
    }
}
