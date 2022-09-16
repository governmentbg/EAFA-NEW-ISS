

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { QuotaHistDTO } from './QuotaHistDTO';

export class YearlyQuotaDTO { 
    public constructor(obj?: Partial<YearlyQuotaDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public fish?: string;

    @StrictlyTyped(Number)
    public year?: number;

    @StrictlyTyped(Number)
    public quotaValueKg?: number;

    @StrictlyTyped(Number)
    public unloadedQuantity?: number;

    @StrictlyTyped(Number)
    public leftover?: number;

    @StrictlyTyped(Number)
    public confiscatedQuantity?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;

    @StrictlyTyped(QuotaHistDTO)
    public changeHistoryRecords?: QuotaHistDTO[];
}