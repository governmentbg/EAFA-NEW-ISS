

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { AuanRegisterDTO } from './AuanRegisterDTO';
import { InspectionTypesEnum } from '@app/enums/inspection-types.enum';
import { InspectionStatesEnum } from '@app/enums/inspection-states.enum';

export class InspectionDTO { 
    public constructor(obj?: Partial<InspectionDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(String)
    public reportNumber?: string;

    @StrictlyTyped(Date)
    public startDate?: Date;

    @StrictlyTyped(Number)
    public inspectionType?: InspectionTypesEnum;

    @StrictlyTyped(String)
    public inspectionTypeName?: string;

    @StrictlyTyped(Number)
    public inspectionState?: InspectionStatesEnum;

    @StrictlyTyped(String)
    public inspectionStateName?: string;

    @StrictlyTyped(String)
    public inspectors?: string;

    @StrictlyTyped(String)
    public inspectionSubjects?: string;

    @StrictlyTyped(Date)
    public lastUpdateDate?: Date;

    @StrictlyTyped(AuanRegisterDTO)
    public auaNs?: AuanRegisterDTO[];

    @StrictlyTyped(Boolean)
    public isActive?: boolean;
}