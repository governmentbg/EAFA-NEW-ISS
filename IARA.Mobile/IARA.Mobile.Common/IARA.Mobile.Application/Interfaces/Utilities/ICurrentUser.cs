namespace IARA.Mobile.Application.Interfaces.Utilities
{
    public interface ICurrentUser
    {
        int Id { get; set; }
        string EgnLnch { get; set; }
        string FirstName { get; set; }
        string MiddleName { get; set; }
        string LastName { get; set; }
        bool MustChangePassword { get; set; }

        /// <summary>
        /// Clears all of the current user information
        /// </summary>
        void Clear();
    }
}
