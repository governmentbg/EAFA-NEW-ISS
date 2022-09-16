

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class StatisticalFormNumStatDTO { 
    public constructor(obj?: Partial<StatisticalFormNumStatDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public groupId?: number;

    @StrictlyTyped(Number)
    public statFormId?: number;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public code?: string;

    @StrictlyTyped(String)
    public dataType?: string;

    @StrictlyTyped(Number)
    public statValue?: number;

    @StrictlyTyped(Number)
    public orderNum?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}