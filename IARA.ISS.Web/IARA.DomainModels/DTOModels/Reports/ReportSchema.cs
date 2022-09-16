namespace IARA.DomainModels.DTOModels.Reports
{
    public class ReportSchema
    {
        public string PropertyName { get; set; }
        public string PropertyDisplayName { get; set; }

        public override int GetHashCode()
        {
            return PropertyName.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            ReportSchema other = obj as ReportSchema;

            if (other != null)
            {
                return other.PropertyName == this.PropertyName;
            }
            else
            {
                return false;
            }
        }
    }
}
