namespace IARA.Infrastructure.FluxIntegrations.Utils
{
    public static class FluxIdentifierGenerator
    {
        public static string GenerateTripIdentifier(string countryCode = "BGR")
        {
            return $"{countryCode}-TRP-{Generate()}";
        }

        public static string GenerateSalesIdentifier(string type, string countryCode = "BGR")
        {
            return $"{countryCode}-{type}-{Generate()}";
        }

        private static string Generate()
        {
            return DateTime.Now.Ticks.ToString();
        }
    }
}
