namespace IARA.Mobile.Insp.Domain.Enums
{
    public enum PostEnum
    {
        /// <summary>
        /// Successfully uploaded to the server.
        /// </summary>
        Success,

        /// <summary>
        /// Saved post to the database because the user is offline.
        /// </summary>
        Offline,

        /// <summary>
        /// The request failed.
        /// </summary>
        Failed
    }
}
