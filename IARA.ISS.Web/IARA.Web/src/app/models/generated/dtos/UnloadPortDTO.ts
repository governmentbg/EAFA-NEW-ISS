

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class UnloadPortDTO { 
    public constructor(obj?: Partial<UnloadPortDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public catchQuotaId?: number;

    @StrictlyTyped(Number)
    public portId?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}