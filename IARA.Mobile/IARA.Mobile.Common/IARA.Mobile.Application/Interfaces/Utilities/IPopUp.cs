namespace IARA.Mobile.Application.Interfaces.Utilities
{
    public interface IPopUp
    {
        /// <summary>
        /// Displays an alert that says the user has encountered a problem.
        /// </summary>
        void AlertException();

        /// <summary>
        /// Displays an alert that says there is no Internet.
        /// </summary>
        void AlertNoInternet();

        /// <summary>
        /// Displays an alert that says there was a problem with the request data.
        /// </summary>
        void AlertUnsuccessfulRequest();

        /// <summary>
        /// Displays an alert that says the data sent to the server exceeds the limits.
        /// </summary>
        void AlertRequestEntityTooLarge();
    }
}
