using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Pub.Application.DTObjects.FishingTickets.LocalDb;
using IARA.Mobile.Pub.Domain.Enums;
using IARA.Mobile.Pub.ViewModels.Base;
using IARA.Mobile.Pub.ViewModels.FlyoutPages.FishingTicket;
using IARA.Mobile.Pub.Views.FlyoutPages;
using IARA.Mobile.Pub.Views.FlyoutPages.FishingTicket;
using IARA.Mobile.Shared.Menu;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;

namespace IARA.Mobile.Pub.ViewModels.FlyoutPages
{
    public class FishingTicketViewModel : PageViewModel
    {
        public static string TicketTypeState = nameof(TicketTypeState);
        public static string TicketPriceState = nameof(TicketPriceState);
        public static string Currency;
        private readonly IBackButton _backButton;
        private string _currentState;

        public FishingTicketViewModel(IBackButton backButton)
        {
            _backButton = backButton;
            TicketTypes = new TLObservableCollection<TicketTypeDto>();
            TicketTariffsByType = new TLObservableCollection<TicketTariffViewModel>();
            GoToSecondStateCommand = CommandBuilder.CreateFrom<TicketTypeDto>(GoToSecondState);
            GoToTicketPageCommand = CommandBuilder.CreateFrom<TicketTariffViewModel>(GoToTicketPage);
        }

        public TicketTypeDto SelectedTicketType { get; set; }
        public List<TicketPeriodDto> TicketPeriods { get; set; }
        public TLObservableCollection<TicketTypeDto> TicketTypes { get; }
        public TLObservableCollection<TicketTariffViewModel> TicketTariffsByType { get; }

        public string CurrentState
        {
            get => _currentState;
            set
            {
                _currentState = value;
                OnPropertyChanged(nameof(CurrentState));
            }
        }

        public ICommand GoToSecondStateCommand { get; }
        public ICommand GoToTicketPageCommand { get; }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[] { GroupResourceEnum.FishingTicket };
        }

        public override async Task Initialize(object sender)
        {
            List<TicketTypeDto> ticketTypes = await FishingTicketsTransaction.GetAllowedTicketTypes();
            TicketTypes.AddRange(ticketTypes);
            TicketPeriods = FishingTicketsTransaction.GetTicketPeriods();
        }

        public override void OnAppearing()
        {
            CurrentState = TicketTypeState;
            _backButton.PreventBackButtonPress = true;
            _backButton.BackButtonPressed += BackButton_BackButtonPressed;
            Currency = TranslateExtension.Translator[nameof(GroupResourceEnum.FishingTicket) + "/Leva"];
        }

        public override void OnDisappearing()
        {
            _backButton.PreventBackButtonPress = false;
            _backButton.BackButtonPressed -= BackButton_BackButtonPressed;
        }

        private void BackButton_BackButtonPressed(object sender, EventArgs e)
        {
            if (CurrentState != TicketTypeState)
            {
                CurrentState = TicketTypeState;
            }
            else
            {
                MainNavigator.Current.GoToPageAsync(nameof(HomePage));
            }
        }

