using System;

namespace IARA.WebMiddlewares.RequestsTracing.Models
{
    public class RequestUser
    {
        public RequestUser(string username, DateTime timeOfRequest)
        {
            Username = username;
            TimeOfRequest = timeOfRequest;
        }

        public string Username { get; set; }
        public DateTime TimeOfRequest { get; set; }

        public override bool Equals(object obj)
        {
            RequestUser otherUser = obj as RequestUser;

            return otherUser != null && otherUser.Username == Username;
        }

        public override int GetHashCode()
        {
            return Username.GetHashCode();
        }
    }
}
