

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class PenalDecreeFishCompensationDTO { 
    public constructor(obj?: Partial<PenalDecreeFishCompensationDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public fishId?: number;

    @StrictlyTyped(Number)
    public weight?: number;

    @StrictlyTyped(Number)
    public count?: number;

    @StrictlyTyped(Number)
    public unitPrice?: number;

    @StrictlyTyped(Number)
    public totalPrice?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}