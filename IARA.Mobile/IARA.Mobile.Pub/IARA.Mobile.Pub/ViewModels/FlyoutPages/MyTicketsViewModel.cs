using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Pub.Application.DTObjects.FishingTickets.API;
using IARA.Mobile.Pub.Domain.Enums;
using IARA.Mobile.Pub.Utilities;
using IARA.Mobile.Pub.ViewModels.Base;
using IARA.Mobile.Pub.ViewModels.FlyoutPages.FishingTicket;
using IARA.Mobile.Pub.ViewModels.FlyoutPages.Payments;
using IARA.Mobile.Pub.Views.FlyoutPages.FishingTicket;
using IARA.Mobile.Pub.Views.FlyoutPages.Payments;
using IARA.Mobile.Shared.Menu;
using IARA.Mobile.Shared.ResourceTranslator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.ViewModels.FlyoutPages
{
    public class MyTicketsViewModel : MainPageViewModel
    {
        private TLObservableCollection<FishingTicketDto> _tickets;
        private TLObservableCollection<FishingTicketDto> _expiredTickets;
        private string _selectedState;
        private string _lastOpenedState;

        public MyTicketsViewModel()
        {
            Tickets = new TLObservableCollection<FishingTicketDto>();
            ExpiredTickets = new TLObservableCollection<FishingTicketDto>();
            ViewTicket = CommandBuilder.CreateFrom<FishingTicketDto>(OnViewTicket);
            RenewTicket = CommandBuilder.CreateFrom<FishingTicketDto>(OnRenewTicket);
            UpdateTicket = CommandBuilder.CreateFrom<FishingTicketDto>(OnUpdateTicket);
            ProceedToPayment = CommandBuilder.CreateFrom<FishingTicketDto>(OnProceedToPayment);
            StateChanged = CommandBuilder.CreateFrom(OnStateChanged);
        }

        public TLObservableCollection<FishingTicketDto> Tickets
        {
            get => _tickets;
            set => SetProperty(ref _tickets, value);
        }

        public TLObservableCollection<FishingTicketDto> ExpiredTickets
        {
            get => _expiredTickets;
            set => SetProperty(ref _expiredTickets, value);
        }

        public string SelectedState
        {
            get => _selectedState;
            set => SetProperty(ref _selectedState, value);
        }

        public ICommand ViewTicket { get; }

        public ICommand RenewTicket { get; }

        public ICommand UpdateTicket { get; }

        public ICommand ProceedToPayment { get; }

        public ICommand StateChanged { get; }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[] { GroupResourceEnum.FishingTicket, GroupResourceEnum.MyTickets };
        }

        public override async Task Initialize(object sender)
        {
            _lastOpenedState = nameof(Tickets);
            await GetStateTickets(_lastOpenedState);
            Translator.Current.PropertyChanged += OnTranslatorPropertyChanged;
        }

        public async Task GetStateTickets(string lastOpenedState, bool local = false)
        {
            SelectedState = null;//Required to show the loading screen
            IsBusy = true;//Required to show the loading screen

            List<FishingTicketDto> tickets = null;
            if (lastOpenedState == nameof(Tickets))
            {
                if (local)
                {
                    tickets = FishingTicketsTransaction.GetLocalStoredTickets();
                }
                else
                {
                    tickets = await FishingTicketsTransaction.GetMyTickets();
                }
            }
            else
            {
                tickets = FishingTicketsTransaction.GetLocalStoredExpiredTickets();
            }


            if (tickets != null && tickets.Count > 0)
            {
                NomenclatureTranslator.UpdateTicketTranslation(tickets, FishingTicketsTransaction);

                if (lastOpenedState == nameof(Tickets))
                {
                    Tickets.ReplaceRange(tickets);
                }
                else
                {
                    ExpiredTickets.ReplaceRange(tickets);
                }

                if (Device.RuntimePlatform == Device.Android)
                {
                    //Android rendering takes some time for each ticket, loading screen is on until full load;
                    await Task.Delay(tickets.Count * 10);
                }
            }

            IsBusy = false;
            SelectedState = _lastOpenedState;
        }
        private void OnTranslatorPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Tickets.Count > 0)
                Tickets.ReplaceRange(NomenclatureTranslator.UpdateTicketTranslation(Tickets.ToList(), FishingTicketsTransaction));

            if (ExpiredTickets.Count > 0)
                ExpiredTickets.ReplaceRange(NomenclatureTranslator.UpdateTicketTranslation(Tickets.ToList(), FishingTicketsTransaction));
        }
        private async Task OnStateChanged()
        {
            if (_lastOpenedState != SelectedState)//Prevent firing event twice(xamarin bug)
            {
                _lastOpenedState = SelectedState;
                await GetStateTickets(_lastOpenedState, local: true);
            }
        }

        private Task OnViewTicket(FishingTicketDto ticket)
        {
            return NavigateToTicket(ticket, TicketAction.View);
        }

        private Task OnRenewTicket(FishingTicketDto ticket)
        {
            return NavigateToTicket(ticket, TicketAction.Renew);
        }

        private async Task OnUpdateTicket(FishingTicketDto ticket)
        {
            if (ticket.ApplicationStatusCode == ApplicationStatuses.CORR_BY_USR_NEEDED)
            {
                ApplicationStatusChangeDto application = await ApplicationTransaction.InitiateApplicationCorrections(ticket.ApplicationId);
                ticket.ApplicationStatusCode = application.Status;
            }

            await NavigateToTicket(ticket, TicketAction.Update);
        }

        private async Task OnProceedToPayment(FishingTicketDto ticket)
        {
            await MainNavigator.Current.GoToPageAsync(new ProceedToPaymentPage(new List<ItemViewModel>
            {
                new ItemViewModel()
                {
                    Price = ticket.Price,
                    Name = ticket.PersonFullName,
                    Description = ticket.TypeName
                }
            },
            ticket.PaymentRequestNum));
        }

        private async Task NavigateToTicket(FishingTicketDto ticket, TicketAction action)
        {
            if (Enum.TryParse(ticket.Type, true, out TicketTypeEnum ticketType))
            {
                TicketMetadataModel metadata = new TicketMetadataModel
                {
                    Action = action,
                    Name = ticket.TypeName,
                    BadgeText = ticket.PeriodName.ToUpper() + (ticket.Price != default ? " - " + ticket.Price.ToString("F2") + TranslateExtension.Translator[nameof(GroupResourceEnum.FishingTicket) + "/Leva"] : null),
                    PeriodId = ticket.PeriodId,
                    PeriodCode = ticket.PeriodCode,
                    TypeId = ticket.TypeId,
                    TypeCode = ticket.Type,
                    TypeName = ticket.TypeName,
                    Id = ticket.Id,
                    Price = ticket.Price,
                    PaymentRequestNum = ticket.PaymentRequestNum,
                    ApplicationStatusCode = ticket.ApplicationStatusCode,
                    PaymentStatus = ticket.PaymentStatus,
                    TicketNumber = ticket.TicketNumber
                };

                if (action == TicketAction.Renew)
                {
                    //In case nomenclature is updated get the new id for both type and period;
                    metadata.TypeId = FishingTicketsTransaction.GetTicketTypeIdByCode(ticket.Type);
                    metadata.PeriodId = FishingTicketsTransaction.GetTicketPeriodIdByCode(ticket.PeriodCode);
                    //Check for new price
                    metadata.Price = FishingTicketsTransaction.GetTicketTariff(ticket.Type, ticket.PeriodCode);

                    if (metadata.Price != ticket.Price)
                    {
                        metadata.BadgeText = ticket.PeriodName.ToUpper() + (metadata.Price != default ? " - " + metadata.Price.ToString("F2") + TranslateExtension.Translator[nameof(GroupResourceEnum.FishingTicket) + "/Leva"] : null);
                    }
                }

                switch (ticketType)
                {
                    case TicketTypeEnum.STANDARD:
                        await MainNavigator.Current.GoToPageAsync(new StandardTicketPage(metadata));
                        break;
                    case TicketTypeEnum.BETWEEN14AND18:
                        await MainNavigator.Current.GoToPageAsync(new Between14and18TicketPage(metadata));
                        break;
                    case TicketTypeEnum.ELDER:
                        await MainNavigator.Current.GoToPageAsync(new ElderTicketPage(metadata));
                        break;
                    case TicketTypeEnum.ASSOCIATION:
                        await MainNavigator.Current.GoToPageAsync(new AssociationTicketPage(metadata));
                        break;
                    case TicketTypeEnum.DISABILITY:
                        await MainNavigator.Current.GoToPageAsync(new DisabilityTicketPage(metadata));
                        break;
                    case TicketTypeEnum.UNDER14:
                        await MainNavigator.Current.GoToPageAsync(new Under14TicketPage(metadata));
                        break;
                    case TicketTypeEnum.BETWEEN14AND18ASSOCIATION:
                        await MainNavigator.Current.GoToPageAsync(new Between14and18AssociationTicketPage(metadata));
                        break;
                    case TicketTypeEnum.ELDERASSOCIATION:
                        await MainNavigator.Current.GoToPageAsync(new ElderAssociationTicketPage(metadata));
                        break;
                }
            }
        }
    }
}