

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class ShipQuotaEditDTO { 
    public constructor(obj?: Partial<ShipQuotaEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public shipId?: number;

    @StrictlyTyped(Number)
    public quotaId?: number;

    @StrictlyTyped(Number)
    public shipQuotaSize?: number;

    @StrictlyTyped(Number)
    public leftoverQuotaSize?: number;

    @StrictlyTyped(String)
    public changeBasis?: string;

    @StrictlyTyped(Number)
    public unloadedByCurrentDateKg?: number;

    @StrictlyTyped(Number)
    public unloadedByCurrentDatePercent?: number;
}