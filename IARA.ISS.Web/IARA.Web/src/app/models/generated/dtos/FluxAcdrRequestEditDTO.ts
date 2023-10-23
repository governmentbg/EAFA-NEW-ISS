

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class FluxAcdrRequestEditDTO { 
    public constructor(obj?: Partial<FluxAcdrRequestEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Date)
    public fromDate?: Date;

    @StrictlyTyped(Date)
    public toDate?: Date;
}