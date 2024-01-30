using IARA.Mobile.Application.DTObjects.Common.API;
using IARA.Mobile.Application.DTObjects.Profile.API;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Pub.Application.DTObjects.AddressNomenclatures.LocalDb;
using IARA.Mobile.Pub.Application.DTObjects.FishingTickets.API;
using IARA.Mobile.Pub.Application.Interfaces.Utilities;
using IARA.Mobile.Pub.Domain.Entities.Nomenclatures;
using IARA.Mobile.Pub.Domain.Enums;
using IARA.Mobile.Pub.Utilities;
using IARA.Mobile.Pub.ViewModels.Base;
using IARA.Mobile.Pub.ViewModels.Controls.PersonView;
using IARA.Mobile.Pub.ViewModels.FlyoutPages.Payments;
using IARA.Mobile.Pub.Views.FlyoutPages;
using IARA.Mobile.Pub.Views.FlyoutPages.Payments;
using IARA.Mobile.Shared.Menu;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Attributes;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.ViewModels.FlyoutPages.FishingTicket
{
    public abstract class BaseTicketViewModel : PageViewModel
    {
        private readonly IFishingTicketsSettings _fishingTicketsSettings;
        private PersonViewModel _person;
        private ValidStateBool _acceptAgreement;
        private ImageSource _photoPath;
        private bool _allowAdditionalUnder14Tickets;
        internal string photoFullPath;
        private bool _isEditEnabled;
        private string _title;
        private bool _hasInvalidData;
        private ValidStateBool _updateProfileData;
        private protected const string ValidToDateTimeFormat = "dd.MM.yyyy HH\\:mm";
        private string _ticketNum;
        public BaseTicketViewModel(IFishingTicketsSettings fishingTicketsSettings)
        {
            UploadPhotoCommand = CommandBuilder.CreateFrom<TLFileResult>(UploadPhoto);
            ChildrenTickets = new TLObservableCollection<Under14SubTicketViewModel>();
            AddUnder14TicketCommand = CommandBuilder.CreateFrom(OnAddUnder14Ticket);
            RemoveUnder14TicketCommand = CommandBuilder.CreateFrom<Under14SubTicketViewModel>(OnRemoveUnder14Ticket);
            _fishingTicketsSettings = fishingTicketsSettings;
            AllowAdditionalUnder14Tickets = fishingTicketsSettings.AllowedUnder14TicketsCount > 0;
            DownloadTicket = CommandBuilder.CreateFrom(DownloadTicketPdf);
            TerritorialUnits = NomenclaturesTransaction.GetTerritorialUnits();
        }

        public BaseTicketViewModel()
        {
            UploadPhotoCommand = CommandBuilder.CreateFrom<TLFileResult>(UploadPhoto);
            ChildrenTickets = new TLObservableCollection<Under14SubTicketViewModel>();
            AddUnder14TicketCommand = CommandBuilder.CreateFrom(OnAddUnder14Ticket);
            RemoveUnder14TicketCommand = CommandBuilder.CreateFrom<Under14SubTicketViewModel>(OnRemoveUnder14Ticket);
            DownloadTicket = CommandBuilder.CreateFrom(DownloadTicketPdf);
            TerritorialUnits = NomenclaturesTransaction.GetTerritorialUnits();
        }

        private ValidStateSelect<TerritorialUnitSelectDto> _territorialUnit;
        [Required]
        public ValidStateSelect<TerritorialUnitSelectDto> TerritorialUnit
        {
            get => _territorialUnit;
            set => SetProperty(ref _territorialUnit, value);
        }

        private List<TerritorialUnitSelectDto> _territorialUnits;
        public List<TerritorialUnitSelectDto> TerritorialUnits
        {
            get => _territorialUnits;
            set => SetProperty(ref _territorialUnits, value);
        }

        public TicketMetadataModel TicketMetadata { get; set; }

        public string TicketNum
        {
            get => _ticketNum;
            set => SetProperty(ref _ticketNum, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        public bool IsEditEnabled
        {
            get => _isEditEnabled;
            set => SetProperty(ref _isEditEnabled, value);
        }
        public bool AllowAdditionalUnder14Tickets
        {
            get => _allowAdditionalUnder14Tickets;
            set => SetProperty(ref _allowAdditionalUnder14Tickets, value);
        }
        public ImageSource Photo
        {
            get => _photoPath;
            set => SetProperty(ref _photoPath, value);
        }
        public string PhotoContentType { get; set; }
        public int? PhotoId { get; set; }
        public PersonViewModel Person
        {
            get => _person;
            set => SetProperty(ref _person, value);
        }

        [Required]
        [ValidGroup(Group.ACCEPT_AGREEMENT)]
        public ValidStateBool AcceptAgreement
        {
            get => _acceptAgreement;
            set => SetProperty(ref _acceptAgreement, value);
        }

        public ValidStateBool UpdateProfileData
        {
            get => _updateProfileData;
            set => SetProperty(ref _updateProfileData, value);
        }

        public bool HasInvalidData
        {
            get => _hasInvalidData;
            set => SetProperty(ref _hasInvalidData, value);
        }
        // Check for Cyrillic E and Latin E
        public bool IsTicketElectronic 
        {
            get
            {
                if(IsActive)
                {
                    if (TicketMetadata == null || TicketMetadata.TicketNumber == null)
                    {
                        return false;
                    }
                    else
                    {
                        return TicketMetadata.TicketNumber.ToUpper().StartsWith("E") || TicketMetadata.TicketNumber.ToUpper().StartsWith("Е");
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        public bool IsActive => TicketMetadata.Id != 0 &&
                    TicketMetadata.Action == TicketAction.View &&
                   (TicketMetadata.PaymentStatus == PaymentStatusEnum.PaidOK
                   || TicketMetadata.PaymentStatus == PaymentStatusEnum.NotNeeded)
                   && TicketMetadata.ApplicationStatusCode != ApplicationStatuses.CORR_BY_USR_NEEDED
                   && TicketMetadata.ApplicationStatusCode != ApplicationStatuses.FILL_BY_APPL;

        public bool CanDownload => !IsTicketElectronic && IsActive;

        public string OnSaveButtonLabel => (TicketMetadata.Action == TicketAction.Update ||
                TicketMetadata.TypeCode == nameof(TicketTypeEnum.UNDER14) ||
                TicketMetadata.TypeCode == nameof(TicketTypeEnum.DISABILITY)) ?
                TranslateExtension.Translator[nameof(GroupResourceEnum.TicketPerson) + "/Complete"] :
                 TranslateExtension.Translator[nameof(GroupResourceEnum.TicketPerson) + "/ProceedToPayment"];
        public TLObservableCollection<Under14SubTicketViewModel> ChildrenTickets { get; set; }
        public ICommand UploadPhotoCommand { get; }
        public ICommand OnSaveCommand { get; set; }
        public ICommand AddUnder14TicketCommand { get; set; }
        public ICommand RemoveUnder14TicketCommand { get; set; }
        public ICommand DownloadTicket { get; set; }
        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[] {
                GroupResourceEnum.FishingTicket,
                GroupResourceEnum.TicketPerson,
                GroupResourceEnum.Validation,
                GroupResourceEnum.Common,
                GroupResourceEnum.Under14Ticket
            };
        }

        protected async Task OnAddUnder14Ticket()
        {
            await TLLoadingHelper.ShowFullLoadingScreen();
            try
            {
                string noPeriod = nameof(TicketPeriodEnum.UNTIL14);
                int periodId = FishingTicketsTransaction.GetTicketPeriodIdByCode(noPeriod);
                string typeCode = nameof(TicketTypeEnum.UNDER14);
                int typeId = FishingTicketsTransaction.GetTicketTypeIdByCode(typeCode);
                Under14SubTicketViewModel ticket = new Under14SubTicketViewModel()
                {
                    TicketMetadata = new TicketMetadataModel()
                    {
                        Name = TranslateExtension.Translator[nameof(GroupResourceEnum.FishingTicket) + "/AdditionalUnder14Ticket"] + (ChildrenTickets.Count + 1),
                        PeriodId = periodId,
                        PeriodCode = noPeriod,
                        TypeCode = typeCode,
                        TypeId = typeId,
                        Action = TicketAction.Create
                    },
                };

                ChildrenTickets.Add(ticket);

                AllowAdditionalUnder14Tickets = _fishingTicketsSettings.AllowedUnder14TicketsCount > ChildrenTickets.Count;
            }
            finally
            {
                await TLLoadingHelper.HideFullLoadingScreen();
            }
        }

        protected async Task OnRemoveUnder14Ticket(Under14SubTicketViewModel ticket)
        {
            bool accept = await App.Current.MainPage.DisplayAlert(TranslateExtension.Translator[nameof(GroupResourceEnum.TicketPerson) + "/Attention"],
               TranslateExtension.Translator[nameof(GroupResourceEnum.FishingTicket) + "/RemoveAdditionalUnder14Ticket"] + ticket.TicketMetadata.Name.ToLower() + "?",
                TranslateExtension.Translator[nameof(GroupResourceEnum.TicketPerson) + "/Ok"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Cancel"]);

            if (accept)
            {
                ChildrenTickets.Remove(ticket);

                int count = 1;
                foreach (Under14SubTicketViewModel childTicket in ChildrenTickets)
                {
                    childTicket.TicketMetadata.Name = TranslateExtension.Translator[nameof(GroupResourceEnum.FishingTicket) + "/AdditionalUnder14Ticket"] + count++;
                }

                AllowAdditionalUnder14Tickets = _fishingTicketsSettings.AllowedUnder14TicketsCount > ChildrenTickets.Count;
            }
        }

        private void UploadPhoto(TLFileResult fileResult)
        {
            if (fileResult != null)
            {
                Photo = fileResult.FullPath;
                photoFullPath = fileResult.FullPath;
                PhotoContentType = fileResult.ContentType;
                PhotoId = null;
            }
        }

        public override Task Initialize(object sender)
        {
            if (TicketMetadata.Action == TicketAction.Create || TicketMetadata.Action == TicketAction.Renew)
            {
                Title = TranslateExtension.Translator[nameof(GroupResourceEnum.FishingTicket) + "/Title"];
            }
            else if (TicketMetadata.Action == TicketAction.Update)
            {
                Title = TranslateExtension.Translator[nameof(GroupResourceEnum.FishingTicket) + "/EditFishingTicket"];
            }
            else if (TicketMetadata.Action == TicketAction.View)
            {
                Title = TranslateExtension.Translator[nameof(GroupResourceEnum.FishingTicket) + "/ViewFishingTicket"];
            }

            IsEditEnabled = TicketMetadata.Action != TicketAction.View;

            return Task.CompletedTask;
        }

        protected virtual Task<bool> CustomValidation()
        {
            return Task.FromResult(false);
        }

        protected List<FishingTicketRequestDto> GetChildTicketsData()
        {
            List<FishingTicketRequestDto> data = new List<FishingTicketRequestDto>();
            foreach (Under14SubTicketViewModel ticket in ChildrenTickets)
            {
                FishingTicketRequestDto under14Ticket = ticket.GetFishingTicketData();
                //Лице представител на допълнителните билети до 14г. се явавя лицето от основния билет;
                under14Ticket.RepresentativePerson = Person.MapToApiPerson();
                under14Ticket.RepresentativePersonAddressRegistrations = Person.MapToApiAddresses();
                data.Add(under14Ticket);
            }
            return data;
        }

        protected abstract FishingTicketRequestDto GetFishingTicketData();

        protected abstract TicketValidationDTO GetTicketValidationData();

        protected async void OnSave()
        {
            await TLLoadingHelper.ShowFullLoadingScreen();
            try
            {
                Validation.Force();

                bool anyInvalidChild = false;
                foreach (Under14SubTicketViewModel childTicket in ChildrenTickets)
                {
                    childTicket.Validation.Force();
                    if (!childTicket.Validation.IsValid)
                    {
                        anyInvalidChild = true;
                    }
                }

                HasInvalidData = !Validation.IsValid || anyInvalidChild || await CustomValidation();

                if (_hasInvalidData)
                {
                    return;
                }

                foreach (Under14SubTicketViewModel childTicket in ChildrenTickets)
                {
                    if (await childTicket.CustomValidation())
                    {
                        return;
                    }
                }

                if (Photo == null && (TicketMetadata.PeriodCode == TicketPeriodEnum.ANNUAL.ToString()
                    || TicketMetadata.PeriodCode == TicketPeriodEnum.HALFYEARLY.ToString()))
                {
                    await DisplayAttentionAlert(TranslateExtension.Translator[nameof(GroupResourceEnum.TicketPerson) + "/ShouldUploadPhoto"]);
                    return;
                }

                if (TicketMetadata.Action != TicketAction.Update && await FishingTicketsTransaction.IsDuplicateTicket(GetTicketValidationData()))
                {
                    await DisplayAttentionAlert(TranslateExtension.Translator[nameof(GroupResourceEnum.FishingTicket) + "/DuplicateTicket"] + "(" + TicketMetadata.Name + ").");
                    return;
                }

                FishingTicketRequestDto ticket = GetFishingTicketData();
                ticket.UpdateProfileData = UpdateProfileData.Value;

                if (TicketMetadata.Action == TicketAction.Create || TicketMetadata.Action == TicketAction.Renew)
                {
                    List<FishingTicketRequestDto> tickets = new List<FishingTicketRequestDto>()
                    {
                        ticket
                    };

                    foreach (FishingTicketRequestDto additionalUnder14Ticket in GetChildTicketsData())
                    {
                        tickets.Add(additionalUnder14Ticket);
                    }

                    AddTicketResponseDto response = await FishingTicketsTransaction.AddTickets(tickets, UpdateProfileData.Value);
                    if (response != null)
                    {
                        MessagingCenter.Instance.Send(new TicketsChangedArgs(TicketsChangedEnum.NewTicket), TicketsChangedArgs.TICKETS_CHANGED_EVENT);

                        if (response.PaidTicketApplicationId.HasValue && TicketMetadata.PaymentStatus != PaymentStatusEnum.NotNeeded)
                        {
                            List<ItemViewModel> items = new List<ItemViewModel>();
                            foreach (FishingTicketRequestDto item in tickets)
                            {
                                string code = FishingTicketsTransaction.GetTicketTypeCodeById(item.TypeId);
                                items.Add(new ItemViewModel
                                {
                                    Name = GetPersonFullName(item.Person),
                                    Description = NomenclatureTranslator.MapTicketTypeTranslation(code),
                                    Price = item.Price,
                                });
                            }
                            await MainNavigator.Current.GoToPageAsync(nameof(HomePage));
                            await MainNavigator.Current.GoToPageAsync(new ProceedToPaymentPage(items, response.PaidTicketPaymentRequestNum));
                        }
                        else
                        {
                            await MainNavigator.Current.GoToPageAsync(nameof(HomePage));
                            await TLSnackbar.Show(TranslateExtension.Translator[nameof(GroupResourceEnum.FishingTicket) + "/TicketRequestSuccess"], System.Drawing.Color.Green);//Билетът беше успешно заявен!
                        }
                    }
                    else
                    {
                        await TLSnackbar.Show(TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/SaveFailed"], Color.Red);
                    }
                }
                else //Update
                {
                    bool success = await FishingTicketsTransaction.EditTicket(ticket);

                    if (success)
                    {
                        MessagingCenter.Instance.Send(new TicketsChangedArgs(TicketsChangedEnum.UpdatedTicket), TicketsChangedArgs.TICKETS_CHANGED_EVENT);

                        if (TicketMetadata.PaymentStatus == PaymentStatusEnum.NotNeeded || TicketMetadata.PaymentStatus == PaymentStatusEnum.PaidOK)
                        {
                            await MainNavigator.Current.GoToPageAsync(nameof(HomePage));
                            await TLSnackbar.Show(TranslateExtension.Translator[nameof(GroupResourceEnum.FishingTicket) + "/TicketEditSuccess"], System.Drawing.Color.Green);//Заявката за билет беше редактирана успешно!
                        }
                        else
                        {

                            ItemViewModel item = new ItemViewModel
                            {
                                Name = GetPersonFullName(ticket.Person),
                                Description = TicketMetadata.TypeName,
                                Price = TicketMetadata.Price,
                            };
                            await MainNavigator.Current.GoToPageAsync(nameof(HomePage));
                            await MainNavigator.Current.GoToPageAsync(new ProceedToPaymentPage(new List<ItemViewModel> { item }, TicketMetadata.PaymentRequestNum));
                        }
                    }
                    else
                    {
                        await TLSnackbar.Show(TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/SaveFailed"], Color.Red);
                    }
                }
            }
            finally
            {
                await TLLoadingHelper.HideFullLoadingScreen();
            }
        }

        private async Task UpdatePersonData()
        {
            if (!Person.IsUnder14)
            {
                UpdateTicketProfileDataDto data = new UpdateTicketProfileDataDto
                {
                    Person = Person.MapToApiPerson(),
                    UserAddresses = Person.MapToApiAddresses(),
                    Photo = photoFullPath != null ? new FileModel()
                    {
                        FullPath = photoFullPath,
                        ContentType = PhotoContentType,
                        UploadedOn = DateTime.Now
                    } : null,
                };

                await FishingTicketsTransaction.UpdateUserProfileData(data);
            }
        }

        protected async Task DisplayAttentionAlert(string message)
        {
            await App.Current.MainPage.DisplayAlert(TranslateExtension.Translator[nameof(GroupResourceEnum.TicketPerson) + "/Attention"],
                message,
                TranslateExtension.Translator[nameof(GroupResourceEnum.TicketPerson) + "/Ok"]);
        }

        protected async Task LoadInitialDataFromProfile()
        {
            await AutoCompletePersonData(Person);
            FileResponse result = await ProfileTransaction.GetUserPhoto();
            await FillUserPhoto(result);
        }

        protected async Task<FishingTicketResponseDto> LoadTicketBaseData(int ticketId)
        {
            FishingTicketResponseDto ticket = await FishingTicketsTransaction.GetTicketById(ticketId);
            if (ticket != null)
            {
                TicketNum = ticket.TicketNum;
                TerritorialUnit.Value = TerritorialUnits.Find(f => f.Id == ticket.DeliveryTerritoryUnitId);
                FillPersonData(Person, ticket.Person, ticket.PersonAddressRegistrations);

                if (ticket.PersonPhoto != null && ticket.PersonPhoto.Id != 0)
                {
                    FileResponse personPhoto = await FishingTicketsTransaction.GetPersonPhoto(ticket.PersonPhoto.Id, TicketMetadata.Id);
                    await FillUserPhoto(personPhoto, ticket.PersonPhoto.Id);
                }
            }

            return ticket;
        }

        protected async Task AutoCompletePersonData(PersonViewModel person)
        {
            ProfileApiDto userData = await ProfileTransaction.GetInfo();

            if (userData == null)
            {
                await MainNavigator.Current.PopPageAsync();
                return;
            }

            FillPersonData(person, userData, userData.UserAddresses);
        }

        protected void FillPersonData(PersonViewModel person, BasePersonInfoApiDto apiPerson, List<AddressRegistrationApiDto> apiPersonAddresses)
        {
            person.EgnLnc.Value = apiPerson.EgnLnc.EgnLnc;
            person.EgnLnc.IdentifierType = apiPerson.EgnLnc.IdentifierType;
            person.FirstName.Value = apiPerson.FirstName;
            person.MiddleName.Value = apiPerson.MiddleName;
            person.LastName.Value = apiPerson.LastName;
            person.HasBulgarianAddressRegistration = apiPerson.HasBulgarianAddressRegistration.GetValueOrDefault();
            if (apiPerson.Document != null)
            {
                if (apiPerson.EgnLnc.IdentifierType == IdentifierTypeEnum.EGN)
                {
                    person.BulgarianCitizen.Idcard.Value = apiPerson.Document.DocumentNumber;
                    person.BulgarianCitizen.IdcardDate.Value = apiPerson.Document.DocumentIssuedOn;
                    person.BulgarianCitizen.IdcardPublisher.Value = apiPerson.Document.DocumentIssuedBy;
                }
                else
                {
                    person.Foreigner.DocumentType.Value = person.Foreigner.DocumentTypes.Find(f => f.Id == apiPerson.Document.DocumentTypeId);
                    person.Foreigner.Citizenship.Value = person.Countries.Find(f => f.Id == apiPerson.CitizenshipCountryId);
                    person.Foreigner.Idcard.Value = apiPerson.Document.DocumentNumber;
                }
            }

            person.DateOfBirth.Value = apiPerson.BirthDate;

            if (apiPerson.GenderId.HasValue)
            {
                string genderCode = NomenclaturesTransaction.GetGenderCodeById(apiPerson.GenderId.Value);
                if ((genderCode == nameof(GenderEnum.F) || genderCode == nameof(GenderEnum.M)) && person.Genders?.Count > 0)
                {
                    person.Gender.Value = person.Genders.Find(f => f.Value == apiPerson.GenderId.Value);
                }
            }

            AddressRegistrationApiDto permanentAddress = apiPersonAddresses?.Find(f => f.AddressType == AddressType.PERMANENT);

            if (permanentAddress != null)
            {
                person.PermanentAddress.AssignAddressRegistration(permanentAddress,
                                                                    person.Countries,
                                                                    person.Districts,
                                                                    bulgarianAddressRequired: person.HasBulgarianAddressRegistration && TicketMetadata.Action != TicketAction.View);
            }

            AddressRegistrationApiDto correspondenceAddress = apiPersonAddresses?.Find(f => f.AddressType == AddressType.CORRESPONDENCE);

            if (correspondenceAddress != null)
            {
                person.CorrespondenceAddress.AssignAddressRegistration(correspondenceAddress, person.Countries, person.Districts);
                person.PermanentAddressMatchWithCorrespondence = false;
            }
        }

        protected async Task FillUserPhoto(FileResponse fileResponse, int? photoId = null)
        {
            if (fileResponse?.File != null && fileResponse?.Name != null && fileResponse?.ContentType != null)
            {
                photoFullPath = Path.Combine(FileSystem.CacheDirectory, fileResponse.Name);
                using (MemoryStream stream = new MemoryStream(fileResponse.File))
                using (FileStream newStream = File.OpenWrite(photoFullPath))
                {
                    await stream.CopyToAsync(newStream);
                }

                Photo = photoFullPath;
                PhotoContentType = fileResponse.ContentType;
                PhotoId = photoId;
            }
        }

        private string GetPersonFullName(BasePersonInfoApiDto person)
        {
            return person.FirstName + " " + (!string.IsNullOrEmpty(person.MiddleName) ? (person.MiddleName + " ") : null) + person.LastName;
        }

        private async Task DownloadTicketPdf()
        {
            bool accept = await App.Current.MainPage.DisplayAlert(null,
                TranslateExtension.Translator[nameof(GroupResourceEnum.FishingTicket) + "/AcceptTicketDownload"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Yes"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/No"]);

            if (accept)
            {
                IDownloader downloader = DependencyService.Resolve<IDownloader>();
                await downloader.DownloadFile(
                    $"Ticket{_ticketNum.Trim()}.pdf",
                    "application/pdf",
                    "FishingTickets/Download",
                    new
                    {
                        ticketId = TicketMetadata.Id
                    }
                );
            }
        }
    }
}