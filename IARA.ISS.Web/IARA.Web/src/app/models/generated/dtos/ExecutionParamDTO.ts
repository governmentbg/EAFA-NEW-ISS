

import { StrictlyTyped } from '@app/shared/decorators/strictly-typed.decorator';
import { ReportParameterTypeEnum } from '@app/enums/report-parameter-type.enum';

export class ExecutionParamDTO { 
    public constructor(obj?: Partial<ExecutionParamDTO>) {
        Object.assign(this, obj);
    }

    @StrictlyTyped(String)
    public name?: string;

    @StrictlyTyped(String)
    public value?: string;

    @StrictlyTyped(Number)
    public type?: ReportParameterTypeEnum;

    @StrictlyTyped(String)
    public defaultValue?: string;

    @StrictlyTyped(String)
    public pattern?: string;

    @StrictlyTyped(Boolean)
    public isMandatory?: boolean;
}