import { AbstractControl, FormGroup, ValidationErrors, ValidatorFn } from '@angular/forms';
import { FishingGearTypesEnum } from '@app/enums/fishing-gear-types.enum';
import { FishingGearDTO } from '@app/models/generated/dtos/FishingGearDTO';
import { FishingGearMarkDTO } from '@app/models/generated/dtos/FishingGearMarkDTO';
import { CommonUtils } from '@app/shared/utils/common.utils';
import { FishingGearPingerDTO } from '@app/models/generated/dtos/FishingGearPingerDTO';

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

    /**
     * Tariffs
     * */
    public static readonly paidNetFishingGears: string[] = [
        FishingGearTypesEnum[FishingGearTypesEnum.GNS],
        FishingGearTypesEnum[FishingGearTypesEnum.GND],
        FishingGearTypesEnum[FishingGearTypesEnum.GNC],
        FishingGearTypesEnum[FishingGearTypesEnum.GTR],
        FishingGearTypesEnum[FishingGearTypesEnum.GTN],
        FishingGearTypesEnum[FishingGearTypesEnum.LNB],
        FishingGearTypesEnum[FishingGearTypesEnum.LNP],
        FishingGearTypesEnum[FishingGearTypesEnum.LN],
        FishingGearTypesEnum[FishingGearTypesEnum.GNF],
        FishingGearTypesEnum[FishingGearTypesEnum.GEN],
        FishingGearTypesEnum[FishingGearTypesEnum.SB]
    ];

    public static readonly paidPoleAndLineFishingGears: string[] = [
        FishingGearTypesEnum[FishingGearTypesEnum.LHP],
        FishingGearTypesEnum[FishingGearTypesEnum.LHM]
    ];

    public static readonly paidLonglinesFishingGears: string[] = [
        FishingGearTypesEnum[FishingGearTypesEnum.LLS],
        FishingGearTypesEnum[FishingGearTypesEnum.LLD],
        FishingGearTypesEnum[FishingGearTypesEnum.LL]
    ];

    public static paidDunabeNetFishingGears: string[] = [
        FishingGearTypesEnum[FishingGearTypesEnum.GNS],
        FishingGearTypesEnum[FishingGearTypesEnum.GND]
    ]; 


    //Duplicate number validators
    public static permitLicenseDuplicateMarkNumbersValidator(): ValidatorFn {
        return (form: AbstractControl): ValidationErrors | null => {
            if (form === null || form === undefined) {
                return null;
            }

            const fishingGears: FishingGearDTO[] | null | undefined = form.get('fishingGearsControl')?.value;

            if (fishingGears === null || fishingGears === undefined || fishingGears.length === 0) {
                return null;
            }

            let marks: FishingGearMarkDTO[] | null | undefined = fishingGears.filter((x: FishingGearDTO) => {
                return x.isActive && x.marks !== null && x.marks !== undefined;
            }).map(x => x.marks!)
                .reduce((prev: FishingGearMarkDTO[], curr: FishingGearMarkDTO[]) => {
                    prev.push(...curr);
                    return prev;
                }, []);
            marks = marks.filter(x => x.isActive);

            if (marks === null || marks === undefined || marks.length === 0) {
                return null;
            }

            const duplicatedMarkNumbers: string[] = [];
            const marksGrouped = CommonUtils.groupBy(marks, x => `${x.fullNumber!.prefix ?? ''}${x.fullNumber!.inputValue}`);
            const entries = Object.entries(marksGrouped);

            for (const entry of entries) {
                if ((entry[1] as Array<FishingGearMarkDTO>).length > 1) {
                    duplicatedMarkNumbers.push(entry[0]);
                }
            }

            if (duplicatedMarkNumbers.length > 0) { // there are duplicated mark numbers on frontend
                return { 'duplicatedMarkNumbers': duplicatedMarkNumbers };
            }

            return null;
        }
    }

    public static permitLicenseDuplicatePingerNumbersValidator(): ValidatorFn {
        return (form: AbstractControl): ValidationErrors | null => {
            if (form === null || form === undefined) {
                return null;
            }

            const fishingGears: FishingGearDTO[] | null | undefined = form.get('fishingGearsControl')?.value;

            if (fishingGears === null || fishingGears === undefined || fishingGears.length === 0) {
                return null;
            }

            let pingers: FishingGearPingerDTO[] | null | undefined = fishingGears.filter((x: FishingGearDTO) => {
                return x.isActive && x.hasPingers && x.pingers !== null && x.pingers !== undefined;
            }).map(x => x.pingers!)
                .reduce((prev: FishingGearPingerDTO[], curr: FishingGearPingerDTO[]) => {
                    prev.push(...curr);
                    return prev;
                }, []);
            pingers = pingers.filter(x => x.isActive);

            if (pingers === null || pingers === undefined || pingers.length === 0) {
                return null;
            }

            const duplicatedPingersNumbers: string[] = [];
            const pingersGrouped = CommonUtils.groupByKey(pingers, 'number');
            const entries = Object.entries(pingersGrouped);

            for (const entry of entries) {
                if ((entry[1] as Array<FishingGearMarkDTO>).length > 1) {
                    duplicatedPingersNumbers.push(entry[0]);
                }
            }

            if (duplicatedPingersNumbers.length > 0) { // there are duplicated pinger numbers on frontend
                return { 'duplicatedPingerNumbers': duplicatedPingersNumbers };
            }

            return null;
        }
    }

    public static atLeastOneFishingGear(): ValidatorFn {
        return (form: AbstractControl): ValidationErrors | null => {

            const fishingGearsControl = (form as FormGroup).get('fishingGearsControl');

            if (fishingGearsControl === null || fishingGearsControl === undefined) {
                return null;
            }

            if (fishingGearsControl.value !== null && fishingGearsControl.value !== undefined && fishingGearsControl.value.length > 0) {
                if ((fishingGearsControl.value as FishingGearDTO[]).some(x => x.isActive === true) === true) {
                    return null;
                }
            }

            return { 'atLeastOneFishingGear': true };
        }
    }
}