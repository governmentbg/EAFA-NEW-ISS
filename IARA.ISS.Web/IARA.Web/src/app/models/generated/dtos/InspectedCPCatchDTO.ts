

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class InspectedCPCatchDTO { 
    public constructor(obj?: Partial<InspectedCPCatchDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public fishId?: number;

    @StrictlyTyped(Number)
    public catchQuantity?: number;

    @StrictlyTyped(Boolean)
    public isDestroyed?: boolean;

    @StrictlyTyped(Boolean)
    public isDonated?: boolean;

    @StrictlyTyped(Boolean)
    public isStored?: boolean;

    @StrictlyTyped(Boolean)
    public isTaken?: boolean;
}