

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ReportParameterTypeEnum } from '@app/enums/report-parameter-type.enum';

export class ReportExecutionParameterDTO { 
    public constructor(obj?: Partial<ReportExecutionParameterDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public value?: string;

    @StrictlyTyped(Number)
    public type?: ReportParameterTypeEnum;
}