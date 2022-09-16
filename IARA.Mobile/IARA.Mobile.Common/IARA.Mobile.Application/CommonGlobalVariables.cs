using IARA.Mobile.Domain.Enums;

namespace IARA.Mobile.Application
{
    public static class CommonGlobalVariables
    {
        /// <summary>
        /// Path to the SQLite file in the system (used only by the IARADbContext)
        /// </summary>
        public static string DatabasePath { get; set; }

        /// <summary>
        /// The amount of items you should pull from the API
        /// </summary>
        public static int PullItemsCount { get; set; }

        /// <summary>
        /// Connection status to the server
        /// </summary>
        public static InternetStatus InternetStatus { get; set; }

        /// <summary>
        /// Indicates the app has finished with it's most vital setup.
        /// </summary>
        public static bool FinishedSetup { get; set; }

        static CommonGlobalVariables()
        {
            InternetStatus = InternetStatus.Connected;
        }
    }
}