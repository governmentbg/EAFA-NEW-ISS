using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Application.DTObjects.Common.API;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Application.DTObjects.Profile;
using IARA.Mobile.Application.DTObjects.Profile.API;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Pub.Application.Attributes;
using IARA.Mobile.Pub.Application.DTObjects.DocumentTypes.LocalDb;
using IARA.Mobile.Pub.ViewModels.Base;
using IARA.Mobile.Shared.ViewModels;
using TechnoLogica.Xamarin.Attributes;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.ViewModels.FlyoutPages
{
    public class ProfileViewModel : MainPageViewModel
    {
        private List<SelectNomenclatureDto> _countries;
        private ProfileApiDto profile;
        private bool _hasAddressOfCorrespondence = true;
        private TLFileResult _imageFile;
        private ImageSource _imagePath;
        private bool _hasBulgarianAddressRegistration;
        private ValidStateRadioButtonList<NomenclatureDto> _gender;
        private List<NomenclatureDto> _genders;
        private ValidStateSelect<DocumentTypeSelectDto> _documentType;
        private ValidState _documentNumber;
        private bool _notificationAgreement;
        private const int ALL_DISTRICTS_ID = -1;
        public ProfileViewModel()
        {
            DocumentTypes = new TLObservableCollection<DocumentTypeSelectDto>();
            Roles = new TLObservableCollection<RoleApiDto>();
            Legals = new TLObservableCollection<RegixLegalDataApiDto>();
            NewsDistricts = new TLObservableCollection<SelectNomenclatureDto>();
            Save = CommandBuilder.CreateFrom(OnSave);
            UploadImage = CommandBuilder.CreateFrom<TLFileResult>(OnUploadImage);
            DocumentChanged = CommandBuilder.CreateFrom(OnDocumentChanged);
            PermanentAddress = new DetailedAddressViewModel();
            CorrespondenceAddress = new DetailedAddressViewModel();
            NotificationAgreementChecked = CommandBuilder.CreateFrom(OnNotificationAgreementChecked);
            NewsDistrictSelected = CommandBuilder.CreateFrom(OnNewsDistrictSelected);
            this.AddValidation(
                groups: new Dictionary<string, Func<bool>>
                {
                    { Group.CORRESPONDENCE, () => !HasSameAddressAsCorrespondence },
                    { Group.DOCUMENT, () => DocumentType.Value != null || !string.IsNullOrEmpty(DocumentNumber) || !string.IsNullOrEmpty(IssuedFrom.Value) || IssuedOn.Value !=null },
                    { Group.NEWS_DISTRICTS, () => NotificationAgreement },
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
        [StringLength(200)]
        public ValidState FirstName { get; set; }

        [StringLength(200)]
        public ValidState MiddleName { get; set; }

        [Required]
        [StringLength(200)]
        public ValidState LastName { get; set; }

        [Required]
        [ValidGroup(Group.DOCUMENT)]
        public ValidStateSelect<DocumentTypeSelectDto> DocumentType
        {
            get => _documentType;
            set => SetProperty(ref _documentType, value);
        }

        [Required]
        [ValidGroup(Group.DOCUMENT)]
        [StringLength(50)]
        public ValidState DocumentNumber
        {
            get => _documentNumber;
            set => SetProperty(ref _documentNumber, value);
        }
        public ValidStateDate IssuedOn { get; set; }

        [StringLength(50)]
        public ValidState IssuedFrom { get; set; }

        public ValidStateSelect<SelectNomenclatureDto> Citizenship { get; set; }

        public ValidStateDate BirthDate { get; set; }

        [StringLength(50)]
        public ValidState Telephone { get; set; }

        public ValidStateRadioButtonList<NomenclatureDto> Gender
        {
            get => _gender;
            set => SetProperty(ref _gender, value);
        }

        public List<NomenclatureDto> Genders
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

        public bool NotificationAgreement
        {
            get => _notificationAgreement;
            set => SetProperty(ref _notificationAgreement, value);
        }

        public TLObservableCollection<DocumentTypeSelectDto> DocumentTypes { get; }

        public TLObservableCollection<RoleApiDto> Roles { get; }
        public TLObservableCollection<RegixLegalDataApiDto> Legals { get; }
        public TLObservableCollection<SelectNomenclatureDto> NewsDistricts { get; }

        [Required]
        [ListLength(1)]
        [ValidGroup(Group.NEWS_DISTRICTS)]
        public ValidStateMultiSelect<SelectNomenclatureDto> SelectedNewsDistricts { get; set; }

        public ICommand Save { get; }
        public ICommand UploadImage { get; }
        public ICommand ChangeLanguage { get; }
        public ICommand DocumentChanged { get; }
        public ICommand NotificationAgreementChecked { get; }
        public ICommand NewsDistrictSelected { get; }
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

            List<DocumentTypeSelectDto> documentTypes = NomenclaturesTransaction.GetDocumentTypes();
            List<SelectNomenclatureDto> countries = AddressTransaction.GetCountries();
            List<SelectNomenclatureDto> districts = AddressTransaction.GetDistricts();

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
                Gender.Value = Genders.Find(f => f.Value == profile.GenderId.Value);
            }

            PermanentAddress.Countries = countries;
            PermanentAddress.Districts = districts;
            CorrespondenceAddress.Countries = countries;
            CorrespondenceAddress.Districts = districts;

            NotificationAgreement = profile.NewsSubscription != NewsSubscriptionType.None;

            if (NotificationAgreement)
            {
                if (profile.NewsSubscription == NewsSubscriptionType.ALL)
                {
                    SelectedNewsDistricts.Value.Add(AllDistrictsValue());
                }
                else
                {
                    foreach (UserNewsDistrictSubscriptionDto district in profile.NewsDistrictSubscriptions)
                    {
                        SelectedNewsDistricts.Value.Add(new SelectNomenclatureDto
                        {
                            Id = district.Id,
                            Name = district.Name
                        });
                    }
                }
            }

            NewsDistricts.Add(AllDistrictsValue());
            NewsDistricts.AddRange(districts);

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
            OnPropertyChanged(nameof(Roles));
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

        private void OnNotificationAgreementChecked()
        {
            if (!IsBusy)
            {
                if (NotificationAgreement)
                {
                    SelectedNewsDistricts.Value.ReplaceRange(new List<SelectNomenclatureDto> { AllDistrictsValue() });
                }
                else
                {
                    SelectedNewsDistricts.Value.Clear();
                }
            }
        }

        private void OnNewsDistrictSelected()
        {
            SelectNomenclatureDto allDistrictsSubscription = SelectedNewsDistricts.Value.Where(x => x.Id == ALL_DISTRICTS_ID).FirstOrDefault();
            if (allDistrictsSubscription != null && SelectedNewsDistricts.Value.Count > 1)
            {
                if (SelectedNewsDistricts.Value[0].Id == ALL_DISTRICTS_ID)
                {
                    SelectedNewsDistricts.Value.Remove(allDistrictsSubscription);
                }
                else
                {
                    SelectedNewsDistricts.Value.ReplaceRange(new List<SelectNomenclatureDto> { allDistrictsSubscription });
                }
            }
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
                GenderId = Gender.Value?.Value
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

            if (NotificationAgreement)
            {
                bool isAllDistrictsSubscription = SelectedNewsDistricts.Any(x => x.Id == ALL_DISTRICTS_ID);

                if (isAllDistrictsSubscription)
                {
                    edit.NewsSubscription = NewsSubscriptionType.ALL;
                }
                else
                {
                    edit.NewsSubscription = NewsSubscriptionType.Districts;
                    edit.NewsDistrictSubscriptions = new List<UserNewsDistrictSubscriptionDto>();
                    foreach (SelectNomenclatureDto districtSubscription in SelectedNewsDistricts)
                    {
                        edit.NewsDistrictSubscriptions.Add(new UserNewsDistrictSubscriptionDto
                        {
                            Id = districtSubscription.Id,
                            Name = districtSubscription.Name,
                        });
                    }
                }
            }

            bool succeeded = await ProfileTransaction.SaveInfo(edit);
            await TLLoadingHelper.HideFullLoadingScreen();
            if (succeeded)
            {
                await TLSnackbar.Show(TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/SuccessfullySaved"], Color.Green);
            }
            else
            {
                await TLSnackbar.Show(TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/SaveFailed"], Color.Red);
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

        private SelectNomenclatureDto AllDistrictsValue()
        {
            return new SelectNomenclatureDto
            {
                Id = ALL_DISTRICTS_ID,
                Name = TranslateExtension.Translator[nameof(GroupResourceEnum.Profile) + "/AllDistricts"]
            };
        }
    }
}
