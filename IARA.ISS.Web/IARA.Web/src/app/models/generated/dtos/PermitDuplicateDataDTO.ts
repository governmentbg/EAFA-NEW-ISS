

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class PermitDuplicateDataDTO { 
    public constructor(obj?: Partial<PermitDuplicateDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Boolean)
    public isOnline?: boolean;

    @StrictlyTyped(Number)
    public permitId?: number;

    @StrictlyTyped(String)
    public permitRegistrationNumber?: string;
}