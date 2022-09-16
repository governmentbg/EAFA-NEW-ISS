

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class FishingCapacitySimpleDTO { 
    public constructor(obj?: Partial<FishingCapacitySimpleDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public grossTonnage?: number;

    @StrictlyTyped(Number)
    public enginePower?: number;
}