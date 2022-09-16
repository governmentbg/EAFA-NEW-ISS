namespace IARA.Mobile.Shared.Attributes.PasswordAttributes
{
    public class RequireUpperCaseAttribute : PasswordBaseValidator
    {
        public RequireUpperCaseAttribute()
            : base(@"[A-Z]+")
        {

        }
    }
}
