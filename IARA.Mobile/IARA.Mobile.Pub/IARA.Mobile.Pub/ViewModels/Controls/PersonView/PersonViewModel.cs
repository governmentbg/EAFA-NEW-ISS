using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Common;
using IARA.Mobile.Application.DTObjects.Common.API;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Application.DTObjects.Profile.API;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Pub.Application.DTObjects.AddressNomenclatures.LocalDb;
using IARA.Mobile.Pub.Domain.Enums;
using IARA.Mobile.Pub.ViewModels.Base;
using IARA.Mobile.Shared.Attributes;
using IARA.Mobile.Shared.ViewModels.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Input;
using TechnoLogica.Xamarin.Attributes;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Pub.ViewModels.Controls.PersonView
{
    public class PersonViewModel : ViewModel
    {
        private readonly int _countryIdBulgaria;
        private List<CountrySelectDto> _countries;
        private List<DistrictSelectDto> _districts;
        private bool _permanentAddressMatchWithCorrespondence;
        private AddressViewModel _permanentAddress;
        private AddressViewModel _correspondenceAddress;
        private bool _hasBulgarianAddressRegistration;
        private ValidState _firstName;
        private ValidState _middleName;
        private ValidState _lastName;
        private EgnLncValidState _egnLnc;
        private ForeignerViewModel _foreigner;
        private BulgarianCitizenViewModel _bulgarianCitizen;
        private ValidStateDate _dateOfBirth;
        private ValidStateBool _isUnder14;
        private bool _isEditEnabled;
        private bool _dateOfBirthRequired;
        private bool _genderRequired;
        private ValidStateRadioButtonList<NomenclatureDto> _gender;
        private List<NomenclatureDto> _genders;

        public PersonViewModel()
        {
            Genders = new List<NomenclatureDto>();
            PermanentAddress = new AddressViewModel();
            CorrespondenceAddress = new AddressViewModel();
            Countries = NomenclaturesTransaction.GetCountries();
            Districts = NomenclaturesTransaction.GetDistricts();
            HasBulgarianAddressChangedCommand = CommandBuilder.CreateFrom<bool>(HasBulgarianAddressChanged);
            Foreigner = new ForeignerViewModel();
            BulgarianCitizen = new BulgarianCitizenViewModel();
            Foreigner.DocumentTypes = NomenclaturesTransaction.GetDocumentTypes();
            HasBulgarianAddressRegistration = true;
            PermanentAddressMatchWithCorrespondence = true;
            _countryIdBulgaria = Countries.First(c => c.Code == CommonConstants.NomenclatureBulgaria).Id;
            PermanentAddress.AddValidation(new Dictionary<string, Func<bool>>()
            {
                { Group.BULGARIAN_ADDRESS, () =>  HasBulgarianAddressRegistration},
                { Group.ADDRESS, () =>  true},
            });
            CorrespondenceAddress.AddValidation(new Dictionary<string, Func<bool>>()
            {
                { Group.BULGARIAN_ADDRESS, () => !PermanentAddressMatchWithCorrespondence
                                        && (CorrespondenceAddress.Country?.Value?.Id == _countryIdBulgaria || HasBulgarianAddressRegistration)},
                { Group.ADDRESS, () => !PermanentAddressMatchWithCorrespondence }
            });
            this.AddValidation(new Dictionary<string, Func<bool>>()
            {
                { Group.BULGARIAN_CITIZEN, () => IsBulgarianCitizen && !IsUnder14},
                { Group.FOREIGNER, () => !IsBulgarianCitizen},
                { Group.DATEOFBIRTH, () => !IsBulgarianCitizen || DateOfBirthRequired},
                { Group.Gender, () => GenderRequired},
                { Group.IS_UNDER_14, () => IsUnder14},
            },
            others: new IValidatableViewModel[] { Foreigner, BulgarianCitizen, PermanentAddress, CorrespondenceAddress });

            (IsUnder14 as IValidState).ForceValidation = () =>
            {
                IsUnder14.IsValid = true;
                return new List<string>();
            };

            EgnLnc.PropertyChanged += OnEgnLncPropertyChanged;
        }

        public bool IsEditEnabled
        {
            get => _isEditEnabled;
            set => SetProperty(ref _isEditEnabled, value);
        }

        public bool DateOfBirthRequired
        {
            get => _dateOfBirthRequired;
            set => SetProperty(ref _dateOfBirthRequired, value);
        }

        public bool GenderRequired
        {
            get => _genderRequired;
            set => SetProperty(ref _genderRequired, value);
        }

        public List<NomenclatureDto> Genders
        {
            get => _genders;
            set => SetProperty(ref _genders, value);
        }

        public ValidStateBool IsUnder14
        {
            get => _isUnder14;
            set => SetProperty(ref _isUnder14, value);
        }

        [Required]
        [StringLength(200)]
        public ValidState FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        [StringLength(200)]
        public ValidState MiddleName
        {
            get => _middleName;
            set => SetProperty(ref _middleName, value);
        }

        [Required]
        [StringLength(200)]
        public ValidState LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        [Required]
        [StringLength(20)]
        [EGN(nameof(EgnLnc))]
        [ValidGroup(Group.IS_UNDER_14)]
        public EgnLncValidState EgnLnc
        {
            get => _egnLnc;
            set => SetProperty(ref _egnLnc, value);
        }

        public bool IsBulgarianCitizen => IdentifierTypeEnum.EGN == EgnLnc?.IdentifierType;

        public bool HasBulgarianAddressRegistration
        {
            get => _hasBulgarianAddressRegistration;
            set => SetProperty(ref _hasBulgarianAddressRegistration, value);
        }

        public BulgarianCitizenViewModel BulgarianCitizen
        {
            get => _bulgarianCitizen;
            set => SetProperty(ref _bulgarianCitizen, value);
        }

        [Required]
        [ValidGroup(Group.DATEOFBIRTH)]
        public ValidStateDate DateOfBirth
        {
            get => _dateOfBirth;
            set => SetProperty(ref _dateOfBirth, value);
        }

        [Required]
        [ValidGroup(Group.Gender)]
        public ValidStateRadioButtonList<NomenclatureDto> Gender
        {
            get => _gender;
            set => SetProperty(ref _gender, value);
        }

        public ForeignerViewModel Foreigner
        {
            get => _foreigner;
            set => SetProperty(ref _foreigner, value);
        }

        public AddressViewModel PermanentAddress
        {
            get => _permanentAddress;
            set => SetProperty(ref _permanentAddress, value);
        }

        public bool PermanentAddressMatchWithCorrespondence
        {
            get => _permanentAddressMatchWithCorrespondence;
            set => SetProperty(ref _permanentAddressMatchWithCorrespondence, value);
        }

        public AddressViewModel CorrespondenceAddress
        {
            get => _correspondenceAddress;
            set => SetProperty(ref _correspondenceAddress, value);
        }

        public List<CountrySelectDto> Countries
        {
            get => _countries;
            set => SetProperty(ref _countries, value);
        }

        public List<DistrictSelectDto> Districts
        {
            get => _districts;
            set => SetProperty(ref _districts, value);
        }

        public DateTime Today => DateTime.Now;

        public ICommand HasBulgarianAddressChangedCommand { get; }

        private void HasBulgarianAddressChanged(bool hasBulgarianAddress)
        {
            if (hasBulgarianAddress)
            {
                PermanentAddress.Country.Value = Countries.First(x => x.Code == CommonConstants.NomenclatureBulgaria);
            }
        }

        private void OnEgnLncPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EgnLnc.IdentifierType))
            {
                OnPropertyChanged(nameof(IsBulgarianCitizen));
            }
        }

        public BasePersonInfoApiDto MapToApiPerson()
        {
            BasePersonInfoApiDto person = new BasePersonInfoApiDto
            {
                FirstName = FirstName.Value,
                MiddleName = MiddleName?.Value,
                LastName = LastName.Value,
                EgnLnc = new EgnLncDto
                {
                    EgnLnc = EgnLnc.Value,
                    IdentifierType = EgnLnc.IdentifierType
                },
                HasBulgarianAddressRegistration = HasBulgarianAddressRegistration,
                CitizenshipCountryId = IsBulgarianCitizen ? _countryIdBulgaria : Foreigner.Citizenship.Value.Id,
            };

            if (GenderRequired)
            {
                person.GenderId = NomenclaturesTransaction.GetActiveGenderId(Gender.Value.Code);
            }

            if (DateOfBirthRequired || EgnLnc.IdentifierType != IdentifierTypeEnum.EGN)
            {
                person.BirthDate = DateOfBirth.Value;
            }

            if (IsBulgarianCitizen)
            {
                if (!IsUnder14)
                {
                    int documentTypeId = NomenclaturesTransaction.GetDocumentTypeIdByCode(nameof(DocumentTypeEnum.LK));
                    person.Document = new PersonDocumentApiDto()
                    {
                        DocumentTypeId = documentTypeId,
                        DocumentNumber = BulgarianCitizen.Idcard.Value,
                        DocumentIssuedOn = BulgarianCitizen.IdcardDate.Value,
                        DocumentIssuedBy = BulgarianCitizen.IdcardPublisher.Value,
                    };
                }
            }
            else
            {
                person.Document = new PersonDocumentApiDto()
                {
                    DocumentNumber = Foreigner.Idcard.Value,
                    DocumentTypeId = Foreigner.DocumentType.Value.Id,
                };
            }

            return person;
        }

        public List<AddressRegistrationApiDto> MapToApiAddresses()
        {
            List<AddressRegistrationApiDto> addresses = new List<AddressRegistrationApiDto>();

            if (HasBulgarianAddressRegistration)
            {
                addresses.Add(new AddressRegistrationApiDto()
                {
                    AddressType = AddressType.PERMANENT,
                    CountryId = PermanentAddress.Country.Value.Id,
                    DistrictId = PermanentAddress.District.Value.Id,
                    MunicipalityId = PermanentAddress.Municipality.Value.Id,
                    PopulatedAreaId = PermanentAddress.PopulatedArea.Value.Id,
                    Street = PermanentAddress.Street.Value,
                    Region = PermanentAddress.Region.Value,
                    PostalCode = PermanentAddress.ZipCode.Value,
                    StreetNum = PermanentAddress.ZipCode.Value,
                    BlockNum = PermanentAddress.Block.Value,
                    EntranceNum = PermanentAddress.Entrance.Value,
                    FloorNum = PermanentAddress.Floor.Value,
                    ApartmentNum = PermanentAddress.Apartment.Value
                });

                if (!PermanentAddressMatchWithCorrespondence)
                {
                    addresses.Add(new AddressRegistrationApiDto()
                    {
                        AddressType = AddressType.CORRESPONDENCE,
                        CountryId = CorrespondenceAddress.Country.Value.Id,
                        DistrictId = CorrespondenceAddress.District.Value.Id,
                        MunicipalityId = CorrespondenceAddress.Municipality.Value.Id,
                        PopulatedAreaId = CorrespondenceAddress.PopulatedArea.Value.Id,
                        Street = CorrespondenceAddress.Street.Value,
                        Region = CorrespondenceAddress.Region.Value,
                        PostalCode = CorrespondenceAddress.ZipCode.Value,
                        StreetNum = CorrespondenceAddress.ZipCode.Value,
                        BlockNum = CorrespondenceAddress.Block.Value,
                        EntranceNum = CorrespondenceAddress.Entrance.Value,
                        FloorNum = CorrespondenceAddress.Floor.Value,
                        ApartmentNum = CorrespondenceAddress.Apartment.Value
                    });
                }
            }
            else
            {
                addresses.Add(new AddressRegistrationApiDto()
                {
                    AddressType = AddressType.PERMANENT,
                    CountryId = PermanentAddress.Country.Value.Id,
                    Street = PermanentAddress.Street.Value,
                    Region = PermanentAddress.Region.Value,
                    PostalCode = PermanentAddress.ZipCode.Value,
                    StreetNum = PermanentAddress.ZipCode.Value,
                    BlockNum = PermanentAddress.Block.Value,
                    EntranceNum = PermanentAddress.Entrance.Value,
                    FloorNum = PermanentAddress.Floor.Value,
                    ApartmentNum = PermanentAddress.Apartment.Value
                });

                if (!PermanentAddressMatchWithCorrespondence)
                {
                    AddressRegistrationApiDto correspondenceAddress = new AddressRegistrationApiDto()
                    {
                        AddressType = AddressType.CORRESPONDENCE,
                        CountryId = CorrespondenceAddress.Country.Value.Id,
                        Street = CorrespondenceAddress.Street.Value,
                        Region = CorrespondenceAddress.Region.Value,
                        PostalCode = CorrespondenceAddress.ZipCode.Value,
                        StreetNum = CorrespondenceAddress.ZipCode.Value,
                        BlockNum = CorrespondenceAddress.Block.Value,
                        EntranceNum = CorrespondenceAddress.Entrance.Value,
                        FloorNum = CorrespondenceAddress.Floor.Value,
                        ApartmentNum = CorrespondenceAddress.Apartment.Value
                    };

                    if (CorrespondenceAddress.Country.Value.Id == _countryIdBulgaria)
                    {
                        correspondenceAddress.DistrictId = CorrespondenceAddress.District.Value.Id;
                        correspondenceAddress.MunicipalityId = CorrespondenceAddress.Municipality.Value.Id;
                        correspondenceAddress.PopulatedAreaId = CorrespondenceAddress.PopulatedArea.Value.Id;
                    }

                    addresses.Add(correspondenceAddress);
                }
            }

            return addresses;
        }
    }
}
