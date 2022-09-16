using System.Collections.Generic;

namespace IARA.SourceGenerators
{
    public static class SchemaToDirectoryMapping
    {
        public static readonly Dictionary<string, string> SCHEMA_TO_FOLDER_DICT = new Dictionary<string, string>
        {
            { "Admin", "Administrative" },
            { "Appl", "Application" },
            { "CatchSales", "CatchesAndSales" },
            { "Checks", "CrossChecks" },
            { "FLUX_ACDR", "FluxAcdr" },
            { "FLUX_FA", "FluxFa" },
            { "FLUX_FLAP", "FluxFlap" },
            { "FLUX_General", "FluxGeneral" },
            { "FLUX_MDM", "FluxMdm" },
            { "FLUX_Sales", "FluxSales" },
            { "FLUX_Vessel", "FluxVessel" },
            { "FLUX_Vessel_Position", "FluxVesselPosition" },
            { "iss", "Common" },
            { "Legals", "Legals" },
            { "LRib", "HobbyFishing" },
            { "News", "News" },
            { "Noms", "Nomenclatures" },
            { "RAquaSt", "AquaFarms" },
            { "RCap", "FishingCapacity" },
            { "RCPP", "Buyers" },
            { "RDal", "Poundnets" },
            { "Rep", "Reports" },
            { "RInfStat", "InformationalStatisticForms" },
            { "RInsp", "Inspections" },
            { "RNauR", "ScientificFishing" },
            { "RPravRib", "QualifiedFishers" },
            { "RQuo", "Quotas" },
            { "RShips", "FishingShips" },
            { "RStRib", "CommercialFishing" },
            { "SysLog", "SystemLogging" },
            { "UsrMgmt", "UserManagement" }
        };

        public static string GetDirectoryOrDefault(string schema)
        {
            if (SCHEMA_TO_FOLDER_DICT.TryGetValue(schema, out string folderName))
            {
                return folderName;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
