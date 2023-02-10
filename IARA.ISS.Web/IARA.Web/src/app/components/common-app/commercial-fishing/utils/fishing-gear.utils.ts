import { FishingGearTypesEnum } from '@app/enums/fishing-gear-types.enum';

export class FishingGearUtils {
    /**
     * ME - Mesh size
     * */
    public static readonly fishingGearCodesWithRequiredMeshSize: string[] = [
        FishingGearTypesEnum[FishingGearTypesEnum.TBB],
        FishingGearTypesEnum[FishingGearTypesEnum.OTB],
        FishingGearTypesEnum[FishingGearTypesEnum.OT],
        FishingGearTypesEnum[FishingGearTypesEnum.OTT],
        FishingGearTypesEnum[FishingGearTypesEnum.OTP],
        FishingGearTypesEnum[FishingGearTypesEnum.PTB],
        FishingGearTypesEnum[FishingGearTypesEnum.TB],
        FishingGearTypesEnum[FishingGearTypesEnum.TBN],
        FishingGearTypesEnum[FishingGearTypesEnum.TBS],
        FishingGearTypesEnum[FishingGearTypesEnum.PUK],
        FishingGearTypesEnum[FishingGearTypesEnum.PUL],
        FishingGearTypesEnum[FishingGearTypesEnum.OTM],
        FishingGearTypesEnum[FishingGearTypesEnum.PTM],
        FishingGearTypesEnum[FishingGearTypesEnum.TM],
        FishingGearTypesEnum[FishingGearTypesEnum.TMS],
        FishingGearTypesEnum[FishingGearTypesEnum.TSP],
        FishingGearTypesEnum[FishingGearTypesEnum.TX],
        FishingGearTypesEnum[FishingGearTypesEnum.SDN],
        FishingGearTypesEnum[FishingGearTypesEnum.SSC],
        FishingGearTypesEnum[FishingGearTypesEnum.SPR],
        FishingGearTypesEnum[FishingGearTypesEnum.SX],
        FishingGearTypesEnum[FishingGearTypesEnum.SV],
        FishingGearTypesEnum[FishingGearTypesEnum.SB],
        FishingGearTypesEnum[FishingGearTypesEnum.PS],
        FishingGearTypesEnum[FishingGearTypesEnum.PS1],
        FishingGearTypesEnum[FishingGearTypesEnum.PS2],
        FishingGearTypesEnum[FishingGearTypesEnum.LA],
        FishingGearTypesEnum[FishingGearTypesEnum.SUX],
        FishingGearTypesEnum[FishingGearTypesEnum.DRB],
        FishingGearTypesEnum[FishingGearTypesEnum.GN],
        FishingGearTypesEnum[FishingGearTypesEnum.GNS],
        FishingGearTypesEnum[FishingGearTypesEnum.GND],
        FishingGearTypesEnum[FishingGearTypesEnum.GNC],
        FishingGearTypesEnum[FishingGearTypesEnum.GTN],
        FishingGearTypesEnum[FishingGearTypesEnum.GTR],
        FishingGearTypesEnum[FishingGearTypesEnum.GNF],
        FishingGearTypesEnum[FishingGearTypesEnum.GEN],
    ];

