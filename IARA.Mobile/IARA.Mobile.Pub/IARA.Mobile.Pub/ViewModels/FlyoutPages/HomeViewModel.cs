using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Pub.Application.DTObjects.FishingTickets.API;
using IARA.Mobile.Pub.Application.DTObjects.FishingTickets.LocalDb;
using IARA.Mobile.Pub.Utilities;
using IARA.Mobile.Pub.ViewModels.Base;
using IARA.Mobile.Pub.ViewModels.FlyoutPages.FishingTicket;
using IARA.Mobile.Shared.Menu;
using IARA.Mobile.Shared.ResourceTranslator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.ViewModels.FlyoutPages
{
    public class HomeViewModel : PageViewModel, IDisposable
    {
        private readonly IBackButton _backButton;
        private LayoutState _ticketsState;
        private bool _hasMoreTickets;
        private MyTicketsViewModel _tickets;
        private string _totalTicketsCount;

        public HomeViewModel(IBackButton backButton, ICurrentUser currentUser)
        {
            _ticketsState = LayoutState.Loading;

            Navigate = CommandBuilder.CreateFrom<string>(OnNavigate);
            _backButton = backButton;
            MyTickets = new MyTicketsViewModel();

            Permissions = NomenclaturesTransaction.GetPermissions();
        }

        public List<string> Permissions { get; }

        public LayoutState TicketsState
        {
            get => _ticketsState;
            set => SetProperty(ref _ticketsState, value);
        }

        public string TotalTicketsCount
        {
            get => _totalTicketsCount;
            set => SetProperty(ref _totalTicketsCount, value);
        }

        public bool HasMoreTickets
        {
            get => _hasMoreTickets;
            set => SetProperty(ref _hasMoreTickets, value);
        }

        public MyTicketsViewModel MyTickets
        {
            get => _tickets;
            set => SetProperty(ref _tickets, value);
        }

        public ICommand Navigate { get; }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[] { GroupResourceEnum.Home, GroupResourceEnum.FishingTicket };
        }

        public override void OnAppearing()
        {
            _backButton.CloseAppOnBackButtonPressed = true;
        }

        public override void OnDisappearing()
        {
            _backButton.CloseAppOnBackButtonPressed = false;
        }

        public override async Task Initialize(object sender)
        {
            List<FishingTicketDto> tickets = await FishingTicketsTransaction.GetHomePageTickets();

            if (tickets != null)
            {
                MyTickets.Tickets.AddRange(NomenclatureTranslator.UpdateTicketTranslation(tickets, FishingTicketsTransaction));
                TicketsHomePageDto result = FishingTicketsTransaction.GetTicketsHomePageMetadata();
                TotalTicketsCount = result.TotalCount.ToString();
                HasMoreTickets = result.HasMore;
            }
            if (MyTickets.Tickets.Count > 0)
            {
                TicketsState = LayoutState.Success;
            }
            else
            {
                TicketsState = LayoutState.Empty;
            }

            Translator.Current.PropertyChanged += OnTranslatorPropertyChanged;
        }

        private void OnTranslatorPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (MyTickets.Tickets.Count > 0)
                MyTickets.Tickets.ReplaceRange(NomenclatureTranslator.UpdateTicketTranslation(MyTickets.Tickets.ToList(), FishingTicketsTransaction));
        }

        private Task OnNavigate(string path)
        {
            return MainNavigator.Current.GoToPageAsync(path);
        }

        public void Dispose()
        {
            MessagingCenter.Instance.Unsubscribe<TicketsChangedArgs>(this, TicketsChangedArgs.TICKETS_CHANGED_EVENT);
        }
    }
}
