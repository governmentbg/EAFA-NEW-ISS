

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class AuanConfiscatedFishingGearDTO { 
    public constructor(obj?: Partial<AuanConfiscatedFishingGearDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public fishingGearId?: number;

    @StrictlyTyped(Number)
    public count?: number;

    @StrictlyTyped(Number)
    public length?: number;

    @StrictlyTyped(Number)
    public netEyeSize?: number;

    @StrictlyTyped(Number)
    public confiscationActionId?: number;

    @StrictlyTyped(String)
    public comments?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}