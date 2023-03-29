namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class PrefixInputDto
    {
        public string Prefix { get; set; }
        public string InputValue { get; set; }

        public override string ToString()
        {
            return (string.IsNullOrEmpty(Prefix) ? string.Empty : Prefix) + (InputValue ?? string.Empty);
        }
    }
}
