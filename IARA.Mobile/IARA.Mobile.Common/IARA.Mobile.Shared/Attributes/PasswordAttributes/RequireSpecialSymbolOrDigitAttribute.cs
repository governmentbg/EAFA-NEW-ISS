namespace IARA.Mobile.Shared.Attributes.PasswordAttributes
{
    public class RequireSpecialSymbolOrDigitAttribute : PasswordBaseValidator
    {
        public RequireSpecialSymbolOrDigitAttribute()
            : base(@"[0-9!@#$%^&*()_+=\[{\]};:<>|./?,-]")
        {
        }
    }
}
