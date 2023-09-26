namespace IARA.Infrastructure.FluxIntegrations.Validations.Common
{
    public class FluxValidationResult
    {
        public string Code { get; set; }

        public string Level { get; set; }

        public FluxValidationSeverity Severity { get; set; }

        public string Object { get; set; }

        public string ErrorMessage { get; set; }
    }
}
