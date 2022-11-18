

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class PenalDecreeResolutionDTO { 
    public constructor(obj?: Partial<PenalDecreeResolutionDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public reasons?: string;

    @StrictlyTyped(String)
    public motives?: string;

    @StrictlyTyped(String)
    public zann?: string;

    @StrictlyTyped(String)
    public zra?: string;

    @StrictlyTyped(String)
    public materialEvidence?: string;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}