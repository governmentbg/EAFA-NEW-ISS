using System;
using IARA.Mobile.Application.Interfaces.Utilities;

namespace IARA.Mobile.Shared.Utilities
{
    public class BackButtonUtility : IBackButton
    {
        public event EventHandler BackButtonPressed;

        public bool PreventBackButtonPress { get; set; }
        public bool CloseAppOnBackButtonPressed { get; set; }

        public void OnBackButtonPressed(Action backButton)
        {
            if (!PreventBackButtonPress)
            {
                backButton();
            }
            else
            {
                BackButtonPressed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
