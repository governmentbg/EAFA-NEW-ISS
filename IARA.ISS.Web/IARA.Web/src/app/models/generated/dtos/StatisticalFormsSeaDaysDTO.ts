

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class StatisticalFormsSeaDaysDTO { 
    public constructor(obj?: Partial<StatisticalFormsSeaDaysDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public fishingGearId?: number;

    @StrictlyTyped(Number)
    public days?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}