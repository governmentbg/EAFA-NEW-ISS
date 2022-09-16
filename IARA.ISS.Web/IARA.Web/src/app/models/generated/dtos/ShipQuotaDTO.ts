

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { QuotaHistDTO } from './QuotaHistDTO';

export class ShipQuotaDTO { 
    public constructor(obj?: Partial<ShipQuotaDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public associationName?: string;

    @StrictlyTyped(String)
    public shipName?: string;

    @StrictlyTyped(String)
    public shipCFR?: string;

    @StrictlyTyped(String)
    public shipExtMarking?: string;

    @StrictlyTyped(String)
    public fish?: string;

    @StrictlyTyped(Number)
    public year?: number;

    @StrictlyTyped(Number)
    public quotaSize?: number;

    @StrictlyTyped(Number)
    public unloadedByCurrentDateKg?: number;

    @StrictlyTyped(Number)
    public confiscatedQuantity?: number;

    @StrictlyTyped(Number)
    public leftover?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;

    @StrictlyTyped(QuotaHistDTO)
    public changeHistoryRecords?: QuotaHistDTO[];
}