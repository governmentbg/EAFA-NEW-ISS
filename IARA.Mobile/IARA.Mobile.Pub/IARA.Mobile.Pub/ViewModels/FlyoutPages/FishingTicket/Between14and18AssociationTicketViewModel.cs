using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Pub.Application.DTObjects.FishingTickets.API;
using IARA.Mobile.Pub.Application.DTObjects.FishingTickets.LocalDb;
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
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Pub.ViewModels.FlyoutPages.FishingTicket
{
    public class Between14and18AssociationTicketViewModel : BaseTicketViewModel
    {
        private DateTime _ticketValidFrom;
        private string _ticketValidTo;
        private ValidState _membershipCardId;
        private ValidStateSelect<AssociationSelectDto> _selectedAssociation;
        private List<AssociationSelectDto> _associations;
        private ValidStateDate _membershipCardCreatedOn;
        private TLFileResult _membershipCardFile;

        public Between14and18AssociationTicketViewModel(IFishingTicketsSettings fishingTicketsSettings)
           : base(fishingTicketsSettings)
        {
            OnSaveCommand = CommandBuilder.CreateFrom(OnSave);
            UploadMembershipCardCommand = CommandBuilder.CreateFrom(UploadMembershipCard);
            ValidFromChanged = CommandBuilder.CreateFrom<DateTime>(OnValidFromChanged);
            TicketValidFrom = DateTime.Now;

        }

        public DateTime TicketValidFrom
        {
            get => _ticketValidFrom;
            set => SetProperty(ref _ticketValidFrom, value);
        }

        public string TicketValidTo
        {
            get => _ticketValidTo;
            set => SetProperty(ref _ticketValidTo, value);
        }

        [Required]
        public ValidStateSelect<AssociationSelectDto> SelectedAssociation
        {
            get => _selectedAssociation;
            set => SetProperty(ref _selectedAssociation, value);
        }

        public List<AssociationSelectDto> Associations
        {
            get => _associations;
            set => SetProperty(ref _associations, value);
        }

        [Required]
        [StringLength(50)]
        public ValidState MembershipCardId
        {
            get => _membershipCardId;
            set => SetProperty(ref _membershipCardId, value);
        }

        [Required]
        public ValidStateDate MembershipCardCreatedOn
        {
            get => _membershipCardCreatedOn;
            set => SetProperty(ref _membershipCardCreatedOn, value);
        }
        public int? MembershipCardFileId { get; set; }
        public bool IsNewMembershipCard { get; set; }
        public TLFileResult MembershipCardFile
        {
            get => _membershipCardFile;
            set => SetProperty(ref _membershipCardFile, value);
        }

        public DateTime Today => DateTime.Today;
        public ICommand UploadMembershipCardCommand { get; }

        public ICommand ValidFromChanged { get; }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[] {
                GroupResourceEnum.FishingTicket,
                GroupResourceEnum.TicketPerson,
                GroupResourceEnum.Validation,
                GroupResourceEnum.Common,
                GroupResourceEnum.Under14Ticket,
                GroupResourceEnum.AssociationTicket,
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
            Associations = await FishingTicketsTransaction.GetFishingAssiciations();
            this.AddValidation(new Dictionary<string, Func<bool>>()
            {
                { Group.ACCEPT_AGREEMENT, () =>  true},
            }, others: new[] { Person });
            if (TicketMetadata.Action == TicketAction.Create)
            {
                await OnValidFromChanged(TicketValidFrom);
                await LoadInitialDataFromProfile();

            }
            else if (TicketMetadata.Action == TicketAction.Renew || TicketMetadata.Action == TicketAction.Update || TicketMetadata.Action == TicketAction.View)
            {
                FishingTicketResponseDto ticket = await LoadTicketBaseData(TicketMetadata.Id);

                if (TicketMetadata.Action == TicketAction.Update || TicketMetadata.Action == TicketAction.View)
                {
                    AllowAdditionalUnder14Tickets = false;

                    if (ticket == null)
                    {
                        await MainNavigator.Current.PopPageAsync();
                    }
                    else
                    {
                        TicketValidFrom = ticket.ValidFrom;
                        if (ticket.MembershipCard != null)
                        {
                            SelectedAssociation.Value = new AssociationSelectDto()
                            {
                                Id = ticket.MembershipCard.AssociationId.Value,
                                Name = Associations.First(x => x.Id == ticket.MembershipCard.AssociationId.Value).Name
                            };
                            MembershipCardId.Value = ticket.MembershipCard.CardNum;
                            MembershipCardCreatedOn.Value = ticket.MembershipCard.IssueDate;

                            List<int> membershipCardTypeIds = NomenclaturesTransaction.GetAllFileTypeIdsByCode(nameof(FileTypeEnum.MEMBERSHIPCARD));
                            FileModel membershipCardFile = ticket.Files.Find(x => membershipCardTypeIds.Contains(x.FileTypeId));
                            if (membershipCardFile != null)
                            {
                                MembershipCardFileId = membershipCardFile.Id;
                                MembershipCardFile = new TLFileResult("/" + membershipCardFile.Name, membershipCardFile.Name, membershipCardFile.ContentType, membershipCardFile.Size);
                            }
                        }

                        await OnValidFromChanged(TicketValidFrom);
                    }
                }
            }
            await TLLoadingHelper.HideFullLoadingScreen();
        }

        private async void UploadMembershipCard()
        {
            TLFileResult file = await TLMediaPicker.PickPhotoAsync();
            if (file == null)
            {
                return;
            }
            MembershipCardFile = file.ImageResize(ImageResizeConstants.MAX_WIDTH, ImageResizeConstants.MAX_HEIGTH, ImageResizeConstants.COMPRESSION_RATE);
            IsNewMembershipCard = true;
        }

        protected override FishingTicketRequestDto GetFishingTicketData()
        {
            FishingTicketRequestDto ticketRequest = new FishingTicketRequestDto()
            {
                Id = TicketMetadata.Id,
                TypeId = TicketMetadata.TypeId,
                PeriodId = TicketMetadata.PeriodId,
                Price = TicketMetadata.Price,
                ValidFrom = TicketValidFrom.Date == DateTime.Now.Date ? DateTime.Now : TicketValidFrom,
                Person = Person.MapToApiPerson(),
                PersonAddressRegistrations = Person.MapToApiAddresses(),
                HasUserConfirmed = AcceptAgreement,
                DeliveryTerritoryUnitId = TerritorialUnit.Value.Id,
                PersonPhoto = photoFullPath != null ? new FileModel()
                {
                    FullPath = photoFullPath,
                    ContentType = PhotoContentType,
                    UploadedOn = DateTime.Now
                } : null,
                MembershipCard = new MembershipCardDto()
                {
                    AssociationId = SelectedAssociation.Value.Id,
                    CardNum = MembershipCardId.Value,
                    IssueDate = MembershipCardCreatedOn.Value,
                }
            };

            int fileTypeId = NomenclaturesTransaction.GetActiveFileTypeIdByCode(nameof(FileTypeEnum.MEMBERSHIPCARD));
            if (TicketMetadata.Action == TicketAction.Create || TicketMetadata.Action == TicketAction.Renew)
            {
                ticketRequest.Files = new List<FileModel>()
                {
                    new FileModel()
                    {
                        FullPath = MembershipCardFile.FullPath,
                        ContentType = MembershipCardFile.ContentType,
                        UploadedOn = DateTime.Now,
                        FileTypeId = fileTypeId
                    }
                };
            }
            else if (IsNewMembershipCard)
            {
                ticketRequest.Files = new List<FileModel>()
                {
                    new FileModel()
                    {
                        FullPath = MembershipCardFile.FullPath,
                        ContentType = MembershipCardFile.ContentType,
                        UploadedOn = DateTime.Now,
                        FileTypeId = fileTypeId
                    },
                    new FileModel()
                    {
                         Id = MembershipCardFileId,
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
                PeriodCode = TicketMetadata.PeriodCode,
                TypeCode = nameof(TicketTypeEnum.BETWEEN14AND18ASSOCIATION),
                ValidFrom = TicketValidFrom.Date == DateTime.Now.Date ? DateTime.Now : TicketValidFrom,
            };
        }

        protected override async Task<bool> CustomValidation()
        {
            int personYears = YearsCalculator.YearsDifferenceFromNow(Person.DateOfBirth.Value.Value);
            if (personYears < 14 || personYears >= 18)
            {
                await DisplayAttentionAlert(TranslateExtension.Translator[nameof(GroupResourceEnum.FishingTicket) + "/Between14and18Only"]);
                return true;
            }

            if (MembershipCardFile == null)
            {
                await DisplayAttentionAlert(TranslateExtension.Translator[nameof(GroupResourceEnum.TicketPerson) + "/ShouldUploadMembershipCard"]);
                return true;
            }

            return false;
        }

        private async Task OnValidFromChanged(DateTime validFrom)
        {
            TicketValidToCalculationParamsDto parameters = new TicketValidToCalculationParamsDto
            {
                TypeId = TicketMetadata.TypeId,
                PeriodId = TicketMetadata.PeriodId,
                ValidFrom = validFrom,
            };

            DateTime? result = await FishingTicketsTransaction.CalculateTicketValidToDate(parameters);

            TicketValidTo = result.HasValue ? result.GetValueOrDefault().ToString(ValidToDateTimeFormat) : null;
        }
    }
}

