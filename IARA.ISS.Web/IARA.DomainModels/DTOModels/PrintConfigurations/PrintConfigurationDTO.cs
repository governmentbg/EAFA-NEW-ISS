namespace IARA.DomainModels.DTOModels.PrintConfigurations
{
    public class PrintConfigurationDTO
    {
        public int Id { get; set; }

        public string ApplicationTypeName { get; set; }

        public string TerritoryUnitName { get; set; }

        public string SignUserNames { get; set; }

        public string SubstituteUserNames { get; set; } 

        public string SubstituteReason { get; set; }

        public bool IsActive { get; set; }
    }
}
