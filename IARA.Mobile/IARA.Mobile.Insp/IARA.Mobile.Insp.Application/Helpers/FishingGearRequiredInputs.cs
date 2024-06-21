using IARA.Mobile.Insp.Domain.Enums;
using System.Collections.Generic;

namespace IARA.Mobile.Insp.Application.Helpers
{
    public static class FishingGearRequiredInputs
    {
        private static readonly Dictionary<string, List<FishGearInputs>> RequiredInputsByFishingGear = new Dictionary<string, List<FishGearInputs>>
        {
            { "TBB", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.Length } },
            { "OTB", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.OptionalGearDimension } },
            { "OT", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.OptionalGearDimension } },
            { "OTT", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.OptionalGearDimension } },
            { "OTP", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.OptionalGearDimension } },
            { "PTB", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.OptionalGearDimension } },
            { "PT", new List<FishGearInputs> { FishGearInputs.OptionalGearDimension } },
            { "TB", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.OptionalGearDimension } },
            { "TBN", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.OptionalGearDimension } },
            { "TBS", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.OptionalGearDimension } },
            { "PUK", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.Length } },
            { "PUL", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.OptionalGearDimension } },
            { "OTM", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.TralModel } },
            { "PTM", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.TralModel } },
            { "TM", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.TralModel } },
            { "TMS", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.TralModel } },
            { "TSP", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.TralModel } },
            { "TX", new List<FishGearInputs> { FishGearInputs.NetEyeSize } },
            { "SDN", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.Length } },
            { "SSC", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.Length } },
            { "SPR", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.Length } },
            { "SX", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.Length } },
            { "SV", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.Length } },
            { "SB", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.Length } },
            { "PS", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.Length, FishGearInputs.Height } },
            { "PS1", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.Length, FishGearInputs.Height } },
            { "PS2", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.Length, FishGearInputs.Height } },
            { "LA", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.Length, FishGearInputs.Height } },
            { "SUX", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.Length, FishGearInputs.Height } },
            { "DRB", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.Length } },
            { "GN", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.Length, FishGearInputs.Height, FishGearInputs.FullHeight, FishGearInputs.NetCountInFlot } },
            { "GNS", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.Length, FishGearInputs.Height, FishGearInputs.FullHeight, FishGearInputs.NetCountInFlot } },
            { "GND", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.Length, FishGearInputs.Height, FishGearInputs.FullHeight, FishGearInputs.NetCountInFlot } },
            { "GNC", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.Length, FishGearInputs.Height, FishGearInputs.FullHeight, FishGearInputs.NetCountInFlot } },
            { "GTN", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.Length, FishGearInputs.Height, FishGearInputs.FullHeight, FishGearInputs.NetCountInFlot } },
            { "GTR", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.Length, FishGearInputs.Height, FishGearInputs.FullHeight, FishGearInputs.NetCountInFlot } },
            { "GNF", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.Length, FishGearInputs.Height, FishGearInputs.FullHeight, FishGearInputs.NetCountInFlot } },
            { "GEN", new List<FishGearInputs> { FishGearInputs.NetEyeSize, FishGearInputs.Length, FishGearInputs.Height, FishGearInputs.FullHeight, FishGearInputs.NetCountInFlot } },
            { "LHP", new List<FishGearInputs> { FishGearInputs.RowCount } },
            { "LHM", new List<FishGearInputs> { FishGearInputs.RowCount } },
            { "LLS", new List<FishGearInputs> { FishGearInputs.RowCount } },
            { "LLD", new List<FishGearInputs> { FishGearInputs.RowCount } },
            { "LL", new List<FishGearInputs> { FishGearInputs.RowCount } },
        };

        public static readonly Dictionary<FishGearInputs, List<string>> RequiredInputs = new Dictionary<FishGearInputs, List<string>>
        {
            { FishGearInputs.NetEyeSize, new List<string> { "TBB", "OTB", "OT", "OTT", "OTP", "PTB", "TB", "TBN", "TBS", "PUK",
                                                            "PUL", "OTM", "PTM", "TM", "TMS", "TSP", "TX", "SDN", "SSC", "SPR",
                                                            "SX", "SV", "SB", "PS", "PS1", "PS2", "LA", "SUX", "DRB", "GN",
                                                            "GNS", "GND", "GNC", "GTN", "GTR", "GNF", "GEN" } },
            { FishGearInputs.Length, new List<string> { "TBB", "PUK", "SDN", "SSC", "SPR", "SX", "SV", "SB", "PS", "PS1",
                                                        "PS2", "LA", "SUX", "DRB", "GN", "GNS", "GND", "GNC", "GTN", "GTR",
                                                        "GNF", "GEN" }},
            { FishGearInputs.Height, new List<string> { "PS", "PS1", "PS2", "LA", "SUX", "GN", "GNS", "GND", "GNC", "GTN",
                                                        "GTR", "GNF", "GEN" }},
            { FishGearInputs.FullHeight, new List<string> { "GN", "GNS", "GND", "GNC", "GTN", "GTR", "GNF", "GEN" }},
            { FishGearInputs.RowCount, new List<string> { "LHP", "LHM", "LLS", "LLD", "LL" }},
            { FishGearInputs.NetCountInFlot, new List<string> { "GN", "GNS", "GND", "GNC", "GTN", "GTR", "GNF", "GEN" }},
            { FishGearInputs.TralModel, new List<string> { "OTM", "PTM", "TM", "TMS", "TSP" }},
            { FishGearInputs.OptionalGearDimension, new List<string> { "OTB", "OT", "OTT", "OTP", "PTB", "PT", "TB", "TBN", "TBS", "PUL" }}
        };

        public static List<FishGearInputs> GetRequiredInputs(string fishingGearCode)
        {
            if (RequiredInputsByFishingGear.ContainsKey(fishingGearCode))
            {
                return RequiredInputsByFishingGear[fishingGearCode];
            }

            return new List<FishGearInputs>();
        }
    }
}
