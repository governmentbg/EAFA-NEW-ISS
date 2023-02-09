using IARA.Common.Enums;
using System.Collections.Generic;
using System.Linq;

namespace IARA.Common.Utils
{
    public static class FLUXValidationUtils
    {
        /// <summary>
        /// ME - Mesh size
        /// </summary>
        public static readonly List<string> FishingGearCodesWithRequiredMeshSize = new List<string>
        {
            nameof(FishingGearTypesEnum.TBB),
            nameof(FishingGearTypesEnum.OTB),
            nameof(FishingGearTypesEnum.OT),
            nameof(FishingGearTypesEnum.OTT),
            nameof(FishingGearTypesEnum.OTP),
            nameof(FishingGearTypesEnum.PTB),
            nameof(FishingGearTypesEnum.TB),
            nameof(FishingGearTypesEnum.TBN),
            nameof(FishingGearTypesEnum.TBS),
            nameof(FishingGearTypesEnum.PUK),
            nameof(FishingGearTypesEnum.PUL),
            nameof(FishingGearTypesEnum.OTM),
            nameof(FishingGearTypesEnum.PTM),
            nameof(FishingGearTypesEnum.TM),
            nameof(FishingGearTypesEnum.TMS),
            nameof(FishingGearTypesEnum.TSP),
            nameof(FishingGearTypesEnum.TX),
            nameof(FishingGearTypesEnum.SDN),
            nameof(FishingGearTypesEnum.SSC),
            nameof(FishingGearTypesEnum.SPR),
            nameof(FishingGearTypesEnum.SX),
            nameof(FishingGearTypesEnum.SV),
            nameof(FishingGearTypesEnum.SB),
            nameof(FishingGearTypesEnum.PS),
            nameof(FishingGearTypesEnum.PS1),
            nameof(FishingGearTypesEnum.PS2),
            nameof(FishingGearTypesEnum.LA),
            nameof(FishingGearTypesEnum.SUX),
            nameof(FishingGearTypesEnum.DRB),
            nameof(FishingGearTypesEnum.GN),
            nameof(FishingGearTypesEnum.GNS),
            nameof(FishingGearTypesEnum.GND),
            nameof(FishingGearTypesEnum.GNC),
            nameof(FishingGearTypesEnum.GTN),
            nameof(FishingGearTypesEnum.GTR),
            nameof(FishingGearTypesEnum.GNF),
            nameof(FishingGearTypesEnum.GEN)
        };

        /// <summary>
        /// GM - Gear dimension by length or width of the gear - in meters:
        /// length of beams,
        /// trawl - perimeter of opening,
        /// seine nets - overall length,
        /// purse seine - length,
        /// puse seine - one boar operated - length,
        /// width of dredges,
        /// gill nets - length
        /// </summary>
        public static readonly List<string> FishingGearCodesWithRequiredGearDimension = new List<string>
        {
            nameof(FishingGearTypesEnum.TBB),
            nameof(FishingGearTypesEnum.PUK),
            nameof(FishingGearTypesEnum.SDN),
            nameof(FishingGearTypesEnum.SSC),
            nameof(FishingGearTypesEnum.SPR),
            nameof(FishingGearTypesEnum.SX),
            nameof(FishingGearTypesEnum.SV),
            nameof(FishingGearTypesEnum.SB),
            nameof(FishingGearTypesEnum.PS),
            nameof(FishingGearTypesEnum.PS1),
            nameof(FishingGearTypesEnum.PS2),
            nameof(FishingGearTypesEnum.LA),
            nameof(FishingGearTypesEnum.SUX),
            nameof(FishingGearTypesEnum.DRB),
            nameof(FishingGearTypesEnum.GN),
            nameof(FishingGearTypesEnum.GNS),
            nameof(FishingGearTypesEnum.GND),
            nameof(FishingGearTypesEnum.GNC),
            nameof(FishingGearTypesEnum.GTN),
            nameof(FishingGearTypesEnum.GTR),
            nameof(FishingGearTypesEnum.GNF),
            nameof(FishingGearTypesEnum.GEN)
        };

        /// <summary>
        ///  Perimeter of opening (GM) or model of trawl (MT) must be provided
        /// </summary>
        public static readonly List<string> FishingGearCodesWithOptionalGearDimension = new List<string>
        {
            nameof(FishingGearTypesEnum.OTB),
            nameof(FishingGearTypesEnum.OT),
            nameof(FishingGearTypesEnum.OTT),
            nameof(FishingGearTypesEnum.OTP),
            nameof(FishingGearTypesEnum.PTB),
            nameof(FishingGearTypesEnum.PT),
            nameof(FishingGearTypesEnum.TB),
            nameof(FishingGearTypesEnum.TBN),
            nameof(FishingGearTypesEnum.TBS),
            nameof(FishingGearTypesEnum.PUL)
        };

