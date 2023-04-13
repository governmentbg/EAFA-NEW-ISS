

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class StatisticalFormShipSeaDaysDTO { 
    public constructor(obj?: Partial<StatisticalFormShipSeaDaysDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public year?: number;

    @StrictlyTyped(Number)
    public days?: number;
}