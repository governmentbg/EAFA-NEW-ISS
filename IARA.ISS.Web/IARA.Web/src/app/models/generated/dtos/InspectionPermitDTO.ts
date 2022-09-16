

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { InspectionToggleTypesEnum } from '@app/enums/inspection-toggle-types.enum';

export class InspectionPermitDTO { 
    public constructor(obj?: Partial<InspectionPermitDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public checkValue?: InspectionToggleTypesEnum;

    @StrictlyTyped(String)
    public description?: string;

    @StrictlyTyped(Number)
    public permitLicenseId?: number;

    @StrictlyTyped(String)
    public permitNumber?: string;

    @StrictlyTyped(String)
    public licenseNumber?: string;

    @StrictlyTyped(Number)
    public typeId?: number;

    @StrictlyTyped(String)
    public typeName?: string;

    @StrictlyTyped(Date)
    public from?: Date;

    @StrictlyTyped(Date)
    public to?: Date;
}