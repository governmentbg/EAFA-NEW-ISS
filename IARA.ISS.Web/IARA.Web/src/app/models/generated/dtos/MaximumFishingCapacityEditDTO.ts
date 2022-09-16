

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class MaximumFishingCapacityEditDTO { 
    public constructor(obj?: Partial<MaximumFishingCapacityEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Date)
    public date?: Date;

    @StrictlyTyped(String)
    public regulation?: string;

    @StrictlyTyped(Number)
    public grossTonnage?: number;

    @StrictlyTyped(Number)
    public power?: number;
}