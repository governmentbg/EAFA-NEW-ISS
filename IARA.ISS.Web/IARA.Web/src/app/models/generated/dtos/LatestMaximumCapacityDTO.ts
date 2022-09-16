

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class LatestMaximumCapacityDTO { 
    public constructor(obj?: Partial<LatestMaximumCapacityDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Date)
    public date?: Date;

    @StrictlyTyped(Date)
    public prevDate?: Date;
}