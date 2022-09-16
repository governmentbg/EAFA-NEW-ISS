namespace IARA.Mobile.Shared.Attributes.PasswordAttributes
{
    public class RequireMinLengthAttribute : PasswordBaseValidator
    {
        public int MinLength { get; }
        public RequireMinLengthAttribute(int minLength)
            : base(@".{" + minLength + ",}")
        {
            MinLength = minLength;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, MinLength);
        }
    }
}
