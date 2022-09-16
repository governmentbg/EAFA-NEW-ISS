using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using IARA.FVMSModels.NISS;

namespace IARA.FVMSModels.GeoZones
{
    public class GeoZoneData
    {
        public string Identifier { get; set; }

        /// <summary>
        ///Дата/час на създаване UTC
        /// </summary>
        [JsonPropertyName("CrDT")]
        public DateTime CreatedOn { get; set; }

        public AccessType Access { get; set; }

        public ZoneType Type { get; set; }

        /// <summary>
        /// От номенклатура FLUX_GENERAL.MDR_FAO_species
        /// </summary>
        public List<string> Species { get; set; }

        /// <summary>
        /// От номенклатура FLUX_FA.MDR_Target_Species_Group
        /// </summary>
        public List<string> SpeciesGroup { get; set; }

        [JsonPropertyName("FromDT")]
        public DateTime ValidFrom { get; set; }

        [JsonPropertyName("ToDT")]
        public DateTime ValidTo { get; set; }

        /// <summary>
        /// Валидност към настоящият момент
        /// </summary>
        public bool IsValid { get; set; }

        public string Description { get; set; }

        [JsonPropertyName("GeoZD")]
        public NISSPolygon GeoZone { get; set; }
    }
}
