

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class InspectionObservationToolDTO { 
    public constructor(obj?: Partial<InspectionObservationToolDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public observationToolId?: number;

    @StrictlyTyped(String)
    public description?: string;

    @StrictlyTyped(Boolean)
    public isOnBoard?: boolean;

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}