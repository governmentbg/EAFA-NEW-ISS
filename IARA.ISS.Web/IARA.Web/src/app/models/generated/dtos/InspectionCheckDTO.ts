

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { InspectionToggleTypesEnum } from '@app/enums/inspection-toggle-types.enum';

export class InspectionCheckDTO { 
    public constructor(obj?: Partial<InspectionCheckDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public checkTypeId?: number;

    @StrictlyTyped(Number)
    public checkValue?: InspectionToggleTypesEnum;

    @StrictlyTyped(String)
    public description?: string;

    @StrictlyTyped(String)
    public number?: string;
}