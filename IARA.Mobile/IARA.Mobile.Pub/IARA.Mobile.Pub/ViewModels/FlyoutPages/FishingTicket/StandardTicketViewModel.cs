using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Pub.Application.DTObjects.FishingTickets.API;
using IARA.Mobile.Pub.Application.Interfaces.Utilities;
using IARA.Mobile.Pub.Domain.Enums;
using IARA.Mobile.Pub.ViewModels.Controls.PersonView;
using IARA.Mobile.Shared.Menu;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;

namespace IARA.Mobile.Pub.ViewModels.FlyoutPages.FishingTicket
{
    public class StandardTicketViewModel : BaseTicketViewModel
    {
        private DateTime _ticketValidFrom;
        private string _ticketValidTo;

        public StandardTicketViewModel(IFishingTicketsSettings fishingTicketsSettings)
            : base(fishingTicketsSettings)
        {
            TicketValidFrom = DateTime.Now;
            OnSaveCommand = CommandBuilder.CreateFrom(OnSave);
            ValidFromChanged = CommandBuilder.CreateFrom<DateTime>(OnValidFromChanged);
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

        public DateTime Today => DateTime.Today;
        public ICommand ValidFromChanged { get; }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[] {
                GroupResourceEnum.FishingTicket,
                GroupResourceEnum.TicketPerson,
                GroupResourceEnum.Under14Ticket,
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
                        await OnValidFromChanged(TicketValidFrom);
                    }
                }
            }
            await TLLoadingHelper.HideFullLoadingScreen();
        }

        protected override FishingTicketRequestDto GetFishingTicketData()
        {
            return new FishingTicketRequestDto()
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
            };
        }

        protected override TicketValidationDTO GetTicketValidationData()
        {
            return new TicketValidationDTO()
            {
                EgnLnch = Person.EgnLnc.Value,
                PeriodCode = TicketMetadata.PeriodCode,
                TypeCode = nameof(TicketTypeEnum.STANDARD),
                ValidFrom = TicketValidFrom.Date == DateTime.Now.Date ? DateTime.Now : TicketValidFrom,
            };
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
