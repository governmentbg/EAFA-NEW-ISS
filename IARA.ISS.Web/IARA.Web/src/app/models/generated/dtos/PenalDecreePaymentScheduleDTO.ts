

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class PenalDecreePaymentScheduleDTO { 
    public constructor(obj?: Partial<PenalDecreePaymentScheduleDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Date)
    public date?: Date;

    @StrictlyTyped(Number)
    public owedAmount?: number;

    @StrictlyTyped(Number)
    public paidAmount?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}