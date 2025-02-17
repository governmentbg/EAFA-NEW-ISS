﻿

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class QuotaHistDTO { 
    public constructor(obj?: Partial<QuotaHistDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public quotaId?: number;

    @StrictlyTyped(Number)
    public fromShipQuotaId?: number;

    @StrictlyTyped(String)
    public fromShipQuotaName?: string;

    @StrictlyTyped(Date)
    public timestamp?: Date;

    @StrictlyTyped(Number)
    public addRemoveQuota?: number;

    @StrictlyTyped(Number)
    public newQuotaValueKg?: number;

    @StrictlyTyped(String)
    public basis?: string;

    @StrictlyTyped(Date)
    public periodStart?: Date;

    @StrictlyTyped(Date)
    public periodEnd?: Date;
}