using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Application.DTObjects.Common.API;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Application.DTObjects.Profile.API;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Shared.ViewModels;
using TechnoLogica.Xamarin.Attributes;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.FlyoutPages.ProfilePage
{
    public class ProfileViewModel : MainPageViewModel
    {
        private List<SelectNomenclatureDto> _countries;
        private List<SelectNomenclatureDto> _genders;
        private ProfileApiDto profile;
        private TLFileResult _imageFile;
        private ImageSource _imagePath;
        private bool _hasAddressOfCorrespondence = true;
        private bool _hasBulgarianAddressRegistration;

        public ProfileViewModel()
        {
            DocumentTypes = new TLObservableCollection<SelectNomenclatureDto>();
            Roles = new TLObservableCollection<RoleApiDto>();
            Legals = new TLObservableCollection<RegixLegalDataApiDto>();

            Save = CommandBuilder.CreateFrom(OnSave);
            UploadImage = CommandBuilder.CreateFrom<TLFileResult>(OnUploadImage);
            DocumentChanged = CommandBuilder.CreateFrom(OnDocumentChanged);

            PermanentAddress = new DetailedAddressViewModel();
            CorrespondenceAddress = new DetailedAddressViewModel();

            this.AddValidation(
                groups: new Dictionary<string, Func<bool>>
                {
                    { Group.CORRESPONDENCE, () => !HasSameAddressAsCorrespondence },
                    { Group.DOCUMENT, () => DocumentType.Value != null || !string.IsNullOrEmpty(DocumentNumber) || !string.IsNullOrEmpty(IssuedFrom.Value) || IssuedOn.Value !=null },
                },
                others: new[] { PermanentAddress, CorrespondenceAddress }
            );

            DocumentType.HasAsterisk = false;
            DocumentNumber.HasAsterisk = false;

            CorrespondenceAddress.Validation.GlobalGroups = new List<string> { Group.CORRESPONDENCE };
        }

        public DetailedAddressViewModel PermanentAddress { get; }
        public DetailedAddressViewModel CorrespondenceAddress { get; }

        public ValidState Username { get; set; }

        public ValidState EgnLnc { get; set; }

        [Required]
        [MaxLength(200)]
        public ValidState FirstName { get; set; }

        [MaxLength(200)]
        public ValidState MiddleName { get; set; }

        [Required]
        [MaxLength(200)]
        public ValidState LastName { get; set; }

        [Required]
        [ValidGroup(Group.DOCUMENT)]
        public ValidStateSelect<SelectNomenclatureDto> DocumentType { get; set; }

        [Required]
        [ValidGroup(Group.DOCUMENT)]
        [MaxLength(50)]
        public ValidState DocumentNumber { get; set; }

        public ValidStateDate IssuedOn { get; set; }

        [MaxLength(50)]
        public ValidState IssuedFrom { get; set; }

        public ValidStateSelect<SelectNomenclatureDto> Citizenship { get; set; }

        public ValidStateDate BirthDate { get; set; }

        [MaxLength(50)]
        public ValidState Telephone { get; set; }

        public ValidStateRadioButtonList<SelectNomenclatureDto> Gender { get; set; }

        public List<SelectNomenclatureDto> Genders
        {
            get => _genders;
            set => SetProperty(ref _genders, value);
        }
        public bool HasBulgarianAddressRegistration
        {
            get => _hasBulgarianAddressRegistration;
            set => SetProperty(ref _hasBulgarianAddressRegistration, value);
        }
        public List<SelectNomenclatureDto> Countries
        {
            get => _countries;
            set => SetProperty(ref _countries, value);
        }
        public bool HasSameAddressAsCorrespondence
        {
            get => _hasAddressOfCorrespondence;
            set => SetProperty(ref _hasAddressOfCorrespondence, value);
        }
        public ImageSource Photo
        {
            get => _imagePath;
            set => SetProperty(ref _imagePath, value);
        }

        public TLObservableCollection<SelectNomenclatureDto> DocumentTypes { get; }
        public TLObservableCollection<RoleApiDto> Roles { get; }
        public TLObservableCollection<RegixLegalDataApiDto> Legals { get; }

        public ICommand Save { get; }
        public ICommand UploadImage { get; }
        public ICommand ChangeLanguage { get; }
        public ICommand DocumentChanged { get; }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[] { GroupResourceEnum.Profile, GroupResourceEnum.Validation };
        }

        public override async Task Initialize(object sender)
        {
            await TLLoadingHelper.ShowFullLoadingScreen();
            profile = await ProfileTransaction.GetInfo();

            if (profile == null)
            {
                await TLLoadingHelper.HideFullLoadingScreen();
                return;
            }

            Genders = NomenclaturesTransaction.GetGenders();

            List<SelectNomenclatureDto> documentTypes = NomenclaturesTransaction.GetDocumentTypes();
            List<SelectNomenclatureDto> countries = NomenclaturesTransaction.GetCountries();
            List<SelectNomenclatureDto> districts = NomenclaturesTransaction.GetDistricts();

            Username.Value = profile.Username;
            EgnLnc.Value = profile.EgnLnc.EgnLnc;
            FirstName.Value = profile.FirstName;
            MiddleName.Value = profile.MiddleName;
            LastName.Value = profile.LastName;
            if (profile.Document != null)
            {
                DocumentType.Value = documentTypes.Find(f => f.Id == profile.Document.DocumentTypeId);
                DocumentNumber.Value = profile.Document.DocumentNumber;
                IssuedOn.Value = profile.Document.DocumentIssuedOn;
                IssuedFrom.Value = profile.Document.DocumentIssuedBy;
            }
            Citizenship.Value = profile.CitizenshipCountryId.HasValue ? countries.Find(f => f.Id == profile.CitizenshipCountryId) : null;
            BirthDate.Value = profile.BirthDate;
            Telephone.Value = profile.Phone;
            HasBulgarianAddressRegistration = profile.HasBulgarianAddressRegistration.GetValueOrDefault();
            Countries = countries;

            if (profile.GenderId.HasValue)
            {
                Gender.Value = Genders.Find(f => f.Id == profile.GenderId.Value);
            }

            PermanentAddress.Countries = countries;
            PermanentAddress.Districts = districts;
            CorrespondenceAddress.Countries = countries;
            CorrespondenceAddress.Districts = districts;

            if (profile.UserAddresses != null)
            {
                AddressRegistrationApiDto permanentAddress = profile.UserAddresses.Find(f => f.AddressType == AddressType.PERMANENT);

                if (permanentAddress != null)
                {
                    PermanentAddress.AssignAddressRegistration(permanentAddress);
                }

                AddressRegistrationApiDto correspondenceAddress = profile.UserAddresses.Find(f => f.AddressType == AddressType.CORRESPONDENCE);

                if (correspondenceAddress != null)
                {
                    CorrespondenceAddress.AssignAddressRegistration(correspondenceAddress);
                    HasSameAddressAsCorrespondence = false;
                }
            }

            DocumentTypes.AddRange(documentTypes);
            Roles.AddRange(profile.Roles);
            Legals.AddRange(profile.Legals);

            if (profile.Photo?.Id != null)
            {
                byte[] bytes = await ProfileTransaction.GetPhoto();
                if (bytes != null)
                {
                    Photo = ImageSource.FromStream(() => new MemoryStream(bytes));
                }
            }

            await TLLoadingHelper.HideFullLoadingScreen();
            IsBusy = false;
        }

        private async Task OnSave()
        {
            if (profile == null)
            {
                return;
            }

            Validation.Force();

            if (!Validation.IsValid)
            {
                return;
            }

            await TLLoadingHelper.ShowFullLoadingScreen();

            EditProfileApiDto edit = new EditProfileApiDto
            {
                Id = profile.Id,
                EgnLnc = profile.EgnLnc,
                Username = profile.Username,
                Email = profile.Email,
                FirstName = FirstName.Value,
                MiddleName = MiddleName.Value,
                LastName = LastName.Value,
                CitizenshipCountryId = Citizenship.Value?.Id,
                BirthDate = BirthDate.Value,
                Phone = Telephone.Value,
                NewsSubscription = profile.NewsSubscription,
                NewsDistrictSubscriptions = profile.NewsDistrictSubscriptions,
                Photo = _imageFile != null ? new FileModel
                {
                    FullPath = _imageFile.FullPath,
                    ContentType = _imageFile.ContentType,
                    UploadedOn = DateTime.Now
                } : null,
                HasBulgarianAddressRegistration = HasBulgarianAddressRegistration,
                UserAddresses = new List<AddressRegistrationApiDto>
                {
                    new AddressRegistrationApiDto
                    {
                        AddressType = AddressType.PERMANENT,
                        ApartmentNum = PermanentAddress.Apartment.Value,
                        BlockNum = PermanentAddress.Block.Value,
                        CountryId = PermanentAddress.Country.Value.Id,
                        DistrictId = PermanentAddress.District.Value?.Id,
                        EntranceNum = PermanentAddress.Entrance.Value,
                        FloorNum = PermanentAddress.Floor.Value,
                        MunicipalityId = PermanentAddress.Municipality.Value?.Id,
                        PopulatedAreaId = PermanentAddress.PopulatedArea.Value?.Id,
                        PostalCode = PermanentAddress.ZipCode.Value,
                        Region = PermanentAddress.Region.Value,
                        Street = PermanentAddress.Street.Value,
                        StreetNum = PermanentAddress.Number.Value
                    }
                },
                GenderId = Gender.Value?.Id,
            };

            if (DocumentType.Value != null || !string.IsNullOrEmpty(DocumentNumber))
            {
                edit.Document = new PersonDocumentApiDto
                {
                    DocumentTypeId = DocumentType.Value.Id,
                    DocumentIssuedOn = IssuedOn.Value,
                    DocumentIssuedBy = IssuedFrom.Value,
                    DocumentNumber = DocumentNumber.Value
                };
            }

            if (!HasSameAddressAsCorrespondence)
            {
                edit.UserAddresses.Add(new AddressRegistrationApiDto
                {
                    AddressType = AddressType.CORRESPONDENCE,
                    ApartmentNum = CorrespondenceAddress.Apartment.Value,
                    BlockNum = CorrespondenceAddress.Block.Value,
                    CountryId = CorrespondenceAddress.Country.Value.Id,
                    DistrictId = CorrespondenceAddress.District.Value?.Id,
                    EntranceNum = CorrespondenceAddress.Entrance.Value,
                    FloorNum = CorrespondenceAddress.Floor.Value,
                    MunicipalityId = CorrespondenceAddress.Municipality.Value?.Id,
                    PopulatedAreaId = CorrespondenceAddress.PopulatedArea.Value?.Id,
                    PostalCode = CorrespondenceAddress.ZipCode.Value,
                    Region = CorrespondenceAddress.Region.Value,
                    Street = CorrespondenceAddress.Street.Value,
                    StreetNum = CorrespondenceAddress.Number.Value
                });
            }

            bool succeeded = await ProfileTransaction.SaveInfo(edit);
            await TLLoadingHelper.HideFullLoadingScreen();

            if (succeeded)
            {
                await TLSnackbar.Show(TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/SavedSuccessfully"], Color.Green);
            }
            else
            {
                await TLSnackbar.Show(TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/SaveFailed"], App.GetResource<Color>("ErrorColor"));
            }
        }

        private void OnDocumentChanged()
        {
            if (DocumentType == null && string.IsNullOrEmpty(DocumentNumber))
            {
                DocumentType.HasAsterisk = false;
                DocumentNumber.HasAsterisk = false;
            }
            else
            {
                DocumentType.HasAsterisk = true;
                DocumentNumber.HasAsterisk = true;
            }
        }

        private void OnUploadImage(TLFileResult file)
        {
            _imageFile = file;
            Photo = file.FullPath;
        }
    }
}
