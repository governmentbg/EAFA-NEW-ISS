

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';

export class InspectionPermitLicenseDTO { 
    public constructor(obj?: Partial<InspectionPermitLicenseDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public permitNumber?: string;

    @StrictlyTyped(String)
    public licenseNumber?: string;

    @StrictlyTyped(String)
    public typeName?: string;

    @StrictlyTyped(Number)
    public typeId?: number;

    @StrictlyTyped(Date)
    public validFrom?: Date;

    @StrictlyTyped(Date)
    public validTo?: Date;
}