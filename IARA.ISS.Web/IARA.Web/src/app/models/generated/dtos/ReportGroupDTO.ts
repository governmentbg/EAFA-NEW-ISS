

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class ReportGroupDTO { 
    public constructor(obj?: Partial<ReportGroupDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public description?: string;

    @StrictlyTyped(String)
    public groupType?: string;
}