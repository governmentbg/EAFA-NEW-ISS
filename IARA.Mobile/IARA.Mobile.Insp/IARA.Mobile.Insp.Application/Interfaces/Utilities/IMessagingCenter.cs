namespace IARA.Mobile.Insp.Application.Interfaces.Utilities
{
    public interface IMessagingCenter
    {
        void SendMessage<TEventArgs>(TEventArgs eventArgs)
            where TEventArgs : class;
    }
}
