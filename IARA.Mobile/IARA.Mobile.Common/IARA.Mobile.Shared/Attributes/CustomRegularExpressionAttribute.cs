using System.ComponentModel.DataAnnotations;

namespace IARA.Mobile.Shared.Attributes
{
    public class CustomRegularExpressionAttribute : RegularExpressionAttribute
    {
        public CustomRegularExpressionAttribute(string pattern, string customErrorMessage)
            : base(pattern)
        {
            CustomErrorMessage = customErrorMessage;
        }

        public string CustomErrorMessage { get; }

        public override string FormatErrorMessage(string name)
        {
            return CustomErrorMessage;
        }
    }
}
