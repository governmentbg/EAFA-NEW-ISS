

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class QuotaSpiciesPortDTO { 
    public constructor(obj?: Partial<QuotaSpiciesPortDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public portId?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}