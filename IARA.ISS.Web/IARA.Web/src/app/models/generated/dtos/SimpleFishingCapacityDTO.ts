

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class SimpleFishingCapacityDTO { 
    public constructor(obj?: Partial<SimpleFishingCapacityDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public grossTonnage?: number;

    @StrictlyTyped(Number)
    public enginePower?: number;
}