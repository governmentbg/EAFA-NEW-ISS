

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class InspectedCPFishingGearDTO { 
    public constructor(obj?: Partial<InspectedCPFishingGearDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public fishingGearId?: number;

    @StrictlyTyped(Number)
    public gearCount?: number;

    @StrictlyTyped(Number)
    public length?: number;

    @StrictlyTyped(String)
    public description?: string;

    @StrictlyTyped(Boolean)
    public isStored?: boolean;

    @StrictlyTyped(Boolean)
    public isTaken?: boolean;
}