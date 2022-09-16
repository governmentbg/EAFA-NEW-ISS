

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ReportParameterTypeEnum } from '@app/enums/report-parameter-type.enum';

export class ReportParameterDTO { 
    public constructor(obj?: Partial<ReportParameterDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(Boolean)
    public isActive?: boolean;

    @StrictlyTyped(Boolean)
    public isMandatory?: boolean;

    @StrictlyTyped(Number)
    public id?: number;

    @StrictlyTyped(Number)
    public parameterId?: number;

    @StrictlyTyped(String)
    public parameterName?: string;

    @StrictlyTyped(String)
    public code?: string;

    @StrictlyTyped(Number)
    public dataType?: ReportParameterTypeEnum;

    @StrictlyTyped(String)
    public defaultValue?: string;

    @StrictlyTyped(String)
    public pattern?: string;

    @StrictlyTyped(String)
    public errorMessage?: string;

    @StrictlyTyped(Number)
    public orderNumber?: number;
}