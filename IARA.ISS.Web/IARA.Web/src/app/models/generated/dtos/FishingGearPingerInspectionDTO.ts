

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class FishingGearPingerInspectionDTO { 
    public constructor(obj?: Partial<FishingGearPingerInspectionDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public fishingGearId?: number;

    @StrictlyTyped(String)
    public number?: string;

    @StrictlyTyped(Number)
    public statusId?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}