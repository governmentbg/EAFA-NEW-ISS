

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class PenalPointsOrderDTO { 
    public constructor(obj?: Partial<PenalPointsOrderDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Boolean)
    public isIncreasePoints?: boolean;

    @StrictlyTyped(String)
    public type?: string;

    @StrictlyTyped(String)
    public decreeNum?: string;

    @StrictlyTyped(Date)
    public issueDate?: Date;

    @StrictlyTyped(Date)
    public effectiveDate?: Date;

    @StrictlyTyped(Date)
    public deliveryDate?: Date;

    @StrictlyTyped(Number)
    public pointsAmount?: number;
}