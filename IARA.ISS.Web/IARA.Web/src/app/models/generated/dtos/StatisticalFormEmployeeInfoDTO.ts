

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class StatisticalFormEmployeeInfoDTO { 
    public constructor(obj?: Partial<StatisticalFormEmployeeInfoDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public groupId?: number;

    @StrictlyTyped(Number)
    public statFormId?: number;

    @StrictlyTyped(Number)
    public menWithPay?: number;

    @StrictlyTyped(Number)
    public menWithoutPay?: number;

    @StrictlyTyped(Number)
    public womenWithPay?: number;

    @StrictlyTyped(Number)
    public womenWithoutPay?: number;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public code?: string;

    @StrictlyTyped(Number)
    public orderNum?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}