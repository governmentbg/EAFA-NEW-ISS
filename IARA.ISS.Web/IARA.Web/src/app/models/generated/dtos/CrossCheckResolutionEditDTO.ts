

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class CrossCheckResolutionEditDTO { 
    public constructor(obj?: Partial<CrossCheckResolutionEditDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public checkResultId?: number;

    @StrictlyTyped(Number)
    public resolutionId?: number;

    @StrictlyTyped(Date)
    public resolutionDate?: Date;

    @StrictlyTyped(String)
    public resolutionDetails?: string;

    @StrictlyTyped(Number)
    public resolutionTypeId?: number;

    @StrictlyTyped(Date)
    public assignedOn?: Date;
}