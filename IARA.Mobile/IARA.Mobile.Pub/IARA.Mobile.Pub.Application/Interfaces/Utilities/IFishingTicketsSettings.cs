namespace IARA.Mobile.Pub.Application.Interfaces.Utilities
{
    public interface IFishingTicketsSettings
    {
        int AllowedUnder14TicketsCount { get; set; }

        /// <summary>
        /// Clears all of the current user information
        /// </summary>
        void Clear();
    }
}
