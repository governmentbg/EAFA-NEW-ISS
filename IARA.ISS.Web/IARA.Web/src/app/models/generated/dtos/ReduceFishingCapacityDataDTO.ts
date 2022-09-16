

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class ReduceFishingCapacityDataDTO { 
    public constructor(obj?: Partial<ReduceFishingCapacityDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public newTonnage?: number;

    @StrictlyTyped(Number)
    public newPower?: number;
}