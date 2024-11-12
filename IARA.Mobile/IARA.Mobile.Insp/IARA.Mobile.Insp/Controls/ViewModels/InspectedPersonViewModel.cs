using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Shared.Attributes;
using IARA.Mobile.Shared.ViewModels.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using TechnoLogica.Xamarin.Attributes;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class InspectedPersonViewModel : ViewModel
    {
        private SelectNomenclatureDto _action;
        private List<SelectNomenclatureDto> _nationalities;
        private List<ShipPersonnelDto> _people;

        public InspectedPersonViewModel(InspectionPageViewModel inspection, InspectedPersonType personType, InspectedPersonType? legalType = null)
        {
            Inspection = inspection;
            PersonType = personType;
            LegalType = legalType;

            _people = new List<ShipPersonnelDto>();

            PersonChosen = CommandBuilder.CreateFrom<ShipPersonnelDto>(OnPersonChosen);

            this.AddValidation(groups: new Dictionary<string, Func<bool>>
            {
                { Group.REGISTERED, () => InRegister },
                { Group.NOT_REGISTERED, () => !InRegister }
            });

            InRegister.Value = true;

            (InRegister as IValidState).ForceValidation = () =>
            {
                InRegister.IsValid = true;
                return null;
            };
            (Person as IValidState).ForceValidation = () =>
            {
                Person.IsValid = true;
                return null;
            };

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

        public InspectionPageViewModel Inspection { get; }

        public InspectedPersonType PersonType { get; }
        public InspectedPersonType? LegalType { get; }
        public bool IsRepresenter { get; set; }

        public SelectNomenclatureDto Action
        {
            get => _action;
            set => SetProperty(ref _action, value);
        }

        public ShipPersonnelDetailedDto ShipUser { get; set; }

        public ValidStateBool InRegister { get; set; }

        [Required]
        [ValidGroup(Group.REGISTERED)]
        public ValidStateSelect<ShipPersonnelDto> Person { get; set; }

        [MaxLength(200)]
        [ValidGroup(Group.NOT_REGISTERED)]
        public ValidState FirstName { get; set; }

        [MaxLength(200)]
        [ValidGroup(Group.NOT_REGISTERED)]
        public ValidState MiddleName { get; set; }

        [MaxLength(200)]
        [ValidGroup(Group.NOT_REGISTERED)]
        public ValidState LastName { get; set; }

        [EGN(nameof(Egn))]
        [MaxLength(20)]
        [ValidGroup(Group.NOT_REGISTERED)]
        public EgnLncValidState Egn { get; set; }

        [MaxLength(20)]
        [ValidGroup(Group.NOT_REGISTERED)]
        public EgnLncValidState EIK { get; set; }

        [MaxLength(4000)]
        [ValidGroup(Group.NOT_REGISTERED)]
        public ValidState Address { get; set; }

        [ValidGroup(Group.NOT_REGISTERED)]
        public ValidStateSelect<SelectNomenclatureDto> Nationality { get; set; }

        public List<SelectNomenclatureDto> Actions { get; }

        public List<SelectNomenclatureDto> Nationalities
        {
            get => _nationalities;
            private set => SetProperty(ref _nationalities, value);
        }
        public List<ShipPersonnelDto> People
        {
            get => _people;
            set => SetProperty(ref _people, value);
        }

        public ICommand PersonChosen { get; set; }

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

            InspectionSubjectPersonnelDto person = personnel.Find(f => f.Type == PersonType || f.Type == LegalType);

            OnEdit(person);
        }

        public void OnEdit(InspectionSubjectPersonnelDto subject)
        {
            if (subject == null)
            {
                return;
            }

            if (subject.IsRegistered && subject.Id.HasValue)
            {
                ShipUser = new ShipPersonnelDetailedDto
                {
                    Id = subject.Id.Value,
                    Address = subject.RegisteredAddress,
                    FirstName = subject.FirstName,
                    MiddleName = subject.MiddleName,
                    LastName = subject.LastName,
                    EgnLnc = subject.EgnLnc,
                    EntryId = subject.EntryId,
                    Type = subject.Type,
                    IsLegal = subject.IsLegal,
                    Eik = subject.Eik,
                };

                InRegister.Value = true;
                Person.AssignFrom(subject.Id.Value, People);
                Address.Value = subject.RegisteredAddress != null
                    ? subject.RegisteredAddress.BuildAddress()
                    : subject.Address;
            }
            else
            {
                InRegister.Value = false;
                Address.Value = subject.Address;
            }

            if (subject.IsLegal)
            {
                Action = Actions.Find(f => f.Code == nameof(SubjectType.Legal));
                Egn.IdentifierType = IdentifierTypeEnum.EGN;
            }
            else if (subject.EgnLnc != null)
            {
                Action = Actions.Find(f => f.Code == nameof(SubjectType.Person));
                Egn.IdentifierType = subject.EgnLnc.IdentifierType;
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
                Egn.AssignFrom(subject.EgnLnc);
            }

            FirstName.Value = subject.FirstName;
            MiddleName.Value = subject.MiddleName;
            LastName.Value = subject.LastName;

            if (subject.CitizenshipId.HasValue)
            {
                Nationality.Value = Nationalities.Find(f => f.Id == subject.CitizenshipId.Value);
            }
        }

        private void OnPersonChosen(ShipPersonnelDto person)
        {
            ShipUser = NomenclaturesTransaction.GetDetailedShipPerson(person.EntryId.Value, person.Type);

            if (ShipUser == null)
            {
                return;
            }

            if (ShipUser.Address != null)
            {
                Nationality.AssignFrom(ShipUser.Address.CountryId, Nationalities);
                Address.Value = ShipUser.Address.BuildAddress();
            }

            InRegister.Value = true;
            FirstName.Value = ShipUser.FirstName;
            MiddleName.Value = ShipUser.MiddleName;
            LastName.Value = ShipUser.LastName;
            Egn.AssignFrom(ShipUser.EgnLnc);
        }

        public static implicit operator InspectionSubjectPersonnelDto(InspectedPersonViewModel viewModel)
        {
            bool isLegal = viewModel.Action?.Code == nameof(SubjectType.Legal);

            if (!viewModel.InRegister)
            {
                if (isLegal)
                {
                    return new InspectionSubjectPersonnelDto
                    {
                        IsRegistered = false,
                        Type = viewModel.LegalType.Value,
                        FirstName = viewModel.FirstName,
                        Eik = viewModel.EIK,
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
                        EgnLnc = !string.IsNullOrWhiteSpace(viewModel.Egn.Value)
                            ? viewModel.Egn
                            : null,
                        IsLegal = false,
                        Address = viewModel.Address,
                        CitizenshipId = viewModel.Nationality.Value,
                    };
                }
            }
            else if (viewModel.ShipUser != null)
            {
                ShipPersonnelDetailedDto user = viewModel.ShipUser;

                if (isLegal)
                {
                    return new InspectionSubjectPersonnelDto
                    {
                        IsRegistered = true,
                        RegisteredAddress = user.Address,
                        Address = user.Address?.BuildAddress(),
                        CitizenshipId = user.Address?.CountryId,
                        FirstName = user.FirstName,
                        Eik = viewModel.EIK,
                        IsLegal = true,
                        EntryId = user.EntryId,
                        Id = user.Id,
                        Type = viewModel.IsRepresenter ? viewModel.LegalType.Value : user.Type,
                    };
                }
                else
                {
                    return new InspectionSubjectPersonnelDto
                    {
                        IsRegistered = true,
                        RegisteredAddress = user.Address,
                        Address = user.Address?.BuildAddress(),
                        CitizenshipId = user.Address?.CountryId,
                        EgnLnc = viewModel.Egn,
                        IsLegal = false,
                        FirstName = user.FirstName,
                        MiddleName = user.MiddleName,
                        LastName = user.LastName,
                        EntryId = user.EntryId,
                        Id = user.Id,
                        Type = viewModel.IsRepresenter ? viewModel.PersonType : user.Type,
                    };
                }
            }

            return null;
        }
    }
}
