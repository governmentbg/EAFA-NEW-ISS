using System;

namespace IARA.Mobile.Application.Interfaces.Utilities
{
    public interface IBackButton
    {
        event EventHandler BackButtonPressed;

        bool PreventBackButtonPress { get; set; }

        bool CloseAppOnBackButtonPressed { get; set; }

        void OnBackButtonPressed(Action backButton);
    }
}
