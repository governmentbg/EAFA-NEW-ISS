

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class FluxAcdrRequestEditDTO { 
    public constructor(obj?: Partial<FluxAcdrRequestEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public month?: number;

    @StrictlyTyped(Number)
    public year?: number;
}