    /**
     * GM - Gear dimension by length or width of the gear - in meters:
     * length of beams,
     * trawl - perimeter of opening,
     * seine nets - overall length,
     * purse seine - length,
     * puse seine - one boar operated - length,
     * width of dredges,
     * gill nets - length
     * */
    public static readonly fishingGearCodesWithRequiredGearDimension: string[] = [
        FishingGearTypesEnum[FishingGearTypesEnum.TBB],
        FishingGearTypesEnum[FishingGearTypesEnum.PUK],
        FishingGearTypesEnum[FishingGearTypesEnum.SDN],
        FishingGearTypesEnum[FishingGearTypesEnum.SSC],
        FishingGearTypesEnum[FishingGearTypesEnum.SPR],
        FishingGearTypesEnum[FishingGearTypesEnum.SX],
        FishingGearTypesEnum[FishingGearTypesEnum.SV],
        FishingGearTypesEnum[FishingGearTypesEnum.SB],
        FishingGearTypesEnum[FishingGearTypesEnum.PS],
        FishingGearTypesEnum[FishingGearTypesEnum.PS1],
        FishingGearTypesEnum[FishingGearTypesEnum.PS2],
        FishingGearTypesEnum[FishingGearTypesEnum.LA],
        FishingGearTypesEnum[FishingGearTypesEnum.SUX],
        FishingGearTypesEnum[FishingGearTypesEnum.DRB],
        FishingGearTypesEnum[FishingGearTypesEnum.GN],
        FishingGearTypesEnum[FishingGearTypesEnum.GNS],
        FishingGearTypesEnum[FishingGearTypesEnum.GND],
        FishingGearTypesEnum[FishingGearTypesEnum.GNC],
        FishingGearTypesEnum[FishingGearTypesEnum.GTN],
        FishingGearTypesEnum[FishingGearTypesEnum.GTR],
        FishingGearTypesEnum[FishingGearTypesEnum.GNF],
        FishingGearTypesEnum[FishingGearTypesEnum.GEN]
    ];

    /**
     * Perimeter of opening (GM) or model of trawl (MT) must be provided
     * */
    public static readonly fishingGearCodesWithOptionalGearDimension: string[] = [
        FishingGearTypesEnum[FishingGearTypesEnum.OTB],
        FishingGearTypesEnum[FishingGearTypesEnum.OT],
        FishingGearTypesEnum[FishingGearTypesEnum.OTT],
        FishingGearTypesEnum[FishingGearTypesEnum.OTP],
        FishingGearTypesEnum[FishingGearTypesEnum.PTB],
        FishingGearTypesEnum[FishingGearTypesEnum.PT],
        FishingGearTypesEnum[FishingGearTypesEnum.TB],
        FishingGearTypesEnum[FishingGearTypesEnum.TBN],
        FishingGearTypesEnum[FishingGearTypesEnum.TBS],
        FishingGearTypesEnum[FishingGearTypesEnum.PUL]
    ];

    /**
     * HE - Height
     * */
    public static readonly fishingGearCodesWithRequiredHeight: string[] = [
        FishingGearTypesEnum[FishingGearTypesEnum.PS],
        FishingGearTypesEnum[FishingGearTypesEnum.PS1],
        FishingGearTypesEnum[FishingGearTypesEnum.PS2],
        FishingGearTypesEnum[FishingGearTypesEnum.LA],
        FishingGearTypesEnum[FishingGearTypesEnum.SUX],
        FishingGearTypesEnum[FishingGearTypesEnum.GN],
        FishingGearTypesEnum[FishingGearTypesEnum.GNS],
        FishingGearTypesEnum[FishingGearTypesEnum.GND],
        FishingGearTypesEnum[FishingGearTypesEnum.GNC],
        FishingGearTypesEnum[FishingGearTypesEnum.GTN],
        FishingGearTypesEnum[FishingGearTypesEnum.GTR],
        FishingGearTypesEnum[FishingGearTypesEnum.GNF],
        FishingGearTypesEnum[FishingGearTypesEnum.GEN]
    ];

    /**
     * NL - Nominal lenght of one net in a fleet
     * */
    public static readonly fishingGearCodesWithRequiredNetLength: string[] = [
        FishingGearTypesEnum[FishingGearTypesEnum.GN],
        FishingGearTypesEnum[FishingGearTypesEnum.GNS],
        FishingGearTypesEnum[FishingGearTypesEnum.GND],
        FishingGearTypesEnum[FishingGearTypesEnum.GNC],
        FishingGearTypesEnum[FishingGearTypesEnum.GTN],
        FishingGearTypesEnum[FishingGearTypesEnum.GTR],
        FishingGearTypesEnum[FishingGearTypesEnum.GNF],
        FishingGearTypesEnum[FishingGearTypesEnum.GEN]
    ];

