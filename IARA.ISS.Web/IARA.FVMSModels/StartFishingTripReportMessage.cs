using System.Text.Json.Serialization;

namespace IARA.FVMSModels
{
    public class StartFishingTripReportMessage
    {
        /// <summary>
        /// Уникален номер на риболовен рейс в система СНРК
        /// </summary>
        public byte[] TripNumber { get; set; }

        [JsonPropertyName("TStamCr")]
        public ulong CreatedOn { get; set; }

        [JsonPropertyName("TStampRec")]
        public ulong RequestTime { get; set; }

        /// <summary>
        /// Причина на заминаване
        /// От номенклатура FLUX_FA.MDR_FA_Reason_Departure
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Вид риболов FLUX_FA.MDR_FA_Fishery
        /// </summary>
        [JsonPropertyName("Fishery_Type")]
        public string FisheryType { get; set; }

        /// <summary>
        /// Целева група
        /// FLUX_FA.MDR_Target_Species_Group
        /// </summary>
        [JsonPropertyName("Species_Target")]
        public string SpeciesTarget { get; set; }

#warning PortObject
        /// <summary>
        /// Пристанище на тръгване
        /// </summary>
        public string Port { get; set; }

#warning PosObject
        /// <summary>
        /// Георграфска позиция
        /// </summary>
        public string Pos { get; set; }

#warning FgearsObject[]
        /// <summary>
        /// Риболовни уреди на борда на РК
        /// </summary>
        [JsonPropertyName("Fgears")]
        public string FGears { get; set; }

#warning FCRecObject[]
        /// <summary>
        /// Наличие на улов при стартиране на риболовен рейс
        /// </summary>
        public string FCRec { get; set; }
    }
}
