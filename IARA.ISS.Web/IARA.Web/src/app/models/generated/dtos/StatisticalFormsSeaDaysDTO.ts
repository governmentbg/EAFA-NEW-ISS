

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class StatisticalFormsSeaDaysDTO { 
    public constructor(obj?: Partial<StatisticalFormsSeaDaysDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public fishingGearId?: number;

    @StrictlyTyped(String)
    public fishingGearName?: string;

    @StrictlyTyped(Number)
    public days?: number;

    @StrictlyTyped(Number)
    public year?: number;
}