        /// <summary>
        /// HE - Height
        /// </summary>
        public static readonly List<string> FishingGearCodesWithRequiredHeight = new List<string>
        {
            nameof(FishingGearTypesEnum.PS),
            nameof(FishingGearTypesEnum.PS1),
            nameof(FishingGearTypesEnum.PS2),
            nameof(FishingGearTypesEnum.LA),
            nameof(FishingGearTypesEnum.SUX),
            nameof(FishingGearTypesEnum.GN),
            nameof(FishingGearTypesEnum.GNS),
            nameof(FishingGearTypesEnum.GND),
            nameof(FishingGearTypesEnum.GNC),
            nameof(FishingGearTypesEnum.GTN),
            nameof(FishingGearTypesEnum.GTR),
            nameof(FishingGearTypesEnum.GNF),
            nameof(FishingGearTypesEnum.GEN)
        };

        /// <summary>
        /// NL - Nominal lenght of one net in a fleet
        /// </summary>
        public static readonly List<string> FishingGearCodesWithRequiredNetLength = new List<string>
        {
            nameof(FishingGearTypesEnum.GN),
            nameof(FishingGearTypesEnum.GNS),
            nameof(FishingGearTypesEnum.GND),
            nameof(FishingGearTypesEnum.GNC),
            nameof(FishingGearTypesEnum.GTN),
            nameof(FishingGearTypesEnum.GTR),
            nameof(FishingGearTypesEnum.GNF),
            nameof(FishingGearTypesEnum.GEN)
        };

        /// <summary>
        /// GN - Gear dimensions by number: 
        /// number of trawls, 
        /// number of beams, 
        /// number of dredges, 
        /// number of pots, 
        /// number of hooks
        /// </summary>
        public static readonly List<string> FishingGearCodesWithRequiredNumberDimension = new List<string>
        {
            nameof(FishingGearTypesEnum.TBB),
            nameof(FishingGearTypesEnum.OTT),
            nameof(FishingGearTypesEnum.PUK),
            nameof(FishingGearTypesEnum.DRB),
            nameof(FishingGearTypesEnum.FPO),
            nameof(FishingGearTypesEnum.LHP),
            nameof(FishingGearTypesEnum.LHM),
            nameof(FishingGearTypesEnum.LLS),
            nameof(FishingGearTypesEnum.LLD),
            nameof(FishingGearTypesEnum.LL)
        };

        /// <summary>
        /// NI - Number of lines
        /// </summary>
        public static readonly List<string> FishingGearCodesWithRequiredNumberOfLines = new List<string>
        {
            nameof(FishingGearTypesEnum.LHP),
            nameof(FishingGearTypesEnum.LHM),
            nameof(FishingGearTypesEnum.LLS),
            nameof(FishingGearTypesEnum.LLD),
            nameof(FishingGearTypesEnum.LL)
        };

        /// <summary>
        /// NN - number of nets in the fleet
        /// </summary>
        public static readonly List<string> FishingGearCodesWithRequiredNumberOfNets = new List<string>
        {
            nameof(FishingGearTypesEnum.GN),
            nameof(FishingGearTypesEnum.GNS),
            nameof(FishingGearTypesEnum.GND),
            nameof(FishingGearTypesEnum.GNC),
            nameof(FishingGearTypesEnum.GTN),
            nameof(FishingGearTypesEnum.GTR),
            nameof(FishingGearTypesEnum.GNF),
            nameof(FishingGearTypesEnum.GEN)
        };

        /// <summary>
        /// QG - quantity of gear onboard
        /// </summary>
        public static readonly List<string> FishingGearCodesWithRequiredQuantityOnboard = new List<string>
        {
            nameof(FishingGearTypesEnum.GN),
            nameof(FishingGearTypesEnum.GNS),
            nameof(FishingGearTypesEnum.GND),
            nameof(FishingGearTypesEnum.GNC),
            nameof(FishingGearTypesEnum.GTN),
            nameof(FishingGearTypesEnum.GTR),
            nameof(FishingGearTypesEnum.GNF),
            nameof(FishingGearTypesEnum.GEN)
        };

        /// <summary>
        /// MT - Model of trawl
        /// </summary>
        public static readonly List<string> FishingGearCodesWithRequiredTrawlModel = new List<string>
        {
            nameof(FishingGearTypesEnum.OTM),
            nameof(FishingGearTypesEnum.PTM),
            nameof(FishingGearTypesEnum.TM),
            nameof(FishingGearTypesEnum.TMS),
            nameof(FishingGearTypesEnum.TSP)
        };

        /// <summary>
        /// MT - Model of trawl
        /// </summary>
        public static readonly List<string> FishingGearCodesWithOptionalTrawlModel = new List<string>
        {
            nameof(FishingGearTypesEnum.OTB),
            nameof(FishingGearTypesEnum.OT),
            nameof(FishingGearTypesEnum.OTT),
            nameof(FishingGearTypesEnum.OTP),
            nameof(FishingGearTypesEnum.PTB),
            nameof(FishingGearTypesEnum.PT),
            nameof(FishingGearTypesEnum.TB),
            nameof(FishingGearTypesEnum.TBN),
            nameof(FishingGearTypesEnum.TBS),
            nameof(FishingGearTypesEnum.PUL)
        };
    }
}
