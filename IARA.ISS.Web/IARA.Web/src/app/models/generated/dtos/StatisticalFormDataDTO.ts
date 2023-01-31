

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class StatisticalFormDataDTO { 
    public constructor(obj?: Partial<StatisticalFormDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public year?: number;

    @StrictlyTyped(Date)
    public submissionDate?: Date;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}