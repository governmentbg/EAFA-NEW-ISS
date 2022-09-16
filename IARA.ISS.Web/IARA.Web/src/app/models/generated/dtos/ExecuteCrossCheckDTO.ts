

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class ExecuteCrossCheckDTO { 
    public constructor(obj?: Partial<ExecuteCrossCheckDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public reportName?: string;

    @StrictlyTyped(Number)
    public reportId?: number;

    @StrictlyTyped(String)
    public reportSql?: string;

    @StrictlyTyped(Number)
    public checkId?: number;

    @StrictlyTyped(String)
    public checkName?: string;
}