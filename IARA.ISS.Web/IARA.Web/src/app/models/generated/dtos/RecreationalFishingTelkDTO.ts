

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class RecreationalFishingTelkDTO { 
    public constructor(obj?: Partial<RecreationalFishingTelkDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Boolean)
    public isIndefinite?: boolean;

    @StrictlyTyped(String)
    public num?: string;

    @StrictlyTyped(Date)
    public validTo?: Date;
}