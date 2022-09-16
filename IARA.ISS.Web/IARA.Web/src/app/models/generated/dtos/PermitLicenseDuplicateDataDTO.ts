

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class PermitLicenseDuplicateDataDTO { 
    public constructor(obj?: Partial<PermitLicenseDuplicateDataDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Boolean)
    public isOnline?: boolean;

    @StrictlyTyped(Number)
    public permitLicenceId?: number;

    @StrictlyTyped(String)
    public permitLicenceRegistrationNumber?: string;
}