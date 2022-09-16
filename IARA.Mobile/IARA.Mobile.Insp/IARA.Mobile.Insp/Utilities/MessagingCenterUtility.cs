using IARA.Mobile.Insp.Application.Interfaces.Utilities;

namespace IARA.Mobile.Insp.Utilities
{
    public class MessagingCenterUtility : IMessagingCenter
    {
        public void SendMessage<TEventArgs>(TEventArgs eventArgs)
            where TEventArgs : class
        {
            Xamarin.Forms.MessagingCenter.Instance.Send(eventArgs, string.Empty);
        }
    }
}
