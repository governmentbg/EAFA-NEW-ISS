namespace IARA.Infrastructure.FluxIntegrations.Validations.Common
{
    [AttributeUsage(AttributeTargets.Method)]
    public class FluxValidationAttribute : Attribute
    {
        public string Code { get; private set; }

        public string Level { get; set; }

        public FluxValidationSeverity Severity { get; private set; }

        public string Object { get; private set; }

        public string ErrorMessage { get; private set; }

        public FluxValidationAttribute(string code, string level, FluxValidationSeverity severity, string obj = "", string errorMessage = "")
        {
            Code = code;
            Level = level;
            Severity = severity;
            Object = obj;
            ErrorMessage = errorMessage;
        }
    }
}
