

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class ReportNodeDTO { 
    public constructor(obj?: Partial<ReportNodeDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public iconName?: string;

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;

    @StrictlyTyped(ReportNodeDTO)
    public children?: ReportNodeDTO[];
}