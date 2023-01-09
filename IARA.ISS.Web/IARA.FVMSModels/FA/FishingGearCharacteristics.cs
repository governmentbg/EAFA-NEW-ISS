namespace IARA.FVMSModels.FA
{
    public class FishingGearCharacteristics
    {
        /// <summary>
        /// Fishing gear code
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// ME - Mesh size
        /// </summary>
        public decimal? MeshSize { get; set; }

        /// <summary>
        /// GD - Gear description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// DA - Devices and gear attachments
        /// </summary>
        public string DevicesAndGearAttachments { get; set; }

        /// <summary>
        /// GO - Gear bar distance
        /// </summary>
        public decimal? BarDistance { get; set; }

        /// <summary>
        /// MT - Model of trawl
        /// </summary>
        public string TrawlModel { get; set; }

        /// <summary>
        /// GM - Gear dimension by length or width of the gear - in metres: 
        ///     length of beams, 
        ///     trawl – perimeter of opening, 
        ///     seine nets – overall length, 
        ///     purse seine – length, 
        ///     purse seine – one boat operated – length, width of dredges, gill nets length
        /// </summary>
        public decimal? Dimension { get; set; }

        /// <summary>
        /// HE - Height of the net
        /// </summary>
        public decimal? Height { get; set; }

        /// <summary>
        /// GN - Gear dimension by number. 
        /// For example: number of trawls, number of beams, number of dredges, number of pots, number of hooks
        /// </summary>
        public int? Number { get; set; }

        /// <summary>
        /// NI - Number of lines
        /// </summary>
        public int? NumberOfLines { get; set; }

        /// <summary>
        /// NL - Nominal length of one net in a fleet
        /// </summary>
        public decimal? OneNetLength { get; set; }

        /// <summary>
        /// NN - Number of nets in the fleet
        /// </summary>
        public decimal? NetsNumber { get; set; }

        /// <summary>
        /// QG - Quantity of gear on board
        /// </summary>
        public int QuantityOnBoard { get; set; }
    }
}
