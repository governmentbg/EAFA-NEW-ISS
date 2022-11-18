

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { FishingGearMarkDTO } from './FishingGearMarkDTO';
import { FishingGearPingerDTO } from './FishingGearPingerDTO';

export class FishingGearDTO { 
    public constructor(obj?: Partial<FishingGearDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public typeId?: number;

    @StrictlyTyped(String)
    public type?: string;

    @StrictlyTyped(Number)
    public count?: number;

    @StrictlyTyped(String)
    public marksNumbers?: string;

    @StrictlyTyped(Number)
    public length?: number;

    @StrictlyTyped(Number)
    public height?: number;

    @StrictlyTyped(Number)
    public netEyeSize?: number;

    @StrictlyTyped(Number)
    public hookCount?: number;

    @StrictlyTyped(String)
    public description?: string;

    @StrictlyTyped(Number)
    public towelLength?: number;

    @StrictlyTyped(Number)
    public houseLength?: number;

    @StrictlyTyped(Number)
    public houseWidth?: number;

    @StrictlyTyped(Number)
    public cordThickness?: number;

    @StrictlyTyped(Number)
    public lineCount?: number;

    @StrictlyTyped(Number)
    public netNominalLength?: number;

    @StrictlyTyped(Number)
    public netsInFleetCount?: number;

    @StrictlyTyped(String)
    public trawlModel?: string;

    @StrictlyTyped(Boolean)
    public hasPingers?: boolean;

    @StrictlyTyped(Number)
    public permitId?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;

    @StrictlyTyped(FishingGearMarkDTO)
    public marks?: FishingGearMarkDTO[];

    @StrictlyTyped(FishingGearPingerDTO)
    public pingers?: FishingGearPingerDTO[];
}