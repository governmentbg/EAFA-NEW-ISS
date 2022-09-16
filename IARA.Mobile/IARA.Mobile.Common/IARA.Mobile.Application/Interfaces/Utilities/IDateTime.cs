using System;

namespace IARA.Mobile.Application.Interfaces.Utilities
{
    /// <summary>
    /// This class is used for easier change in the future between DateTime.Now and DateTime.UtcNow
    /// </summary>
    public interface IDateTime
    {
        /// <summary>
        /// Gives the current DateTime
        /// </summary>
        DateTime Now { get; }

        /// <summary>
        /// Gives the current DateTime of today
        /// </summary>
        DateTime Today { get; }
    }
}
