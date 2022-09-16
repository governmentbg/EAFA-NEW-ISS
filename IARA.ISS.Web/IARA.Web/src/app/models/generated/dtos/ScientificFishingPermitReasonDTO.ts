

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class ScientificFishingPermitReasonDTO { 
    public constructor(obj?: Partial<ScientificFishingPermitReasonDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public permitId?: number;

    @StrictlyTyped(Number)
    public permitReasonId?: number;
}