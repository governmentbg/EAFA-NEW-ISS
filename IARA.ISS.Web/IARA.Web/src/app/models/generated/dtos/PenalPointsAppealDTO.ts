

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class PenalPointsAppealDTO { 
    public constructor(obj?: Partial<PenalPointsAppealDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public statusId?: number;

    @StrictlyTyped(String)
    public statusName?: string;

    @StrictlyTyped(String)
    public appealNum?: string;

    @StrictlyTyped(String)
    public decreeNum?: string;

    @StrictlyTyped(Number)
    public courtId?: number;

    @StrictlyTyped(Date)
    public appealDate?: Date;

    @StrictlyTyped(Date)
    public decreeDate?: Date;

    @StrictlyTyped(Date)
    public dateOfChange?: Date;

    @StrictlyTyped(String)
    public details?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}