namespace IARA.Mobile.Shared.Attributes.PasswordAttributes
{
    public class RequireLowerCaseAttribute : PasswordBaseValidator
    {
        public RequireLowerCaseAttribute()
            : base(@"[a-z]+")
        {

        }
    }
}
