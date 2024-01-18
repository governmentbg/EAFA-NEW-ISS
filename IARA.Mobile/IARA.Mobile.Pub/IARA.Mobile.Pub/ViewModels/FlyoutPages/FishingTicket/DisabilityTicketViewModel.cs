using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Pub.Application.DTObjects.FishingTickets.API;
using IARA.Mobile.Pub.Application.Interfaces.Utilities;
using IARA.Mobile.Pub.Domain.Enums;
using IARA.Mobile.Pub.ViewModels.Controls.PersonView;
using IARA.Mobile.Shared.Menu;
using TechnoLogica.Xamarin.Attributes;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Pub.ViewModels.FlyoutPages.FishingTicket
{
    public class DisabilityTicketViewModel : BaseTicketViewModel
    {
        private ValidState _medicalЕxpertiseNumber;
        private bool _permanentMedicalExpertise;
        private ValidStateDate _medicalЕxpertiseValidTo;

        public DisabilityTicketViewModel(IFishingTicketsSettings fishingTicketsSettings)
            : base(fishingTicketsSettings)
        {
            OnSaveCommand = CommandBuilder.CreateFrom(OnSave);
        }

        [Required]
        [StringLength(50)]
        public ValidState МedicalЕxpertiseNumber
        {
            get => _medicalЕxpertiseNumber;
            set => SetProperty(ref _medicalЕxpertiseNumber, value);
        }

        public bool PermanentMedicalExpertise
        {
            get => _permanentMedicalExpertise;
            set => SetProperty(ref _permanentMedicalExpertise, value);
        }

        [Required]
        [ValidGroup(Group.NON_PERMANENT_EXPERTISE)]
        public ValidStateDate МedicalЕxpertiseValidTo
        {
            get => _medicalЕxpertiseValidTo;
            set => SetProperty(ref _medicalЕxpertiseValidTo, value);
        }

        public DateTime Today => DateTime.Today;

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[] {
                GroupResourceEnum.DisabilityTicket,
                GroupResourceEnum.FishingTicket,
                GroupResourceEnum.TicketPerson,
                GroupResourceEnum.Validation,
                GroupResourceEnum.Common,
                GroupResourceEnum.Under14Ticket
            };
        }

        public override async Task Initialize(object sender)
        {
            await base.Initialize(sender);
            await TLLoadingHelper.ShowFullLoadingScreen();
            Person = new PersonViewModel();
            this.AddValidation(new Dictionary<string, Func<bool>>()
            {
                { Group.NON_PERMANENT_EXPERTISE, () =>  !PermanentMedicalExpertise},
                { Group.ACCEPT_AGREEMENT, () =>  true},
            }, others: new[] { Person });
            if (TicketMetadata.Action == TicketAction.Create)
            {
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
                    else if (ticket.TelkData != null)
                    {
                        МedicalЕxpertiseNumber.Value = ticket.TelkData.Num;
                        МedicalЕxpertiseValidTo.Value = ticket.TelkData.ValidTo;
                        PermanentMedicalExpertise = ticket.TelkData.IsIndefinite.GetValueOrDefault();
                    }
                }
            }
            await TLLoadingHelper.HideFullLoadingScreen();
        }

        protected override FishingTicketRequestDto GetFishingTicketData()
        {
            int ticketPeriodId = this.FishingTicketsTransaction.GetTicketPeriodIdByCode(PermanentMedicalExpertise ? nameof(TicketPeriodEnum.NOPERIOD) : nameof(TicketPeriodEnum.DISABILITY));
            return new FishingTicketRequestDto()
            {
                Id = TicketMetadata.Id,
                TypeId = TicketMetadata.TypeId,
                PeriodId = ticketPeriodId,
                Price = TicketMetadata.Price,
                Person = Person.MapToApiPerson(),
                PersonAddressRegistrations = Person.MapToApiAddresses(),
                ValidFrom = DateTime.Now,
                HasUserConfirmed = AcceptAgreement,
                DeliveryTerritoryUnitId = TerritorialUnit.Value.Id,
                PersonPhoto = photoFullPath != null ? new FileModel()
                {
                    FullPath = photoFullPath,
                    ContentType = PhotoContentType,
                    UploadedOn = DateTime.Now
                } : null,
                TelkData = new TelkReviewDto()
                {
                    IsIndefinite = PermanentMedicalExpertise,
                    Num = МedicalЕxpertiseNumber.Value,
                    ValidTo = МedicalЕxpertiseValidTo.Value
                }
            };
        }

        protected override TicketValidationDTO GetTicketValidationData()
        {
            return new TicketValidationDTO()
            {
                EgnLnch = Person.EgnLnc.Value,
                PeriodCode = PermanentMedicalExpertise ? nameof(TicketPeriodEnum.NOPERIOD) : nameof(TicketPeriodEnum.DISABILITY),
                TypeCode = nameof(TicketTypeEnum.DISABILITY),
                TELKIsIndefinite = PermanentMedicalExpertise,
                TELKValidTo = МedicalЕxpertiseValidTo.Value,
            };
        }
    }
}

