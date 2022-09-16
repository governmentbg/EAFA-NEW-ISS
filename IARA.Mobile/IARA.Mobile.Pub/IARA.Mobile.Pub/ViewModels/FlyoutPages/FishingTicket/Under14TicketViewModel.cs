using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Pub.Application.DTObjects.FishingTickets.API;
using IARA.Mobile.Pub.Application.Helpers;
using IARA.Mobile.Pub.Application.Interfaces.Utilities;
using IARA.Mobile.Pub.Domain.Enums;
using IARA.Mobile.Pub.ViewModels.Controls.PersonView;
using IARA.Mobile.Shared;
using IARA.Mobile.Shared.Helpers;
using IARA.Mobile.Shared.Menu;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Pub.ViewModels.FlyoutPages.FishingTicket
{
    public class Under14TicketViewModel : BaseTicketViewModel
    {
        private PersonViewModel _representativePerson;
        private TLFileResult birthCertificateFile;

        public Under14TicketViewModel(IFishingTicketsSettings fishingTicketsSettings)
            : base(fishingTicketsSettings)
        {
            UploadBirthCertificateCommand = CommandBuilder.CreateFrom(UploadBirthCertificate);
            OnSaveCommand = CommandBuilder.CreateFrom(OnSave);
            fishingTicketsSettings.AllowedUnder14TicketsCount--;
            AllowAdditionalUnder14Tickets = fishingTicketsSettings.AllowedUnder14TicketsCount > 0;
        }

        public PersonViewModel RepresentativePerson
        {
            get => _representativePerson;
            set => SetProperty(ref _representativePerson, value);
        }
        public bool IsNewBirthCertificateFile { get; set; }
        public int? BirthCertificateFileId { get; set; }
        public TLFileResult BirthCertificateFile
        {
            get => birthCertificateFile;
            set => SetProperty(ref birthCertificateFile, value);
        }

        public ICommand UploadBirthCertificateCommand { get; }

        private async Task UploadBirthCertificate()
        {
            TLFileResult file = await TLMediaPicker.PickPhotoAsync();
            if (file == null)
            {
                return;
            }
            IsNewBirthCertificateFile = true;
            BirthCertificateFile = file.ImageResize(ImageResizeConstants.MAX_WIDTH, ImageResizeConstants.MAX_HEIGTH, ImageResizeConstants.COMPRESSION_RATE);
        }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[] {
                GroupResourceEnum.Under14Ticket,
                GroupResourceEnum.FishingTicket,
                GroupResourceEnum.TicketPerson,
                GroupResourceEnum.Validation,
                GroupResourceEnum.Common
            };
        }

        public override async Task Initialize(object sender)
        {
            await base.Initialize(sender);
            await TLLoadingHelper.ShowFullLoadingScreen();
            Person = new PersonViewModel()
            {
                DateOfBirthRequired = true,
            };
            Person.IsUnder14.Value = true;
            RepresentativePerson = new PersonViewModel();

            this.AddValidation(new Dictionary<string, Func<bool>>()
            {
                { Group.ACCEPT_AGREEMENT, () =>  true},
            }, others: new IValidatableViewModel[] { Person, RepresentativePerson });

            if (TicketMetadata.Action == TicketAction.Create)
            {
                await AutoCompletePersonData(RepresentativePerson);
            }
            else if (TicketMetadata.Action == TicketAction.Renew)
            {
                FishingTicketResponseDto ticket = await LoadTicketBaseData(TicketMetadata.Id);

                if (ticket != null)
                {
                    FillPersonData(RepresentativePerson, ticket.RepresentativePerson, ticket.RepresentativePersonAddressRegistrations);
                }
            }
            else if (TicketMetadata.Action == TicketAction.Update || TicketMetadata.Action == TicketAction.View)
            {
                AllowAdditionalUnder14Tickets = false;
                FishingTicketResponseDto ticket = await LoadTicketBaseData(TicketMetadata.Id);

                if (ticket == null)
                {
                    await MainNavigator.Current.PopPageAsync();
                }
                else
                {
                    FillPersonData(RepresentativePerson, ticket.RepresentativePerson, ticket.RepresentativePersonAddressRegistrations);
                    List<int> birthCertificateTypeIds = NomenclaturesTransaction.GetAllFileTypeIdsByCode(nameof(FileTypeEnum.BIRTHCERTIFICATE));
                    FileModel birthCertificateFile = ticket.Files.FirstOrDefault(x => birthCertificateTypeIds.Contains(x.FileTypeId));
                    if (birthCertificateFile != null)
                    {
                        BirthCertificateFileId = birthCertificateFile.Id;
                        BirthCertificateFile = new TLFileResult("/" + birthCertificateFile.Name, birthCertificateFile.Name, birthCertificateFile.ContentType, birthCertificateFile.Size);
                    }
                }
            }
            await TLLoadingHelper.HideFullLoadingScreen();
        }

        protected override FishingTicketRequestDto GetFishingTicketData()
        {
            FishingTicketRequestDto ticketRequest = new FishingTicketRequestDto()
            {
                Id = TicketMetadata.Id,
                TypeId = TicketMetadata.TypeId,
                PeriodId = TicketMetadata.PeriodId,
                Price = TicketMetadata.Price,
                Person = Person.MapToApiPerson(),
                PersonAddressRegistrations = Person.MapToApiAddresses(),
                RepresentativePerson = RepresentativePerson.MapToApiPerson(),
                RepresentativePersonAddressRegistrations = RepresentativePerson.MapToApiAddresses(),
                ValidFrom = DateTime.Now,
                HasUserConfirmed = AcceptAgreement,
                PersonPhoto = photoFullPath != null ? new FileModel()
                {
                    FullPath = photoFullPath,
                    ContentType = PhotoContentType,
                    UploadedOn = DateTime.Now
                } : null,
            };

            int fileTypeId = NomenclaturesTransaction.GetActiveFileTypeIdByCode(nameof(FileTypeEnum.BIRTHCERTIFICATE));
            if (TicketMetadata.Action == TicketAction.Create || TicketMetadata.Action == TicketAction.Renew)
            {
                ticketRequest.Files = new List<FileModel>()
                {
                    new FileModel()
                    {
                        FullPath = BirthCertificateFile.FullPath,
                        ContentType = BirthCertificateFile.ContentType,
                        UploadedOn = DateTime.Now,
                        FileTypeId = fileTypeId
                    }
                };
            }
            else if (IsNewBirthCertificateFile)
            {
                ticketRequest.Files = new List<FileModel>()
                {
                    new FileModel()
                    {
                        FullPath = BirthCertificateFile.FullPath,
                        ContentType = BirthCertificateFile.ContentType,
                        UploadedOn = DateTime.Now,
                        FileTypeId = fileTypeId
                    },
                    new FileModel()
                    {
                         Id = BirthCertificateFileId,
                         Deleted = true,
                    }
                };
            }

            return ticketRequest;
        }

        protected override TicketValidationDTO GetTicketValidationData()
        {
            return new TicketValidationDTO()
            {
                EgnLnch = Person.EgnLnc.Value,
                PeriodCode = nameof(TicketPeriodEnum.UNTIL14),
                TypeCode = nameof(TicketTypeEnum.UNDER14),
                ChildDateOfBirth = Person.DateOfBirth.Value
            };
        }

        protected override async Task<bool> CustomValidation()
        {
            if (Photo == null)
            {
                await DisplayAttentionAlert(TranslateExtension.Translator[nameof(GroupResourceEnum.TicketPerson) + "/ShouldUploadPhoto"]);
                return true;
            }

            if (YearsCalculator.YearsDifferenceFromNow(Person.DateOfBirth.Value.Value) >= 14)
            {
                await DisplayAttentionAlert(TranslateExtension.Translator[nameof(GroupResourceEnum.Under14Ticket) + "/Under14Only"]);
                return true;
            }

            if (BirthCertificateFile == null)
            {
                await DisplayAttentionAlert(TranslateExtension.Translator[nameof(GroupResourceEnum.TicketPerson) + "/BirthCertificateIsRequired"]);
                return true;
            }

            //Prevent UK_Persons exception on save
            if (!string.IsNullOrEmpty(Person.EgnLnc.Value) && Person.EgnLnc.Value == RepresentativePerson.EgnLnc.Value)
            {
                await DisplayAttentionAlert(TranslateExtension.Translator[nameof(GroupResourceEnum.TicketPerson) + "/EgnsMatching"]);
                return true;
            }

            return false;
        }
    }
}