    /**
     * GN - Gear dimensions by number: 
     * number of trawls, 
     * number of beams, 
     * number of dredges, 
     * number of pots, 
     * number of hooks
     * */
    public static readonly fishingGearCodesWithRequiredNumberDimension: string[] = [
        FishingGearTypesEnum[FishingGearTypesEnum.TBB],
        FishingGearTypesEnum[FishingGearTypesEnum.OTT],
        FishingGearTypesEnum[FishingGearTypesEnum.PUK],
        FishingGearTypesEnum[FishingGearTypesEnum.DRB],
        FishingGearTypesEnum[FishingGearTypesEnum.FPO],
        FishingGearTypesEnum[FishingGearTypesEnum.LHP],
        FishingGearTypesEnum[FishingGearTypesEnum.LHM],
        FishingGearTypesEnum[FishingGearTypesEnum.LLS],
        FishingGearTypesEnum[FishingGearTypesEnum.LLD],
        FishingGearTypesEnum[FishingGearTypesEnum.LL]
    ];

    /**
     * NI - Number of lines
     * */
    public static readonly fishingGearCodesWithRequiredNumberOfLines: string[] = [
        FishingGearTypesEnum[FishingGearTypesEnum.LHP],
        FishingGearTypesEnum[FishingGearTypesEnum.LHM],
        FishingGearTypesEnum[FishingGearTypesEnum.LLS],
        FishingGearTypesEnum[FishingGearTypesEnum.LLD],
        FishingGearTypesEnum[FishingGearTypesEnum.LL]
    ];

    /**
     * NN - number of nets in the fleet
     * */
    public static readonly fishingGearCodesWithRequiredNumberOfNets: string[] = [
        FishingGearTypesEnum[FishingGearTypesEnum.GN],
        FishingGearTypesEnum[FishingGearTypesEnum.GNS],
        FishingGearTypesEnum[FishingGearTypesEnum.GND],
        FishingGearTypesEnum[FishingGearTypesEnum.GNC],
        FishingGearTypesEnum[FishingGearTypesEnum.GTN],
        FishingGearTypesEnum[FishingGearTypesEnum.GTR],
        FishingGearTypesEnum[FishingGearTypesEnum.GNF],
        FishingGearTypesEnum[FishingGearTypesEnum.GEN]
    ];

    /**
     * QG - quantity of gear onboard
     * */
    public static readonly fishingGearCodesWithRequiredQuantityOnboard: string[] = [
        FishingGearTypesEnum[FishingGearTypesEnum.GN],
        FishingGearTypesEnum[FishingGearTypesEnum.GNS],
        FishingGearTypesEnum[FishingGearTypesEnum.GND],
        FishingGearTypesEnum[FishingGearTypesEnum.GNC],
        FishingGearTypesEnum[FishingGearTypesEnum.GTN],
        FishingGearTypesEnum[FishingGearTypesEnum.GTR],
        FishingGearTypesEnum[FishingGearTypesEnum.GNF],
        FishingGearTypesEnum[FishingGearTypesEnum.GEN]
    ];

    /**
     * MT - Model of trawl
     * */
    public static readonly fishingGearCodesWithRequiredTrawlModel: string[] = [
        FishingGearTypesEnum[FishingGearTypesEnum.OTM],
        FishingGearTypesEnum[FishingGearTypesEnum.PTM],
        FishingGearTypesEnum[FishingGearTypesEnum.TM],
        FishingGearTypesEnum[FishingGearTypesEnum.TMS],
        FishingGearTypesEnum[FishingGearTypesEnum.TSP]
    ];

    /**
     * MT - Model of trawl
     * */
    public static readonly fishingGearCodesWithOptionalTrawlModel: string[] = [
        FishingGearTypesEnum[FishingGearTypesEnum.OTB],
        FishingGearTypesEnum[FishingGearTypesEnum.OT],
        FishingGearTypesEnum[FishingGearTypesEnum.OTT],
        FishingGearTypesEnum[FishingGearTypesEnum.OTP],
        FishingGearTypesEnum[FishingGearTypesEnum.PTB],
        FishingGearTypesEnum[FishingGearTypesEnum.PT],
        FishingGearTypesEnum[FishingGearTypesEnum.TB],
        FishingGearTypesEnum[FishingGearTypesEnum.TBN],
        FishingGearTypesEnum[FishingGearTypesEnum.TBS],
        FishingGearTypesEnum[FishingGearTypesEnum.PUL]
    ];
}