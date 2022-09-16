

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { CrossChecksAutoExecFrequencyEnum } from '@app/enums/cross-checks-auto-exec-frequency.enum';

export class CrossCheckDTO { 
    public constructor(obj?: Partial<CrossCheckDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public code?: string;

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public groupName?: string;

    @StrictlyTyped(String)
    public purpose?: string;

    @StrictlyTyped(Number)
    public errorLevel?: number;

    @StrictlyTyped(String)
    public dataSource?: string;

    @StrictlyTyped(String)
    public dataSourceColumns?: string;

    @StrictlyTyped(String)
    public checkSource?: string;

    @StrictlyTyped(String)
    public checkSourceColumns?: string;

    @StrictlyTyped(String)
    public checkTableName?: string;

    @StrictlyTyped(Number)
    public autoExecFrequency?: CrossChecksAutoExecFrequencyEnum;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}