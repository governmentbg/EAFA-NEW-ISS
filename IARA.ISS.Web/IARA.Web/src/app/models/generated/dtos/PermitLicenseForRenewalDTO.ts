

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class PermitLicenseForRenewalDTO { 
    public constructor(obj?: Partial<PermitLicenseForRenewalDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public registrationNumber?: string;

    @StrictlyTyped(String)
    public holderNames?: string;

    @StrictlyTyped(String)
    public captain?: string;

    @StrictlyTyped(Date)
    public validFrom?: Date;

    @StrictlyTyped(Date)
    public validTo?: Date;

    @StrictlyTyped(String)
    public fishingGears?: string;

    @StrictlyTyped(String)
    public auqticOrganisms?: string;

    @StrictlyTyped(Boolean)
    public isChecked?: boolean;
}