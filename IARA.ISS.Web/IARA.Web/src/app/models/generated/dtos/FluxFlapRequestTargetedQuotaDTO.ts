

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class FluxFlapRequestTargetedQuotaDTO { 
    public constructor(obj?: Partial<FluxFlapRequestTargetedQuotaDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public flapQuotaTypeCode?: string;

    @StrictlyTyped(String)
    public speciesCode?: string;

    @StrictlyTyped(Number)
    public tonnage?: number;
}