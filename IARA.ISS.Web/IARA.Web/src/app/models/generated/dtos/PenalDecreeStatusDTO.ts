

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class PenalDecreeStatusDTO { 
    public constructor(obj?: Partial<PenalDecreeStatusDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public statusId?: number;

    @StrictlyTyped(String)
    public statusName?: string;

    @StrictlyTyped(Date)
    public dateOfChange?: Date;

    @StrictlyTyped(String)
    public details?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}