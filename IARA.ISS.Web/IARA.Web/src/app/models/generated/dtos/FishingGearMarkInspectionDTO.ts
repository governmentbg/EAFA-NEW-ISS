

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class FishingGearMarkInspectionDTO { 
    public constructor(obj?: Partial<FishingGearMarkInspectionDTO>) {
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