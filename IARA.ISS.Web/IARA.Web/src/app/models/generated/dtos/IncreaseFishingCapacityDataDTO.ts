

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class IncreaseFishingCapacityDataDTO { 
    public constructor(obj?: Partial<IncreaseFishingCapacityDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public newTonnage?: number;

    @StrictlyTyped(Number)
    public newPower?: number;
}