        private Task GoToSecondState(TicketTypeDto ticketType)
        {
            SelectedTicketType = ticketType;

            if (Enum.TryParse(ticketType.Code, true, out TicketTypeEnum ticketTypeEnum))
            {

                if (ticketTypeEnum == TicketTypeEnum.UNDER14 || ticketTypeEnum == TicketTypeEnum.DISABILITY)
                {
                    string noPeriodCode = ticketTypeEnum == TicketTypeEnum.UNDER14 ? nameof(TicketPeriodEnum.UNTIL14) : nameof(TicketPeriodEnum.DISABILITY);
                    int periodId = FishingTicketsTransaction.GetTicketPeriodIdByCode(noPeriodCode);
                    TicketMetadataModel metadata = new TicketMetadataModel()
                    {
                        Action = TicketAction.Create,
                        Name = SelectedTicketType.Name,
                        PeriodId = periodId,
                        PeriodCode = noPeriodCode,
                        TypeId = ticketType.Id,
                        TypeCode = ticketType.Code,
                        PaymentStatus = (ticketTypeEnum == TicketTypeEnum.UNDER14 || ticketTypeEnum == TicketTypeEnum.DISABILITY) ? PaymentStatusEnum.NotNeeded : PaymentStatusEnum.Unpaid
                    };

                    if (ticketTypeEnum == TicketTypeEnum.UNDER14)
                    {
                        return MainNavigator.Current.GoToPageAsync(new Under14TicketPage(metadata));
                    }
                    else if (ticketTypeEnum == TicketTypeEnum.DISABILITY)
                    {
                        return MainNavigator.Current.GoToPageAsync(new DisabilityTicketPage(metadata));
                    }
                }

                TicketTariffsByType.Clear();
                List<TicketTariffDto> tariffs = FishingTicketsTransaction.GetTicketTariffsByTicketType(ticketType.Code);
                foreach (TicketTariffDto tariff in tariffs)
                {
                    //e.g. Ticket_between14and18_weekly
                    string tariffPeriod = tariff.Code.Split('_')[2];

                    TicketPeriodDto period = TicketPeriods.FirstOrDefault(x => x.Code.ToLower() == tariffPeriod);
                    if (period != null)
                    {
                        TicketTariffsByType.Add(new TicketTariffViewModel()
                        {
                            Price = tariff.Price,
                            PeriodCode = period.Code,
                            PeriodId = period.Id,
                            PeriodName = period.Name,
                            TariffName = tariff.Name,
                        });
                    }
                }
                CurrentState = TicketPriceState;
            }

            return Task.CompletedTask;
        }

        private Task GoToTicketPage(TicketTariffViewModel ticketTariff)
        {
            if (Enum.TryParse(SelectedTicketType.Code, true, out TicketTypeEnum ticketTypeEnum))
            {
                TicketMetadataModel metadata = new TicketMetadataModel()
                {
                    Action = TicketAction.Create,
                    Name = SelectedTicketType.Name,
                    BadgeText = ticketTariff.PeriodName + " - " + ticketTariff.Price.ToString("F2") + TranslateExtension.Translator[nameof(GroupResourceEnum.FishingTicket) + "/Leva"],
                    PeriodId = ticketTariff.PeriodId,
                    PeriodCode = ticketTariff.PeriodCode,
                    TypeId = SelectedTicketType.Id,
                    TypeCode = SelectedTicketType.Code,
                    Price = ticketTariff.Price,
                    PaymentStatus = (ticketTypeEnum == TicketTypeEnum.UNDER14 || ticketTypeEnum == TicketTypeEnum.DISABILITY) ? PaymentStatusEnum.NotNeeded : PaymentStatusEnum.Unpaid
                };

                switch (ticketTypeEnum)
                {
                    case TicketTypeEnum.STANDARD:
                        return MainNavigator.Current.GoToPageAsync(new StandardTicketPage(metadata));
                    case TicketTypeEnum.BETWEEN14AND18:
                        return MainNavigator.Current.GoToPageAsync(new Between14and18TicketPage(metadata));
                    case TicketTypeEnum.ELDER:
                        return MainNavigator.Current.GoToPageAsync(new ElderTicketPage(metadata));
                    case TicketTypeEnum.ASSOCIATION:
                        return MainNavigator.Current.GoToPageAsync(new AssociationTicketPage(metadata));
                    case TicketTypeEnum.BETWEEN14AND18ASSOCIATION:
                        return MainNavigator.Current.GoToPageAsync(new Between14and18AssociationTicketPage(metadata));
                    case TicketTypeEnum.ELDERASSOCIATION:
                        return MainNavigator.Current.GoToPageAsync(new ElderAssociationTicketPage(metadata));
                }
            }
            return Task.CompletedTask;
        }
    }
}
