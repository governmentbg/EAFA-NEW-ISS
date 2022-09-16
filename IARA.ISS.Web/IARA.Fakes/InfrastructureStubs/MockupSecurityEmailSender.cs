using IARA.Security;
using IARA.Security.Enums;

namespace IARA.Fakes.InfrastructureStubs
{
    public class MockupSecurityEmailSender : ISecurityEmailSender
    {
        public string EnqueuePasswordEmail(string email, SecurityEmailTypes emailType, string baseAddress = null)
        {
            return string.Empty;
        }
    }
}
